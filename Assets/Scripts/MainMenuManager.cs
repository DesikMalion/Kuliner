using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] GameObject home, modul;
    [SerializeField] GameObject subModul1, subModul2, subModul3;

    private GameObject currentPanel;
    [SerializeField] GameObject btnKembali; // tombol kembali, hanya aktif kalau lagi di submodul

    private static MainMenuManager instance;

    [Header("Pengaturan Posisi Menu (VR)")]
    [SerializeField] Transform menuCanvas;//drag Canvas (World Space) yang jadi parent semua panel
    [SerializeField] float jarakDariKamera = 1.5f;
    [SerializeField] float tinggiOffset = 0f;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            // sudah ada instance lain (misal karena scene ini di-load ulang), hancurkan yang baru
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // tiap kali scene baru selesai load, XR Origin/kamera ada di posisi berbeda,
        // jadi menu perlu diposisikan ulang biar tetap kelihatan di depan pemain
        PosisikanMenuDiDepanKamera();
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TampilkanPanel(home);
        PosisikanMenuDiDepanKamera();
    }
    
    // Update is called once per frame
    void Update()
    {
        //adjust jarak dan tinggi panel
        //PosisikanMenuDiDepanKamera();
    }
    public void BukaHome()
    {
        TampilkanPanel(home);
    }
    public void BukaModul()
    {
        TampilkanPanel(modul);
    }
    public void BukaSubModul1()
    {
        TampilkanPanel(subModul1);
    }

    public void BukaSubModul2()
    {
        TampilkanPanel(subModul2);
    }

    public void BukaSubModul3()
    {
        TampilkanPanel(subModul3);
    }
    public void Kembali()
    {
        //jika berada di submodul1/2/3 -> nonaktifkan submodul, tampilkan modul
        if (currentPanel == subModul1 || currentPanel == subModul2 || currentPanel == subModul3)
        {
            TampilkanPanel(modul);
        }
        //jika berada di modul -> nonaktifkan modul, tampilkan home
        else if (currentPanel == modul)
        {
            TampilkanPanel(home);
        }
        //fallback, kalau sudah di home / belum terdeteksi, tetap ke home
        else
        {
            TampilkanPanel(home);
        }
    }
    private void PosisikanMenuDiDepanKamera()
    {
        if (menuCanvas == null) return;

        Camera kameraAktif = Camera.main; // pastikan kamera di XR Origin setiap scene bertag "MainCamera"
        if (kameraAktif == null) return;

        Transform cam = kameraAktif.transform;

        Vector3 posisiBaru = cam.position + cam.forward * jarakDariKamera + Vector3.up * tinggiOffset;
        menuCanvas.position = posisiBaru;

        // hadapkan menu ke kamera (billboard), tanpa ikut miring naik/turun
        Vector3 arahKeKamera = cam.position - menuCanvas.position;
        arahKeKamera.y = 0f;
        if (arahKeKamera.sqrMagnitude > 0.001f)
        {
            menuCanvas.rotation = Quaternion.LookRotation(-arahKeKamera);
        }
    }
    public void PindahScene(string namaScene)
    {
        SceneManager.LoadScene(namaScene);
    }
    private void TampilkanPanel(GameObject panelTujuan)
    {
        if (home != null) home.SetActive(false);
        if (modul != null) modul.SetActive(false);
        if (subModul1 != null) subModul1.SetActive(false);
        if (subModul2 != null) subModul2.SetActive(false);
        if (subModul3 != null) subModul3.SetActive(false);

        if (panelTujuan != null)
        {
            panelTujuan.SetActive(true);
        }

        currentPanel = panelTujuan;

        // tombol kembali cuma aktif kalau lagi ada di salah satu subModul
        bool sedangDiSubModul = (panelTujuan == subModul1 || panelTujuan == subModul2 || panelTujuan == subModul3);
        if (btnKembali != null)
            btnKembali.SetActive(sedangDiSubModul);
    }
}
