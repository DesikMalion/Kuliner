using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

// Membuat daftar pilihan kategori untuk dropdown di Inspector
public enum KategoriSimpan { RakKering, Chiller, Freezer }

[RequireComponent(typeof(XRGrabInteractable))]
public class BahanData : MonoBehaviour
{
    [Header("Informasi Penerimaan")]
    public string namaBahan;
    [TextArea] public string deskripsiKondisi;
    public bool isLayakDiterima;
    [HideInInspector] public bool sudahDisortir = false;

    [Header("Informasi Penyimpanan")]
    public KategoriSimpan kategoriPenyimpanan; // Kategori tempat yang benar
    [HideInInspector] public bool sudahDisimpan = false;

    private Vector3 posisiAwal;
    private Quaternion rotasiAwal;
    private Rigidbody rb;
    private XRGrabInteractable grabInteractable;

    // Variabel pendeteksi zona
    private AreaSortir areaSortirSaatIni;
    private ZonaPenyimpanan zonaSimpanSaatIni;

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
        {
            grabInteractable.selectExited.AddListener(OnDilepas);
        }
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectExited.RemoveListener(OnDilepas);
        }
    }

    // --- LOGIKA PENERIMAAN ---
    public void SetAreaSaatIni(AreaSortir area) { areaSortirSaatIni = area; }
    public void HapusAreaSaatIni(AreaSortir area) { if (areaSortirSaatIni == area) areaSortirSaatIni = null; }

    // --- LOGIKA PENYIMPANAN ---
    public void SetZonaSimpanSaatIni(ZonaPenyimpanan zona) { zonaSimpanSaatIni = zona; }
    public void HapusZonaSimpanSaatIni(ZonaPenyimpanan zona) { if (zonaSimpanSaatIni == zona) zonaSimpanSaatIni = null; }

    private void OnDilepas(SelectExitEventArgs args)
    {
        // Jika dilepas di meja penerimaan
        if (areaSortirSaatIni != null && !sudahDisortir)
        {
            sudahDisortir = true;
            areaSortirSaatIni.ProsesBahanMasuk(this);
        }

        // Jika dilepas di kulkas/rak penyimpanan
        if (zonaSimpanSaatIni != null && !sudahDisimpan)
        {
            sudahDisimpan = true;
            zonaSimpanSaatIni.ProsesBahanMasuk(this);
        }
    }

    public void ResetKePosisiAwal()
    {
        transform.position = posisiAwal;
        transform.rotation = rotasiAwal;

        sudahDisortir = false;
        sudahDisimpan = false;

        areaSortirSaatIni = null;
        zonaSimpanSaatIni = null;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}