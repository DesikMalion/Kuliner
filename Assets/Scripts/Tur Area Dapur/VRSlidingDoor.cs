using UnityEngine;


[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable))]
public class VRSlidingDoor : MonoBehaviour
{
    public enum SumbuGeser { X, Y, Z }

    [Header("Pengaturan Rel Pintu")]
    [Tooltip("Sumbu pergerakan pintu. Berdasarkan gizmo di gambar, gunakan Z.")]
    public SumbuGeser sumbu = SumbuGeser.Z;

    [Tooltip("Batas minimum posisi lokal (titik mentok paling rendah/negatif)")]
    public float batasMin = 0f;

    [Tooltip("Batas maksimum posisi lokal (titik mentok paling tinggi/positif)")]
    public float batasMax = 1f;

    private Vector3 posisiAwalLokal;
    private Quaternion rotasiAwalLokal;

    void Start()
    {
        // Simpan posisi dan rotasi awal saat game mulai
        posisiAwalLokal = transform.localPosition;
        rotasiAwalLokal = transform.localRotation;

        // Memastikan movement type optimal untuk objek yang menempel pada rel
        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab != null && grab.movementType == UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable.MovementType.Instantaneous)
        {
            grab.movementType = UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable.MovementType.Kinematic;
        }
    }

    void LateUpdate()
    {
        // Ambil posisi saat ini (yang sedang ditarik paksa oleh tangan VR)
        Vector3 posisiTerkunci = transform.localPosition;

        // Clamp (batasi) pergerakan hanya pada sumbu yang dipilih, paksa sumbu lain diam
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

        // Timpa posisi objek dengan posisi yang sudah dibatasi
        transform.localPosition = posisiTerkunci;

        // Kunci rotasi agar pintu tidak miring saat ditarik dari ujung pegangan
        transform.localRotation = rotasiAwalLokal;
    }
}