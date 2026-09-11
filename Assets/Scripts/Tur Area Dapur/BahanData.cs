using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// Kategori penyimpanan berdasarkan tabel Anda
public enum TipePenyimpanan
{
    BelumDitentukan,
    RakBahanKering,
    LemariPendingin,
    Freezer
}

[RequireComponent(typeof(XRGrabInteractable))]
public class BahanData : MonoBehaviour
{
    [Header("Informasi Penerimaan")]
    public string namaBahan;
    [TextArea] public string deskripsiKondisi;
    public bool isLayakDiterima;

    [Header("Informasi Penyimpanan")]
    public TipePenyimpanan tempatSeharusnya;

    [HideInInspector] public bool sudahDisortir = false;
    [HideInInspector] public bool sudahDisimpan = false;

    private Vector3 posisiAwal;
    private Quaternion rotasiAwal;
    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;

    private AreaSortir areaSortirSaatIni;
    private AreaSortir areaSimpanSaatIni;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void Start()
    {
        posisiAwal = transform.position;
        rotasiAwal = transform.rotation;
    }

    private void OnEnable()
    {
        if (grabInteractable != null)
            grabInteractable.selectExited.AddListener(OnDilepas);
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
            grabInteractable.selectExited.RemoveListener(OnDilepas);
    }

    // --- Logika Area Penerimaan ---
    public void SetAreaSortir(AreaSortir area) { areaSortirSaatIni = area; }
    public void HapusAreaSortir(AreaSortir area) { if (areaSortirSaatIni == area) areaSortirSaatIni = null; }

    // --- Logika Area Penyimpanan ---
    public void SetAreaSimpan(AreaSortir area) { areaSimpanSaatIni = area; }
    public void HapusAreaSimpan(AreaSortir area) { if (areaSimpanSaatIni == area) areaSimpanSaatIni = null; }

    // Dieksekusi saat user melepaskan Grip/Trigger
    private void OnDilepas(SelectExitEventArgs args)
    {
        // Cek apakah dilepas di Area Penerimaan
        if (areaSortirSaatIni != null && !sudahDisortir)
        {
            sudahDisortir = true;
            areaSortirSaatIni.ProsesBahanMasuk(this);
        }
        // Cek apakah dilepas di Area Penyimpanan
        else if (areaSimpanSaatIni != null && !sudahDisimpan)
        {
            sudahDisimpan = true;
            areaSimpanSaatIni.ProsesBahanMasuk(this);
        }
    }

    public void ResetKePosisiAwal()
    {
        transform.position = posisiAwal;
        transform.rotation = rotasiAwal;

        sudahDisortir = false;
        sudahDisimpan = false;
        areaSortirSaatIni = null;
        areaSimpanSaatIni = null;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}