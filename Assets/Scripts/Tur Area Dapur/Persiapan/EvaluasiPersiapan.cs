using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[System.Serializable]
public struct DataSpawnBahan
{
    public GameObject prefabBahan;
    public Transform titikMuncul;
}

// --- CLASS BARU UNTUK PISAU DINAMIS ---
[System.Serializable]
public class DataPisau
{
    public GameObject objekPisau;

    // Variabel memori ini disembunyikan agar Inspector tetap rapi, 
    // karena akan diisi otomatis oleh script saat Start()
    [HideInInspector] public Vector3 posAwal;
    [HideInInspector] public Quaternion rotAwal;
    [HideInInspector] public Transform parentAwal;
}

public class EvaluasiPersiapan : MonoBehaviour
{
    [Header("Statistik Evaluasi")]
    public int jumlahPelanggaranKontaminasi = 0;

    [Tooltip("Daftar kesalahan yang akan ditampilkan di UI Laporan Akhir")]
    public List<string> rincianPelanggaran = new List<string>();

    [Header("Audio Feedback")]
    public AudioSource audioSource;
    public AudioClip suaraErrorKontaminasi;
    public AudioClip suaraBenar;

    [Header("Status K3")]
    [Tooltip("Apakah pisau saat ini berada di tempat yang aman?")]
    public bool isPisauAman = true;
    public int jumlahPelanggaranK3 = 0;

    [Header("Status K3 Pisau")]
    [Tooltip("Jumlah total pisau yang ada di meja persiapan (misal: 2)")]
    [HideInInspector] public int totalPisau;
    public int pisauAmanCount => pisauDiRak.Count;

    [Header("Status Wadah & Potongan")]
    [Tooltip("Total SEMUA potongan (sayur + daging dll) yang harus masuk ke wadah apapun (misal: 7)")]
    public int totalSemuaPotongan = 7;
    private int potonganMasukCount = 0;
    [HideInInspector] public bool areaSelesai = false;

    public UIManagerPersiapan uiManager;
    private HashSet<GameObject> pisauDiRak = new HashSet<GameObject>();
    private bool sudahDicekK3 = false;

    // --- PENGATURAN DINAMIS BAHAN & PISAU ---
    [Header("Pengaturan Reset Bahan (Dinamis)")]
    [Tooltip("Masukkan semua bahan yang ada di area persiapan ke dalam daftar ini")]
    public DataSpawnBahan[] daftarBahanPersiapan;

    [Header("Pengaturan Reset Pisau (Dinamis)")]
    [Tooltip("Masukkan semua pisau yang digunakan di area ini")]
    public DataPisau[] daftarPisau;

    [Header("Referensi Wadah")]
    public SensorWadah[] semuaWadah;

    private void Start()
    {
        if (daftarPisau != null)
        {
            totalPisau = daftarPisau.Length;
        }
        // Menyimpan memori posisi untuk BERAPAPUN jumlah pisau yang didaftarkan
        foreach (DataPisau pisau in daftarPisau)
        {
            if (pisau.objekPisau != null)
            {
                pisau.posAwal = pisau.objekPisau.transform.position;
                pisau.rotAwal = pisau.objekPisau.transform.rotation;
                pisau.parentAwal = pisau.objekPisau.transform.parent;
            }
        }
    }

    public void CatatPelanggaranKontaminasi(string namaBahan, TipeTalenan talenanDipakai, TipeTalenan talenanSeharusnya)
    {
        jumlahPelanggaranKontaminasi++;
        string teksLaporan = $"- {namaBahan} salah talenan (diletakkan di talenan {talenanDipakai}).";
        rincianPelanggaran.Add(teksLaporan);

        if (audioSource != null && suaraErrorKontaminasi != null)
        {
            audioSource.PlayOneShot(suaraErrorKontaminasi);
        }
    }

    public void CatatPelanggaranPisau(string namaBahan, TipeTalenan pisauDipakai, TipeTalenan pisauSeharusnya)
    {
        jumlahPelanggaranKontaminasi++;
        string teksLaporan = $"- Salah Pisau: {namaBahan} dipotong menggunakan pisau {pisauDipakai}. Seharusnya pisau {pisauSeharusnya}.";
        rincianPelanggaran.Add(teksLaporan);

        if (audioSource != null && suaraErrorKontaminasi != null)
        {
            audioSource.PlayOneShot(suaraErrorKontaminasi);
        }
    }

    public void CatatPenempatanBenar()
    {
        if (audioSource != null && suaraBenar != null)
        {
            audioSource.PlayOneShot(suaraBenar);
        }
    }

    public void UpdateStatusPisau(bool statusAman)
    {
        isPisauAman = statusAman;
    }

    public void LaporPotonganMasuk()
    {
        if (areaSelesai) return;

        potonganMasukCount++;

        if (potonganMasukCount >= totalSemuaPotongan)
        {
            areaSelesai = true;
            SelesaikanAreaPersiapan();
        }
    }

    public void LaporPotonganKeluar()
    {
        if (areaSelesai) return;
        if (potonganMasukCount > 0) potonganMasukCount--;
    }

    public void CekEvaluasiAkhirArea()
    {
        //Debug.Log($"[CEK K3] Target Total Pisau: {totalPisau} | Pisau di Rak Saat Ini: {pisauAmanCount}");
        if (sudahDicekK3) return;
        sudahDicekK3 = true;

        // 1. Audit K3 Pisau
        if (pisauAmanCount < totalPisau)
        {
            jumlahPelanggaranK3++;
            rincianPelanggaran.Add($"- Pelanggaran K3: Ada {totalPisau - pisauAmanCount} pisau diletakkan sembarangan (tidak dikembalikan).");

            if (audioSource != null && suaraErrorKontaminasi != null)
                audioSource.PlayOneShot(suaraErrorKontaminasi);
        }

        // 2. Audit Akurasi Porsi per Wadah
        foreach (var wadah in semuaWadah)
        {
            if (wadah != null)
            {
                if (wadah.potonganTerkumpul != wadah.targetJumlahPotongan)
                {
                    rincianPelanggaran.Add($"- Kesalahan Porsi: Wadah {wadah.bahanYangDiterima} berisi {wadah.potonganTerkumpul} potongan. Seharusnya {wadah.targetJumlahPotongan} potongan.");
                }
            }
        }
    }
    private bool ApakahIniPisau(GameObject benda)
    {
        // Cek apakah benda yang dicek ada di dalam daftar pisau kita
        foreach (DataPisau data in daftarPisau)
        {
            if (data.objekPisau == benda) return true;
        }
        return false;
    }
    public void PisauMasukTempatAman(SelectEnterEventArgs args)
    {
        GameObject bendaMasuk = args.interactableObject.transform.gameObject;

        // Hanya masukkan ke hitungan jika benda tersebut BENAR-BENAR pisau
        if (ApakahIniPisau(bendaMasuk))
        {
            pisauDiRak.Add(bendaMasuk);
            //Debug.Log($"[K3] {bendaMasuk.name} masuk ke rak dengan aman.");
        }
    }

    public void PisauKeluarTempatAman(SelectExitEventArgs args)
    {
        GameObject bendaKeluar = args.interactableObject.transform.gameObject;

        if (ApakahIniPisau(bendaKeluar))
        {
            pisauDiRak.Remove(bendaKeluar);
            //Debug.Log($"[K3] AWAS! {bendaKeluar.name} dikeluarkan dari rak.");
        }
    }

    public void SelesaikanAreaPersiapan()
    {
        if (uiManager != null)
        {
            uiManager.MunculkanPanelAwal();
        }
    }

    public void ResetSemuaSistem()
    {
        jumlahPelanggaranKontaminasi = 0;
        jumlahPelanggaranK3 = 0;
        rincianPelanggaran.Clear();
        sudahDicekK3 = false;
        potonganMasukCount = 0;
        areaSelesai = false;

        // Reset Bahan menggunakan fungsi yang sama saat awal mulai
        MulaiAreaPersiapan();

        // Mengembalikan BERAPAPUN jumlah pisau yang ada di daftar
        foreach (DataPisau pisau in daftarPisau)
        {
            KembalikanPisau(pisau.objekPisau, pisau.posAwal, pisau.rotAwal, pisau.parentAwal);
        }

        // Reset semua mangkuk
        foreach (var wadah in semuaWadah)
        {
            if (wadah != null) wadah.ResetWadah();
        }
    }

    public void MulaiAreaPersiapan()
    {
        HapusBahanDiMeja();

        foreach (var data in daftarBahanPersiapan)
        {
            if (data.prefabBahan != null && data.titikMuncul != null)
            {
                Instantiate(data.prefabBahan, data.titikMuncul.position, data.titikMuncul.rotation);
            }
        }
    }

    public void HapusBahanDiMeja()
    {
        PotonganBahan[] sisaPotongan = FindObjectsOfType<PotonganBahan>();
        foreach (var p in sisaPotongan) Destroy(p.gameObject);

        BahanPersiapan[] sisaBahan = FindObjectsOfType<BahanPersiapan>();
        foreach (var b in sisaBahan) Destroy(b.gameObject);
    }

    private void KembalikanPisau(GameObject pisau, Vector3 posAwal, Quaternion rotAwal, Transform parentAwal)
    {
        if (pisau == null) return;

        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab = pisau.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab != null) grab.enabled = false;

        pisau.transform.SetParent(parentAwal);
        pisau.transform.position = posAwal;
        pisau.transform.rotation = rotAwal;

        Rigidbody rb = pisau.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (grab != null) grab.enabled = true;
    }
}