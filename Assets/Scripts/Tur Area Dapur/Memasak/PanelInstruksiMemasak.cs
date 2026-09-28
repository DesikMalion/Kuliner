using ITISKIRUHERE;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelInstruksiMemasak : MonoBehaviour
{
    [Header("Referensi UI")]
    public TextMeshProUGUI teksInstruksi;
    public Button tombolNavigasi;
    public GameObject[] daftarAlatMasak;
    private TextMeshProUGUI teksTombol;
    private bool diFaseSatu = true; // Status penanda kita sedang di Interaksi 1 atau 2

    private void Start()
    {
        // Mencari komponen teks di dalam tombol (Next/Previous)
        if (tombolNavigasi != null)
        {
            teksTombol = tombolNavigasi.GetComponentInChildren<TextMeshProUGUI>();
        }
        if (EvaluasiMemasak.Instance != null && daftarAlatMasak != null)
        {
            EvaluasiMemasak.Instance.totalAlat = daftarAlatMasak.Length;
        }
        // Jalankan Fase 1 saat panel pertama kali aktif
        SetFaseSatu();
    }

    // Fungsi ini dihubungkan ke Event OnClick() pada Tombol di Inspector
    public void TekanTombolNavigasi()
    {
        if (diFaseSatu)
        {
            SetFaseDua();
        }
        else
        {
            SetFaseSatu();
        }
    }

    private void SetFaseSatu()
    {
        diFaseSatu = true;
        teksInstruksi.text = "Pelajari peralatan di area ini. Arahkan laser Anda ke alat masak untuk membaca fungsinya.";

        if (teksTombol != null) teksTombol.text = "Next";

        // AKTIFKAN fitur Hover Info
        if (ToolInfoUIManager.Instance != null)
        {
            ToolInfoUIManager.Instance.isInteraksi1Aktif = true;
        }
        AturOutline(true, 10f);
    }

    private void SetFaseDua()
    {
        diFaseSatu = false;
        teksInstruksi.text = "Sekarang saatnya ujian! Arahkan laser ke alat dan klik pelatuk untuk menjawab kuis.";

        if (teksTombol != null) teksTombol.text = "Previous";

        // MATIKAN fitur Hover Info (Interaksi 1)
        if (ToolInfoUIManager.Instance != null)
        {
            ToolInfoUIManager.Instance.isInteraksi1Aktif = false;

            // Paksa panel info tertutup jika sedang terbuka saat tombol ditekan
            ToolInfoUIManager.Instance.ForceHide();
        }
        AturOutline(false, 0f);
    }
    private void AturOutline(bool statusPulse, float nilaiWidth)
    {
        foreach (GameObject alat in daftarAlatMasak)
        {
            if (alat != null)
            {
                // Mencari komponen Advanced Outline di dalam objek alat
                AdvancedOutline outline = alat.GetComponent<AdvancedOutline>();

                if (outline != null)
                {
                    /* 
                     * PENTING: Jika muncul error "does not contain a definition for...", 
                     * silakan sesuaikan besar/kecil huruf pada nama variabel di bawah ini 
                     * (misal: diubah menjadi PulseWidth atau pulse_width) 
                     * agar sama persis dengan script Advanced Outline asli Anda. 
                     */
                    outline.PulseWidth = statusPulse;
                    outline.OutlineWidth = nilaiWidth;
                }
            }
        }
    }
}