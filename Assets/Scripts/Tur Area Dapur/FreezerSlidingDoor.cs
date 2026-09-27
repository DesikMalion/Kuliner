using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRGrabInteractable))]
[RequireComponent(typeof(Rigidbody))]
public class FreezerSlidingDoor : MonoBehaviour
{
    public enum SumbuGeser { X, Y, Z }

    [Header("Pengaturan Rel Pintu")]
    [Tooltip("Sumbu pergerakan pintu (local space parent).")]
    public SumbuGeser sumbu = SumbuGeser.Z;

    [Tooltip("Batas minimum posisi lokal")]
    public float batasMin = 0f;

    [Tooltip("Batas maksimum posisi lokal")]
    public float batasMax = 1f;

    private Rigidbody rb;
    private XRGrabInteractable grab;

    private Vector3 posisiAwalLokal;
    private Quaternion rotasiAwalLokal;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grab = GetComponent<XRGrabInteractable>();

        // PENTING: pakai Instantaneous, BUKAN Kinematic.
        // Movement Type "Kinematic" di XR Interaction Toolkit menggerakkan Rigidbody
        // lewat rb.MovePosition() di dalam FixedUpdate miliknya sendiri. Kalau script kita
        // JUGA mengoreksi posisi lewat rb.MovePosition() di FixedUpdate, dua-duanya rebutan
        // siapa yang dieksekusi terakhir tiap physics step -> bisa bikin pintu macet total
        // atau malah "lari" tidak terduga, tergantung urutan eksekusi script.
        //
        // Dengan Instantaneous, XR Toolkit hanya menulis transform.position/rotation biasa
        // tiap Update(). Kita lalu mengoreksi transform.localPosition di LateUpdate(), yang
        // dijamin Unity berjalan SETELAH semua Update() selesai -> tidak ada race condition,
        // koreksi kita selalu jadi kata terakhir sebelum frame dirender.
        grab.movementType = XRBaseInteractable.MovementType.Instantaneous;

        // Rigidbody tetap Kinematic supaya tidak jatuh karena gravity/collision fisik,
        // tapi TIDAK dipakai untuk digerakkan lewat MovePosition oleh script ini.
        rb.isKinematic = true;
        rb.interpolation = RigidbodyInterpolation.None;
        rb.constraints = RigidbodyConstraints.None;
    }

    void Start()
    {
        posisiAwalLokal = transform.localPosition;
        rotasiAwalLokal = transform.localRotation;
    }

    void LateUpdate()
    {
        Vector3 posisiTerkunci = transform.localPosition;

        switch (sumbu)
        {
            case SumbuGeser.X:
                posisiTerkunci.x = Mathf.Clamp(posisiTerkunci.x, batasMin, batasMax);
                posisiTerkunci.y = posisiAwalLokal.y;
                posisiTerkunci.z = posisiAwalLokal.z;
                break;
            case SumbuGeser.Y:
                posisiTerkunci.x = posisiAwalLokal.x;
                posisiTerkunci.y = Mathf.Clamp(posisiTerkunci.y, batasMin, batasMax);
                posisiTerkunci.z = posisiAwalLokal.z;
                break;
            case SumbuGeser.Z:
                posisiTerkunci.x = posisiAwalLokal.x;
                posisiTerkunci.y = posisiAwalLokal.y;
                posisiTerkunci.z = Mathf.Clamp(posisiTerkunci.z, batasMin, batasMax);
                break;
        }

        transform.localPosition = posisiTerkunci;
        transform.localRotation = rotasiAwalLokal;
    }
}