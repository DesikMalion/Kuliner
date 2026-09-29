using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EvaluasiMemasak : MonoBehaviour
{
    public static EvaluasiMemasak Instance { get; private set; }

    [Header("Referensi UI Laporan Akhir")]
    [Tooltip("Panel Canvas besar yang muncul di akhir area")]
    public GameObject panelLaporanAkhir;
    public TextMeshProUGUI teksRingkasan;
    public TextMeshProUGUI teksRincianKesalahan;

    [Header("Data Evaluasi (Terisi Otomatis)")]
    public int totalAlat = 0;
    private int alatSelesai = 0;
    private int totalKesalahan = 0;

    public AreaProgressManager progressManager;


    // Menyimpan jumlah salah per alat (misal: "Oven" -> 2 kali salah)
    private Dictionary<string, int> rekapKesalahan = new Dictionary<string, int>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        // Pastikan panel laporan tertutup di awal
        if (panelLaporanAkhir != null) panelLaporanAkhir.SetActive(false);
    }

    // Dipanggil saat siswa salah menjawab kuis
    public void CatatKesalahan(string namaAlat)
    {
        totalKesalahan++;

        if (rekapKesalahan.ContainsKey(namaAlat))
        {
            rekapKesalahan[namaAlat]++;
        }
        else
        {
            rekapKesalahan.Add(namaAlat, 1);
        }
    }

    // Dipanggil saat siswa menjawab benar
    public void CatatAlatSelesai()
    {
        alatSelesai++;

        // Cek apakah semua alat sudah dijawab benar
        if (alatSelesai >= totalAlat && totalAlat > 0)
        {
            TampilkanLaporan();
        }
    }

    private void TampilkanLaporan()
    {
        // 1. Munculkan Panel
        if (panelLaporanAkhir != null) panelLaporanAkhir.SetActive(true);

        // 2. Isi Teks Ringkasan Skor
        if (teksRingkasan != null)
        {
            teksRingkasan.text = $"<b>Area Memasak Selesai!</b>\nAlat Dipelajari: {alatSelesai} / {totalAlat}\nTotal Kesalahan: {totalKesalahan}";
        }

        // 3. Isi Teks Rincian Kesalahan
        if (teksRincianKesalahan != null)
        {
            if (rekapKesalahan.Count == 0)
            {
                teksRincianKesalahan.text = "<color=green>Sempurna! Anda memahami semua fungsi alat tanpa kesalahan.</color>";
            }
            else
            {
                string rincian = "<b>Rincian Kesalahan Evaluasi:</b>\n";
                foreach (var item in rekapKesalahan)
                {
                    rincian += $"- {item.Key} : Salah menebak {item.Value} kali\n";
                }
                teksRincianKesalahan.text = rincian;
            }
        }
    }
    public void KlikLanjutkan()
    {
        if (panelLaporanAkhir != null)
        {
            panelLaporanAkhir.SetActive(false);
        }

        if (progressManager != null)
        {
            progressManager.TambahTugasSelesai();
        }
    }
}