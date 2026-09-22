using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManagerPersiapan : MonoBehaviour
{
    [Header("Pengaturan Panel")]
    public GameObject panelInstruksi;
    public GameObject panelLaporan;

    [Header("Tombol Laporan")]
    [Tooltip("Tombol untuk membuka laporan. Nonaktif (disabled) sampai semua instruksi/aktivitas selesai.")]
    public GameObject tombolLaporan;

    [Header("Pengaturan Teks UI (TextMeshPro)")]
    public TextMeshProUGUI teksKesimpulan;
    public TextMeshProUGUI teksRincian;

    [Header("Sumber Data")]
    public EvaluasiPersiapan managerPersiapan;

    private void Start()
    {
        // Kunci tombol laporan di awal permainan, sebelum semua aktivitas selesai
        if (tombolLaporan != null)
        {
            tombolLaporan.gameObject.SetActive(false);
        }
    }

    // Dipanggil otomatis oleh EvaluasiPersiapan saat SEMUA instruksi/aktivitas sudah selesai
    public void MunculkanPanelAwal()
    {
        panelInstruksi.SetActive(true);
        panelLaporan.SetActive(false);

        // Baru sekarang tombol laporan boleh ditekan siswa
        if (tombolLaporan != null)
        {
            tombolLaporan.gameObject.SetActive(true);
        }
    }

    // Fungsi ini dipanggil saat siswa menekan tombol "Lanjutkan"
    public void BukaPanelLaporan()
    {
        panelInstruksi.SetActive(false);
        panelLaporan.SetActive(true);

        // Cek K3 pisau di momen paling akhir ini, agar siswa punya waktu maksimal
        // untuk mengembalikan pisau sebelum dinilai
        if (managerPersiapan != null)
        {
            managerPersiapan.CekEvaluasiAkhirArea();
        }

        TulisLaporanKeLayar();
    }

    private void TulisLaporanKeLayar()
    {
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

            // Menggabungkan semua isi List menjadi satu paragraf yang rapi
            teksRincian.text = string.Join("\n", managerPersiapan.rincianPelanggaran);
        }
    }
    public void TekanTombolReset()
    {
        if (managerPersiapan != null)
        {
            managerPersiapan.ResetSemuaSistem();
        }

        // Kembalikan UI ke panel instruksi awal
        panelLaporan.SetActive(false);
        panelInstruksi.SetActive(true);

        if (tombolLaporan != null)
        {
            tombolLaporan.gameObject.SetActive(false);
        }
    }
}