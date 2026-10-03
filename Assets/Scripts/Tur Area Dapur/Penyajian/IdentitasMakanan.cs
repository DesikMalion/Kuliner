using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class IdentitasMakanan : MonoBehaviour
{
    [Header("Identitas Menu")]
    public string namaMenu;

    [HideInInspector] public Vector3 posisiAwal;
    [HideInInspector] public Quaternion rotasiAwal;
    [HideInInspector] public Transform parentAwal;

    // --- Variabel Baru untuk Mencegah Bug Tabrakan ---
    [HideInInspector] public bool sedangDipegangTangan = false;
    [HideInInspector] public float waktuTerakhirDilepas = -999f;

    private void Awake()
    {
        posisiAwal = transform.position;
        rotasiAwal = transform.rotation;
        parentAwal = transform.parent;
        // Menyadap komponen interaksi untuk mendeteksi tangan VR
        var grabObj = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grabObj != null)
        {
            grabObj.selectEntered.AddListener(MulaiDipegang);
            grabObj.selectExited.AddListener(SelesaiDipegang);
        }
    }

    private void Start()
    {
        
    }

    private void MulaiDipegang(SelectEnterEventArgs args)
    {
        // Pastikan yang mengambil adalah Tangan, BUKAN Socket Piring
        if (!(args.interactorObject is XRSocketInteractor))
        {
            sedangDipegangTangan = true;
        }
    }

    private void SelesaiDipegang(SelectExitEventArgs args)
    {
        if (!(args.interactorObject is XRSocketInteractor))
        {
            sedangDipegangTangan = false;
            waktuTerakhirDilepas = Time.time; // Catat detik ke berapa makanan ini dilepas dari tangan
        }
    }
}