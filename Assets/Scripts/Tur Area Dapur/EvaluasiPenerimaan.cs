using UnityEngine;
using TMPro;

public class EvaluasiPenerimaan : MonoBehaviour
{
    [Header("Pengaturan Sortir")]
    public int totalBahan = 5;
    private int bahanDiproses = 0;
    private int jumlahBenar = 0;
    private int jumlahSalah = 0;

    public AreaProgressManager progressManager;

    [Header("UI Laporan")]
    public GameObject panelLaporan;
    public TextMeshProUGUI teksStatistik;
    public TextMeshProUGUI teksRemark;

    [Header("Audio Feedback")]
    public AudioSource audioSource;
    public AudioClip suaraBenar;
    public AudioClip suaraSalah;

    public BahanData[] semuaBahan;
    public void ProsesBahan(BahanData bahan, bool masukAreaTerima)
    {
        bahanDiproses++;

        // Evaluasi logika: jika (masuk terima DAN bahan bagus) ATAU (masuk tolak DAN bahan jelek)
        bool jawabanBenar = (masukAreaTerima && bahan.isLayakDiterima) || (!masukAreaTerima && !bahan.isLayakDiterima);

        if (jawabanBenar)
        {
            jumlahBenar++;
            MainkanSuara(suaraBenar); // Putar nada sukses
        }
        else
        {
            jumlahSalah++;
            MainkanSuara(suaraSalah); // Putar nada gagal
        }

        if (bahanDiproses >= totalBahan)
        {
            TampilkanLaporan();
        }
    }

    private void MainkanSuara(AudioClip klipSuara)
    {
        if (audioSource != null && klipSuara != null)
        {
            // PlayOneShot memastikan suara tidak bertabrakan terpotong jika ada 2 barang masuk bersamaan
            audioSource.PlayOneShot(klipSuara);
        }
    }

    private void TampilkanLaporan()
    {
        teksStatistik.text = $"Statistik Penyortiran:\n" +
                             $"- Total Bahan Keseluruhan : {totalBahan}\n" +
                             $"- Jumlah Sortir Benar     : {jumlahBenar}\n" +
                             $"- Jumlah Sortir Salah     : {jumlahSalah}";

        if (jumlahSalah == 0)
        {
            teksRemark.text = "Sempurna! Semua bahan disortir dengan tepat sesuai standar kelayakan.";
            teksRemark.color = Color.green;
        }
        else if (jumlahSalah <= 2)
        {
            teksRemark.text = $"Hampir sempurna! Namun ada {jumlahSalah} bahan yang salah disortir. Lebih teliti lagi dalam mengecek kemasan dan kondisi bahan.";
            teksRemark.color = Color.yellow;
        }
        else
        {
            teksRemark.text = "Masih banyak kesalahan penyortiran. Di dapur komersial, menerima bahan rusak dapat membahayakan konsumen. Perhatikan kembali standar bahan!";
            teksRemark.color = Color.red;
        }

        panelLaporan.SetActive(true);
    }

    public void KlikLanjutkan()
    {
        panelLaporan.SetActive(false);
        if (progressManager != null)
        {
            progressManager.TambahTugasSelesai();
        }
    }
    public void KlikReset()
    {
        // 1. Sembunyikan panel evaluasi
        panelLaporan.SetActive(false);

        // 2. Kembalikan semua bahan ke posisi semula di atas meja
        foreach (BahanData bahan in semuaBahan)
        {
            if (bahan != null)
            {
                bahan.ResetKePosisiAwal();
            }
        }

        // 3. Kembalikan skor perhitungan ke angka nol
        bahanDiproses = 0;
        jumlahBenar = 0;
        jumlahSalah = 0;

        Debug.Log("Simulasi di-reset. Silakan mulai menyortir lagi.");
    }
}