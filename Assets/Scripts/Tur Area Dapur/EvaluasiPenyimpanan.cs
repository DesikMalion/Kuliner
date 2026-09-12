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

    [Header("UI Laporan")]
    public GameObject panelLaporan;
    public TextMeshProUGUI teksStatistik;
    public TextMeshProUGUI teksRemark;

    [Header("Audio Feedback")]
    public AudioSource audioSource;
    public AudioClip suaraBenar;
    public AudioClip suaraSalah;

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

        panelLaporan.SetActive(true);
    }

    public void KlikLanjutkan()
    {
        panelLaporan.SetActive(false);
        if (progressManager != null) progressManager.TambahTugasSelesai();
    }

    public void KlikReset()
    {
        panelLaporan.SetActive(false);
        foreach (BahanData bahan in semuaBahan)
        {
            if (bahan != null) bahan.ResetKePosisiAwal();
        }
        bahanDiproses = 0;
        jumlahBenar = 0;
        jumlahSalah = 0;
    }
}