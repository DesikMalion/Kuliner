using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class AreaSequenceManager : MonoBehaviour
{
    public Button[] areaButton;
    [Header("Pengaturan Teks Instruksi")]
    public TextMeshProUGUI teksInstruksiUtama; // Teks bagian atas panel

    [Header("Grup Bahan Tiap Area")]
    [Tooltip("Masukkan parent GameObject bahan untuk tiap area. Urutannya harus sama persis dengan areaButton (0: Penerimaan, 1: Penyimpanan, dst).")]
    public GameObject[] grupBahanArea;

    public RectTransform panelUtama;
    public Transform[] titikMunculPanel;
    // Daftar nama area agar teks bisa menyesuaikan otomatis
    private string[] namaArea = new string[]
    {
        "Area Penerimaan",
        "Area Penyimpanan",
        "Area Persiapan",
        "Area Memasak",
        "Area Penyajian",
        "Area Pencucian"
    };
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        ResetButtons();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void ResetButtons()
    {
        for (int i = 0; i < areaButton.Length; i++)
        {
            areaButton[i].interactable = (i == 0);

            // Matikan semua bahan di awal permainan agar meja bersih
            if (i < grupBahanArea.Length && grupBahanArea[i] != null)
            {
                grupBahanArea[i].SetActive(false);
            }
        }

        UpdateTeksInstruksi(0);

        if (titikMunculPanel.Length > 0 && titikMunculPanel[0] != null)
        {
            panelUtama.transform.localPosition = titikMunculPanel[0].localPosition;
            panelUtama.transform.localEulerAngles = titikMunculPanel[0].localEulerAngles;
        }
    }
    public void UnlockNextArea(int nextIndex)
    {
        if (nextIndex < areaButton.Length)
        {
            if (nextIndex > 0)
            {
                areaButton[nextIndex - 1].interactable = false;
            }
            // Buka kunci tombol selanjutnya
            areaButton[nextIndex].interactable = true;

            // Perbarui teks instruksi agar sesuai dengan area yang baru terbuka
            UpdateTeksInstruksi(nextIndex);

            if (nextIndex < titikMunculPanel.Length && titikMunculPanel[nextIndex] != null)
            {
                panelUtama.transform.position = titikMunculPanel[nextIndex].position;
                panelUtama.transform.rotation = titikMunculPanel[nextIndex].rotation;
            }

            // Munculkan panel
            panelUtama.gameObject.SetActive(true);
        }
    }

    private void UpdateTeksInstruksi(int index)
    {
        if (teksInstruksiUtama != null && index < namaArea.Length)
        {
            // Format teks dinamis
            teksInstruksiUtama.text = $"Silakan klik tombol <b>{namaArea[index]}</b> di atas, lalu temukan area tersebut dengan melihat petunjuk yang ada di dinding.";
        }
    }

    public void PilihArea(int indexPilihan)
    {
        // 1. Matikan semua bahan dari area lain untuk mencegah sisa bahan menumpuk
        foreach (GameObject grup in grupBahanArea)
        {
            if (grup != null) grup.SetActive(false);
        }

        // 2. Aktifkan hanya grup bahan untuk area yang tombolnya baru saja diklik
        if (indexPilihan < grupBahanArea.Length && grupBahanArea[indexPilihan] != null)
        {
            grupBahanArea[indexPilihan].SetActive(true);
        }
    }
}
