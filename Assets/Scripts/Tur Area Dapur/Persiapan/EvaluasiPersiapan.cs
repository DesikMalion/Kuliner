using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;


[System.Serializable]
public struct DataSpawnBahan
{
    public GameObject prefabBahan;
    public Transform titikMuncul;
}
public class EvaluasiPersiapan : MonoBehaviour
{
    [Header("Statistik Evaluasi")]
    public int jumlahPelanggaranKontaminasi = 0;

    // 2. Daftar untuk menyimpan detail setiap pelanggaran siswa
    [Tooltip("Daftar kesalahan yang akan ditampilkan di UI Laporan Akhir")]
    public List<string> rincianPelanggaran = new List<string>();

    [Header("Audio Feedback")]
    public AudioSource audioSource;
    public AudioClip suaraErrorKontaminasi;
    public AudioClip suaraBenar;

    [Header("Status K3")]
    [Tooltip("Apakah pisau saat ini berada di tempat yang aman?")]
    public bool isPisauAman = true; // Asumsi awal pisau sudah ada di raknya
    public int jumlahPelanggaranK3 = 0;

    [Header("Status K3 Pisau")]
    [Tooltip("Jumlah total pisau yang ada di meja persiapan (misal: 2)")]
    public int totalPisau = 2;
    public int pisauAmanCount => pisauDiRak.Count; // Menghitung berapa pisau yang ada di dalam rak

    [Header("Status Wadah & Potongan")]
    [Tooltip("Total SEMUA potongan (sayur + daging) yang harus masuk ke wadah apapun (misal: 7)")]
    public int totalSemuaPotongan = 7;
    private int potonganMasukCount = 0;
    [HideInInspector] public bool areaSelesai = false;

    public UIManagerPersiapan uiManager;
    private HashSet<GameObject> pisauDiRak = new HashSet<GameObject>();
    private bool sudahDicekK3 = false; // Mencegah CekEvaluasiAkhirArea mencatat pelanggaran dobel jika dipanggil lebih dari sekali

    [Header("Pengaturan Reset Bahan (Prefab)")]
    public GameObject prefabDaging;
    public Transform titikMunculDaging;
    public GameObject prefabSayur;
    public Transform titikMunculSayur;

    [Header("Pengaturan Reset Pisau")]
    public GameObject pisauDaging;
    public Transform rakPisauDaging; // Transform posisi awal pisau / socket
    public GameObject pisauSayur;
    public Transform rakPisauSayur;

    [Header("Referensi Wadah")]
    public SensorWadah[] semuaWadah;

    [Header("Memori Posisi Awal Pisau")]
    private Vector3 posPisauDagingAwal;
    private Quaternion rotPisauDagingAwal;
    private Transform parentPisauDagingAwal;

    private Vector3 posPisauSayurAwal;
    private Quaternion rotPisauSayurAwal;
    private Transform parentPisauSayurAwal;

    [Header("Pengaturan Reset Bahan (Bisa Lebih Dari 1)")]
    [Tooltip("Masukkan semua bahan yang ada di area persiapan ke dalam daftar ini")]
    public DataSpawnBahan[] daftarBahanPersiapan;


    private void Start()
    {
        if (pisauDaging != null)
        {
            posPisauDagingAwal = pisauDaging.transform.position;
            rotPisauDagingAwal = pisauDaging.transform.rotation;
            parentPisauDagingAwal = pisauDaging.transform.parent;
        }

        // Simpan posisi, rotasi, dan parent asli pisau sayur
        if (pisauSayur != null)
        {
            posPisauSayurAwal = pisauSayur.transform.position;
            rotPisauSayurAwal = pisauSayur.transform.rotation;
            parentPisauSayurAwal = pisauSayur.transform.parent;
        }
        //SpawnBahanAwal();
    }

    private void SpawnBahanAwal()
    {
        if (prefabDaging && titikMunculDaging)
            Instantiate(prefabDaging, titikMunculDaging.position, titikMunculDaging.rotation);

        if (prefabSayur && titikMunculSayur)
            Instantiate(prefabSayur, titikMunculSayur.position, titikMunculSayur.rotation);
    }
    public void CatatPelanggaranKontaminasi(string namaBahan, TipeTalenan talenanDipakai, TipeTalenan talenanSeharusnya)
    {
        jumlahPelanggaranKontaminasi++;

        // 3. Merangkai kalimat laporan dan memasukkannya ke dalam buku catatan (List)
        string teksLaporan = $"- {namaBahan} salah talenan (diletakkan di talenan {talenanDipakai}).";
        rincianPelanggaran.Add(teksLaporan);

        //Debug.LogWarning($"KONTAMINASI SILANG: {namaBahan} diletakkan di talenan {talenanDipakai}. Seharusnya di {talenanSeharusnya}!");

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

        //Debug.LogWarning($"KONTAMINASI SILANG: Pisau {pisauDipakai} dipakai untuk {namaBahan}!");

        if (audioSource != null && suaraErrorKontaminasi != null)
        {
            audioSource.PlayOneShot(suaraErrorKontaminasi);
        }
    }

    public void CatatPenempatanBenar()
    {
        //Debug.Log("Penempatan bahan di talenan sudah benar.");
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
        //Debug.Log($"Potongan masuk ke wadah (Benar/Salah). Total: {potonganMasukCount} / {totalSemuaPotongan}");

        // Jika semua bahan (7 potong) sudah masuk ke dalam wadah (terlepas dari wadah mana pun)
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
        if (sudahDicekK3) return; // Sudah pernah dicek, jangan catat dobel
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
                // Jika porsi yang terkumpul tidak sama dengan target instruksi
                if (wadah.potonganTerkumpul != wadah.targetJumlahPotongan)
                {
                    rincianPelanggaran.Add($"- Kesalahan Porsi: Wadah {wadah.bahanYangDiterima} berisi {wadah.potonganTerkumpul} potongan. Seharusnya {wadah.targetJumlahPotongan} potongan.");
                }
            }
        }
    }
    public void PisauMasukTempatAman(SelectEnterEventArgs args)
    {
        pisauDiRak.Add(args.interactableObject.transform.gameObject);
    }
    public void PisauKeluarTempatAman(SelectExitEventArgs args)
    {
        pisauDiRak.Remove(args.interactableObject.transform.gameObject);
    }
    public void SelesaikanAreaPersiapan()
    {
        //Debug.Log("Misi Memotong Selesai! Semua bahan sudah di dalam wadah.");

        // Catatan: pengecekan K3 pisau (CekEvaluasiAkhirArea) TIDAK dipanggil di sini lagi.
        // Ini dipindah ke UIManagerPersiapan.BukaPanelLaporan(), supaya siswa masih
        // punya waktu mengembalikan pisau ke rak sebelum sistem menilai.

        // Munculkan Panel Instruksi yang memiliki tombol Lanjutkan
        if (uiManager != null)
        {
            uiManager.MunculkanPanelAwal();
        }
    }
    public void ResetSemuaSistem()
    {
        // 1. Reset Data Laporan
        jumlahPelanggaranKontaminasi = 0;
        jumlahPelanggaranK3 = 0;
        rincianPelanggaran.Clear();
        //wadahSelesaiCount = 0;
        sudahDicekK3 = false;
        potonganMasukCount = 0;
        areaSelesai = false;

        // 2. Hancurkan semua sisa potongan dan bahan yang ada di meja
        PotonganBahan[] sisaPotongan = FindObjectsOfType<PotonganBahan>();
        foreach (var p in sisaPotongan) Destroy(p.gameObject);

        BahanPersiapan[] sisaBahan = FindObjectsOfType<BahanPersiapan>();
        foreach (var b in sisaBahan) Destroy(b.gameObject);

        // 3. Munculkan ulang bahan utuh dari Prefab (Panggil fungsi baru)
        SpawnBahanAwal();

        // 4. Kembalikan posisi pisau & matikan gaya fisika sisa lemparan
        // 4. Kembalikan posisi pisau & matikan gaya fisika sisa lemparan
        KembalikanPisau(pisauDaging, posPisauDagingAwal, rotPisauDagingAwal, parentPisauDagingAwal);
        KembalikanPisau(pisauSayur, posPisauSayurAwal, rotPisauSayurAwal, parentPisauSayurAwal);

        // 5. Reset semua mangkuk
        foreach (var wadah in semuaWadah)
        {
            if (wadah != null) wadah.ResetWadah();
        }

        //Debug.Log("Sistem berhasil di-reset!");
    }
    public void MulaiAreaPersiapan()
    {
        HapusBahanDiMeja();

        // 2. Spawn bahan baru secara INDEPENDEN (tanpa Parent)
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

        // 1. Matikan komponen Grab sejenak untuk memaksa tangan/socket melepasnya (tanpa membunuh skrip kecepatan)
        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab = pisau.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        if (grab != null) grab.enabled = false;

        // 2. Kembalikan pisau menjadi Child dari Rak
        pisau.transform.SetParent(parentAwal);

        // 3. Terapkan posisi dan rotasi presisi
        pisau.transform.position = posAwal;
        pisau.transform.rotation = rotAwal;

        // 4. Hentikan sisa gaya pantulan fisika
        Rigidbody rb = pisau.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        // 5. Hidupkan kembali komponen Grab agar bisa dipegang lagi
        if (grab != null) grab.enabled = true;
    }
}