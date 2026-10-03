using System.Collections.Generic;
using TMPro;
using UnityEngine;

[System.Serializable]
public class DataMenuMakanan
{
    public string namaMenu;
    public GameObject objekMakanan3D;
}

public class UIManagerPenyajian : MonoBehaviour
{
    [Header("Referensi Hierarchy UI")]
    public GameObject teksInstruksi;
    public GameObject objekOrderan;
    public GameObject objekLaporan;
    public GameObject tombolFooter;

    public TextMeshProUGUI teksTombol;
    public TextMeshProUGUI teksStatistikLaporan;

    [Header("Referensi Orderan")]
    public TextMeshProUGUI[] teksNamaMenu;
    public TextMeshProUGUI[] teksJumlahMenu;

    [Header("Manajer Objek Makanan 3D")]
    public DataMenuMakanan[] daftarSemuaMakanan;

    [Header("Manajer Progres Area")]
    public AreaProgressManager progressManager;

    private string[] semuaMenu = { "Ikan Goreng", "Nasi Goreng", "Steak", "Pizza", "Waffle" };
    [HideInInspector] public List<string> pesananAktif = new List<string>();

    private int faseAktif = 0;

    private void Start()
    {
        if (teksInstruksi != null) teksInstruksi.SetActive(true);
        if (objekOrderan != null) objekOrderan.SetActive(false);
        if (objekLaporan != null) objekLaporan.SetActive(false);

        if (tombolFooter != null) tombolFooter.SetActive(true);
        if (teksTombol != null) teksTombol.text = "Lanjutkan";

        faseAktif = 0;

        foreach (var menu in daftarSemuaMakanan)
        {
            if (menu.objekMakanan3D != null) menu.objekMakanan3D.SetActive(false);
        }
    }

    public void KlikTombolUtama()
    {
        if (faseAktif == 0)
        {
            if (teksInstruksi != null) teksInstruksi.SetActive(false);
            AcakMenuPesanan();
            if (objekOrderan != null) objekOrderan.SetActive(true);
            if (tombolFooter != null) tombolFooter.SetActive(false);
            faseAktif = 1;
        }
        else if (faseAktif == 1)
        {
            if (objekOrderan != null) objekOrderan.SetActive(false);
            if (objekLaporan != null) objekLaporan.SetActive(true);
            if (teksTombol != null) teksTombol.text = "Selesaikan Dapur";
            faseAktif = 2;
        }
        else if (faseAktif == 2)
        {
            gameObject.SetActive(false);
            if (progressManager != null) progressManager.TambahTugasSelesai();
        }
    }

    private void AcakMenuPesanan()
    {
        pesananAktif.Clear();
        List<string> menuTersedia = new List<string>(semuaMenu);

        // Maksimal 4, Minimal 2 (Sesuai yang sudah kita atur)
        int maksimalBisaDitampilkan = Mathf.Min(4, teksNamaMenu.Length);
        int targetJumlahOrder = Random.Range(2, maksimalBisaDitampilkan + 1);

        for (int i = 0; i < targetJumlahOrder; i++)
        {
            int randomIndex = Random.Range(0, menuTersedia.Count);
            string menuTerpilih = menuTersedia[randomIndex];

            pesananAktif.Add(menuTerpilih);
            menuTersedia.RemoveAt(randomIndex);

            foreach (var dataMenu in daftarSemuaMakanan)
            {
                if (dataMenu.namaMenu == menuTerpilih && dataMenu.objekMakanan3D != null)
                {
                    dataMenu.objekMakanan3D.SetActive(true);
                }
            }
        }

        for (int i = 0; i < teksNamaMenu.Length; i++)
        {
            if (i < pesananAktif.Count)
            {
                teksNamaMenu[i].gameObject.SetActive(true);
                teksJumlahMenu[i].gameObject.SetActive(true);
                teksNamaMenu[i].text = pesananAktif[i];
                teksJumlahMenu[i].text = "1";
            }
            else
            {
                teksNamaMenu[i].gameObject.SetActive(false);
                teksJumlahMenu[i].gameObject.SetActive(false);
            }
        }
    }

    public void MunculkanTombolLaporan(int totalKesalahan)
    {
        if (tombolFooter != null) tombolFooter.SetActive(true);
        if (teksTombol != null) teksTombol.text = "Lihat Laporan";

        if (teksStatistikLaporan != null)
        {
            teksStatistikLaporan.text = $"<b>Area Penyajian Selesai!</b>\nSemua orderan telah ditata dengan tepat.\n\nTotal Kesalahan Penataan: {totalKesalahan}x";
        }
    }

    // Fungsi tunggal yang menggabungkan semua perbaikan fisika VR
    public void ResetAreaPenyajian()
    {
        // 1. Kembalikan Piring (Paksa lepas dari genggaman agar tidak terpental)
        PiringPenyajian[] semuaPiring = FindObjectsByType<PiringPenyajian>(FindObjectsSortMode.None);
        foreach (var piring in semuaPiring)
        {
            var grabPiring = piring.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
            if (grabPiring != null) grabPiring.enabled = false;

            Rigidbody rbPiring = piring.GetComponent<Rigidbody>();
            if (rbPiring != null)
            {
                rbPiring.linearVelocity = Vector3.zero;
                rbPiring.angularVelocity = Vector3.zero;
            }

            piring.transform.position = piring.posisiAwal;
            piring.transform.rotation = piring.rotasiAwal;

            if (grabPiring != null) grabPiring.enabled = true;
        }

        // 2. Kembalikan Makanan dengan sistem Anti-Tembus Meja (+ 5cm)
        foreach (var menu in daftarSemuaMakanan)
        {
            if (menu.objekMakanan3D != null)
            {
                var grabMakanan = menu.objekMakanan3D.GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
                if (grabMakanan != null) grabMakanan.enabled = false;

                IdentitasMakanan idMakanan = menu.objekMakanan3D.GetComponent<IdentitasMakanan>();
                if (idMakanan != null)
                {
                    menu.objekMakanan3D.transform.SetParent(idMakanan.parentAwal);

                    // Nyalakan Collider duluan sebagai bemper
                    Collider colMakanan = menu.objekMakanan3D.GetComponent<Collider>();
                    if (colMakanan != null) colMakanan.enabled = true;

                    // Beri offset +0.05f di sumbu Y agar sedikit melayang di atas meja saat muncul
                    Vector3 posisiAman = idMakanan.posisiAwal + new Vector3(0f, 0.05f, 0f);
                    menu.objekMakanan3D.transform.position = posisiAman;
                    menu.objekMakanan3D.transform.rotation = idMakanan.rotasiAwal;
                }

                // Nyalakan gravitasi terakhir setelah posisi aman
                Rigidbody rbMakanan = menu.objekMakanan3D.GetComponent<Rigidbody>();
                if (rbMakanan != null)
                {
                    rbMakanan.linearVelocity = Vector3.zero;
                    rbMakanan.angularVelocity = Vector3.zero;
                    rbMakanan.isKinematic = false;
                }

                if (grabMakanan != null) grabMakanan.enabled = true;

                // Sembunyikan objeknya (akan dimunculkan lagi saat tombol dilanjutkan diklik)
                menu.objekMakanan3D.SetActive(false);
            }
        }

        // 3. Reset UI kembali ke Instruksi (Fase 0)
        faseAktif = 0;
        if (teksInstruksi != null) teksInstruksi.SetActive(true);
        if (objekOrderan != null) objekOrderan.SetActive(false);
        if (objekLaporan != null) objekLaporan.SetActive(false);
        if (tombolFooter != null) tombolFooter.SetActive(true);
        if (teksTombol != null) teksTombol.text = "Lanjutkan";

        // 4. Hapus memori nilai evaluasi
        if (EvaluasiPenyajian.Instance != null)
        {
            // Pastikan di skrip EvaluasiPenyajian.cs nama fungsinya adalah ResetEvaluasi()
            EvaluasiPenyajian.Instance.ResetEvaluasi();
        }
    }
}