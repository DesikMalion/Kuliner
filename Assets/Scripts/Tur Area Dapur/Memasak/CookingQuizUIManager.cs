using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class CookingQuizUIManager : MonoBehaviour
{
    public static CookingQuizUIManager Instance { get; private set; }

    [Header("Panel Kuis UI")]
    [SerializeField] private GameObject quizPanel;
    [SerializeField] private TextMeshProUGUI teksPertanyaan;
    [SerializeField] private TextMeshProUGUI teksFeedback;

    [Header("Tombol Pilihan (Isi dengan 5 Tombol)")]
    [SerializeField] private Button[] tombolPilihan;

    [Header("Pengaturan Billboard")]
    [SerializeField] private bool facePlayer = true;
    [SerializeField] private Transform playerCamera;

    private Transform currentTarget;
    private Vector3 currentOffset;
    private CookingToolHoverTarget alatAktif;
    private string jawabanBenarSaatIni;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        if (quizPanel != null) quizPanel.SetActive(false);
        if (playerCamera == null && Camera.main != null) playerCamera = Camera.main.transform;
    }

    private void LateUpdate()
    {
        if (quizPanel == null || !quizPanel.activeSelf || currentTarget == null) return;

        quizPanel.transform.position = currentTarget.TransformPoint(currentOffset);

        if (facePlayer && playerCamera != null)
        {
            Vector3 direction = quizPanel.transform.position - playerCamera.position;
            if (direction.sqrMagnitude > 0.001f)
            {
                quizPanel.transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }

    public void TampilkanKuis(CookingToolHoverTarget alat, Transform target, string pertanyaan, string jawabanBenar, string[] pilihanSalah, Vector3 panelOffset)
    {
        // --- BARIS PENGAMAN BARU ---
        // Jika panel sudah tampil dan yang diklik adalah alat yang sama, JANGAN diacak lagi!
        if (quizPanel.activeSelf && alatAktif == alat) return;

        alatAktif = alat;
        currentTarget = target;
        currentOffset = panelOffset;
        jawabanBenarSaatIni = jawabanBenar;

        teksPertanyaan.text = pertanyaan;
        teksFeedback.text = "";

        List<string> semuaPilihan = new List<string>(pilihanSalah);
        semuaPilihan.Add(jawabanBenar);

        for (int i = 0; i < semuaPilihan.Count; i++)
        {
            string temp = semuaPilihan[i];
            int randomIndex = Random.Range(i, semuaPilihan.Count);
            semuaPilihan[i] = semuaPilihan[randomIndex];
            semuaPilihan[randomIndex] = temp;
        }

        string[] prefix = { "A. ", "B. ", "C. ", "D. ", "E. " };

        for (int i = 0; i < tombolPilihan.Length; i++)
        {
            if (i < semuaPilihan.Count)
            {
                tombolPilihan[i].gameObject.SetActive(true);
                TextMeshProUGUI teksTombol = tombolPilihan[i].GetComponentInChildren<TextMeshProUGUI>();

                if (teksTombol != null)
                {
                    teksTombol.text = prefix[i] + semuaPilihan[i];
                }

                tombolPilihan[i].onClick.RemoveAllListeners();
                string jawabanDipilih = semuaPilihan[i];
                tombolPilihan[i].onClick.AddListener(() => CekJawaban(jawabanDipilih));
            }
            else
            {
                tombolPilihan[i].gameObject.SetActive(false);
            }
        }

        quizPanel.SetActive(true);
    }

    private void CekJawaban(string jawabanDipilih)
    {
        if (jawabanDipilih == jawabanBenarSaatIni)
        {
            // Teks menjadi Hijau
            teksFeedback.text = "<color=#00FF00>Jawaban anda benar</color>";
            if (alatAktif != null) alatAktif.TandaiSelesai();

            // Tutup kuis otomatis setelah 2 detik
            Invoke("SembunyikanKuis", 2f);
        }
        else
        {
            // Teks menjadi Merah
            teksFeedback.text = "<color=#FF0000>Jawaban salah, silakan coba lagi</color>";
        }
    }

    public void SembunyikanKuis()
    {
        currentTarget = null;
        alatAktif = null;
        if (quizPanel != null) quizPanel.SetActive(false);
    }
}