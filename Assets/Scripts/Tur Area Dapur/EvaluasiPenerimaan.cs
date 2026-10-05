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

    [Header("Referensi UI Panel (Satu Pintu)")]
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

    public BahanData[] semuaBahan;

    private void Start()
    {
        // Saat simulasi dimulai, atur otomatis ke fase instruksi
        AturFaseInstruksi();
    }

    private void AturFaseInstruksi()
    {
        // Menyalakan teks instruksi dan menyembunyikan elemen laporan serta tombol
        if (objekInstruksi != null) objekInstruksi.SetActive(true);
        if (objekLaporan != null) objekLaporan.SetActive(false);
        if (tombolLanjutkan != null) tombolLanjutkan.SetActive(false);
        if (tombolReset != null) tombolReset.SetActive(false);
    }

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
            // PlayOneShot memastikan suara tidak bertabrakan jika ada 2 barang masuk bersamaan
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

        // Matikan instruksi, dan munculkan hasil laporan beserta tombol aksi
        if (objekInstruksi != null) objekInstruksi.SetActive(false);
        if (objekLaporan != null) objekLaporan.SetActive(true);
        if (tombolLanjutkan != null) tombolLanjutkan.SetActive(true);
        if (tombolReset != null) tombolReset.SetActive(true);
    }

    public void KlikLanjutkan()
    {
        // Menyembunyikan seluruh Canvas/Panel Induk ini dari hadapan pemain
        if (objekInstruksi != null)
        {
            objekInstruksi.SetActive(true);
            objekLaporan.SetActive(false);
        }

        if (progressManager != null)
        {
            progressManager.TambahTugasSelesai();
        }
    }

    public void KlikReset()
    {
        // 1. Sembunyikan panel evaluasi dan kembalikan ke tampilan instruksi
        AturFaseInstruksi();

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
    }
}