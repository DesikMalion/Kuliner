using UnityEngine;
using TMPro;

public class EvaluasiPenyimpanan : MonoBehaviour
{
    [Header("Pengaturan Evaluasi")]
    public int totalBahanDisimpan = 5;
    private int bahanDiproses = 0;
    private int jumlahBenar = 0;
    private int jumlahSalah = 0;

    public AreaProgressManager progressManager;
    public BahanData[] semuaBahan; // Untuk fungsi reset

    [Header("Referensi UI Panel (Satu Pintu)")]
    public GameObject panelVisualUtama; // Papan utama pembungkus UI
    public GameObject objekInstruksi;
    public GameObject objekLaporan;
    public GameObject tombolLanjutkan;
    public GameObject tombolReset;

    [Header("Teks Laporan")]
    public TextMeshProUGUI teksStatistik;
    public TextMeshProUGUI teksRemark;

    [Header("Audio Feedback")]
    public AudioSource audioSource;
    public AudioClip suaraBenar;
    public AudioClip suaraSalah;

    private void Start()
    {
        // Pastikan selalu mulai di fase instruksi
        AturFaseInstruksi();
    }

    private void AturFaseInstruksi()
    {
        //if (panelVisualUtama != null) panelVisualUtama.SetActive(true);
        if (objekInstruksi != null) objekInstruksi.SetActive(true);
        if (objekLaporan != null) objekLaporan.SetActive(false);
        if (tombolLanjutkan != null) tombolLanjutkan.SetActive(false);
        if (tombolReset != null) tombolReset.SetActive(false);
    }

    public void ProsesPenyimpanan(BahanData bahan, KategoriSimpan zonaTujuan)
    {
        bahanDiproses++;

        // Cek apakah kategori bahan cocok dengan kategori zona tempat dia dilepas
        if (bahan.kategoriPenyimpanan == zonaTujuan)
        {
            jumlahBenar++;
            MainkanSuara(suaraBenar);
        }
        else
        {
            jumlahSalah++;
            MainkanSuara(suaraSalah);
            Debug.Log($"{bahan.namaBahan} salah tempat! Ditaruh di {zonaTujuan}, seharusnya {bahan.kategoriPenyimpanan}");
        }

        if (bahanDiproses >= totalBahanDisimpan)
        {
            TampilkanLaporan();
        }
    }

    private void MainkanSuara(AudioClip klip)
    {
        if (audioSource != null && klip != null) audioSource.PlayOneShot(klip);
    }

    private void TampilkanLaporan()
    {
        teksStatistik.text = $"Statistik Penyimpanan:\n" +
                             $"- Total Bahan       : {totalBahanDisimpan}\n" +
                             $"- Posisi Benar      : {jumlahBenar}\n" +
                             $"- Posisi Salah      : {jumlahSalah}";

        if (jumlahSalah == 0)
        {
            teksRemark.text = "Sempurna! Anda memahami tata letak penyimpanan dan menghindari kontaminasi silang.";
            teksRemark.color = Color.green;
        }
        else
        {
            teksRemark.text = "Hati-hati! Menyimpan bahan mentah atau kering di suhu yang salah dapat memicu bakteri dan kerusakan bahan.";
            teksRemark.color = Color.red;
        }

        // Matikan instruksi, dan munculkan hasil laporan beserta tombol aksi
        if (objekInstruksi != null) objekInstruksi.SetActive(false);
        if (objekLaporan != null) objekLaporan.SetActive(true);
        if (tombolLanjutkan != null) tombolLanjutkan.SetActive(true);
        if (tombolReset != null) tombolReset.SetActive(true);
    }

    public void KlikLanjutkan()
    {
        // Matikan seluruh papan visual (termasuk background, judul, laporan, dan tombol) agar bersih
        if (objekInstruksi != null)
        {
            objekInstruksi.SetActive(true);
            objekLaporan.SetActive(false);
        }

        if (progressManager != null) progressManager.TambahTugasSelesai();
    }

    public void KlikReset()
    {
        // 1. Sembunyikan panel evaluasi dan kembalikan ke tampilan instruksi
        AturFaseInstruksi();

        // 2. Kembalikan semua bahan ke posisi semula di atas meja
        foreach (BahanData bahan in semuaBahan)
        {
            if (bahan != null) bahan.ResetKePosisiAwal();
        }

        // 3. Kembalikan skor perhitungan ke angka nol
        bahanDiproses = 0;
        jumlahBenar = 0;
        jumlahSalah = 0;
    }
}