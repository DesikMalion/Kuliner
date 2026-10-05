using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EvaluasiMemasak : MonoBehaviour
{
    public static EvaluasiMemasak Instance { get; private set; }

    [Header("Referensi UI Panel (Satu Pintu)")]
    public GameObject panelVisualUtama; // Papan utama pembungkus UI
    public GameObject objekInstruksi;   // GameObject 'Instruksi'
    public GameObject objekLaporan;     // GameObject 'Laporan'

    [Header("Referensi Teks Laporan Akhir")]
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
    }

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
        // Matikan panel instruksi (beserta tombol fase-nya) dan munculkan panel laporan
        if (objekInstruksi != null) objekInstruksi.SetActive(false);
        if (objekLaporan != null) objekLaporan.SetActive(true);

        if (teksRingkasan != null)
        {
            teksRingkasan.text = $"<b>Area Memasak Selesai!</b>\nAlat Dipelajari: {alatSelesai} / {totalAlat}\nTotal Kesalahan: {totalKesalahan}";
        }

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
                    rincian += $"- {item.Key} : Salah menjawab {item.Value} kali\n";
                }
                teksRincianKesalahan.text = rincian;
            }
        }
    }

    public void KlikLanjutkan()
    {
        // Sembunyikan seluruh papan visual dari pandangan
        if (panelVisualUtama != null) panelVisualUtama.SetActive(false);

        if (progressManager != null)
        {
            progressManager.TambahTugasSelesai();
        }
    }

    public void KlikReset()
    {
        // Reset skor dan kembalikan tampilan
        alatSelesai = 0;
        totalKesalahan = 0;
        rekapKesalahan.Clear();
        AturFaseInstruksi();
    }
}