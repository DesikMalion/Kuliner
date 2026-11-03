
using UnityEngine;

public class SimulationMenuController : MonoBehaviour
{
    [Header("Menu")]
    public GameObject menuPanel;
    public GameObject buttonKembali;

    [Header("Root Simulasi")]
    public GameObject simulasiIdentifikasi;
    public GameObject simulasiPenyimpanan;

    private void Start()
    {
        ShowMenu();
    }

    public void ShowMenu()
    {
        // Tampilkan menu utama
        if (menuPanel != null)
            menuPanel.SetActive(true);

        // Sembunyikan tombol kembali saat di menu
        if (buttonKembali != null)
            buttonKembali.SetActive(false);

        // Matikan kedua simulasi
        if (simulasiIdentifikasi != null)
            simulasiIdentifikasi.SetActive(false);

        if (simulasiPenyimpanan != null)
            simulasiPenyimpanan.SetActive(false);
    }

    public void OpenIdentifikasi()
    {
        // Tutup menu
        if (menuPanel != null)
            menuPanel.SetActive(false);

        // Aktifkan simulasi identifikasi saja
        if (simulasiIdentifikasi != null)
            simulasiIdentifikasi.SetActive(true);

        if (simulasiPenyimpanan != null)
            simulasiPenyimpanan.SetActive(false);

        if (buttonKembali != null)
            buttonKembali.SetActive(true);
    }

    public void OpenPenyimpanan()
    {
        // Tutup menu
        if (menuPanel != null)
            menuPanel.SetActive(false);

        // Aktifkan simulasi penyimpanan saja
        if (simulasiIdentifikasi != null)
            simulasiIdentifikasi.SetActive(false);

        if (simulasiPenyimpanan != null)
            simulasiPenyimpanan.SetActive(true);

        if (buttonKembali != null)
            buttonKembali.SetActive(true);
    }
}
