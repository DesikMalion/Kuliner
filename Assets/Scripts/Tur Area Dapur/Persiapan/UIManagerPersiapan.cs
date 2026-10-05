using UnityEngine;
using TMPro;

public class UIManagerPersiapan : MonoBehaviour
{
    [Header("Referensi UI Panel (Satu Pintu)")]
    public GameObject panelVisualUtama; // Papan utama pembungkus UI
    public GameObject objekInstruksi;
    public GameObject objekLaporan;
    public GameObject tombolLanjutkan;
    public GameObject tombolReset;

    [Header("Pengaturan Teks UI")]
    public TextMeshProUGUI teksKesimpulan;
    public TextMeshProUGUI teksRincian;

    [Header("Referensi Manager")]
    public EvaluasiPersiapan managerPersiapan;
    public AreaProgressManager progressManager;

    private void Start()
    {
        // Pastikan selalu mulai di fase instruksi saat awal
        AturFaseInstruksi();
    }

    public void AturFaseInstruksi()
    {
        //if (panelVisualUtama != null) panelVisualUtama.SetActive(true);
        if (objekInstruksi != null) objekInstruksi.SetActive(true);
        if (objekLaporan != null) objekLaporan.SetActive(false);
        if (tombolLanjutkan != null) tombolLanjutkan.SetActive(false);
        if (tombolReset != null) tombolReset.SetActive(false);
    }

    // Dipanggil otomatis oleh EvaluasiPersiapan saat potongan terakhir masuk wadah
    public void MunculkanLaporanAkhir()
    {
        // Cek K3 pisau di momen paling akhir sebelum laporan dicetak
        if (managerPersiapan != null)
        {
            managerPersiapan.CekEvaluasiAkhirArea();
        }

        TulisLaporanKeLayar();

        // Ganti tampilan dari Instruksi ke Laporan
        if (objekInstruksi != null) objekInstruksi.SetActive(false);
        if (objekLaporan != null) objekLaporan.SetActive(true);
        if (tombolLanjutkan != null) tombolLanjutkan.SetActive(true);
        if (tombolReset != null) tombolReset.SetActive(true);
    }

    private void TulisLaporanKeLayar()
    {
        if (managerPersiapan == null) return;

        // Jika list pelanggaran kosong, berarti siswa bekerja sempurna
        if (managerPersiapan.rincianPelanggaran.Count == 0)
        {
            teksKesimpulan.text = "Luar Biasa! Persiapan bahan tuntas tanpa kesalahan.";
            teksKesimpulan.color = Color.green;
            teksRincian.text = "<b>Tindakan Benar:</b>\n- Pemilihan warna talenan tepat.\n- Penggunaan pisau sesuai standar HACCP.\n- K3: Pisau dikembalikan ke tempat aman.";
        }
        else
        {
            teksKesimpulan.text = $"Evaluasi Selesai. Ditemukan {managerPersiapan.rincianPelanggaran.Count} Pelanggaran Prosedur:";
            teksKesimpulan.color = Color.red;
            teksRincian.text = string.Join("\n", managerPersiapan.rincianPelanggaran);
        }
    }

    public void KlikLanjutkan()
    {
        // Menyembunyikan seluruh papan visual
        if (panelVisualUtama != null) panelVisualUtama.SetActive(false);

        if (progressManager != null) progressManager.TambahTugasSelesai();
    }

    public void KlikReset()
    {
        // 1. Bersihkan 3D objek melalui manager persiapan
        if (managerPersiapan != null)
        {
            managerPersiapan.ResetSemuaSistem();
        }

        // 2. Kembalikan UI ke panel instruksi awal
        AturFaseInstruksi();
    }
}