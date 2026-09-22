using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SensorWadah : MonoBehaviour
{
    [Header("Target Misi")]
    public string bahanYangDiterima = "Daging Segar";
    [Tooltip("Jumlah potongan tepat yang harus masuk ke wadah ini")]
    public int targetJumlahPotongan = 4; // <-- Dikembalikan

    [Header("Status Saat Ini")]
    [HideInInspector] public int potonganTerkumpul = 0; // Menghitung bahan yang BENAR saja

    [Header("Referensi Manager")]
    public EvaluasiPersiapan managerPersiapan;

    private List<GameObject> bendaDihitung = new List<GameObject>();
    private List<string> bahanSalahPernahMasuk = new List<string>();

    private void OnTriggerEnter(Collider other)
    {
        PotonganBahan potongan = other.GetComponent<PotonganBahan>();
        if (potongan == null) potongan = other.GetComponentInParent<PotonganBahan>();

        if (potongan != null)
        {
            potongan.SetWadah(this);
            XRGrabInteractable grab = potongan.GetComponent<XRGrabInteractable>();

            if (grab != null && !grab.isSelected)
            {
                ProsesEvaluasiPotongan(potongan);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // KUNCI ABSOLUT: Jika tugas sudah ditutup oleh Manager, 
        // abaikan semua pantulan atau pergeseran benda di dalam mangkuk.
        if (managerPersiapan != null && managerPersiapan.areaSelesai) return;

        PotonganBahan potongan = other.GetComponent<PotonganBahan>();
        if (potongan == null) potongan = other.GetComponentInParent<PotonganBahan>();

        if (potongan != null)
        {
            potongan.HapusWadah(this);

            //XRGrabInteractable grab = potongan.GetComponent<XRGrabInteractable>();
            //if (grab == null || !grab.isSelected)
            //{
            //    potongan.transform.SetParent(null);
            //}

            if (bendaDihitung.Contains(other.gameObject))
            {
                bendaDihitung.Remove(other.gameObject);

                // Jika benda yang keluar adalah bahan yang BENAR, kurangi jumlah porsinya
                if (potongan.namaBahanAsal == bahanYangDiterima && potonganTerkumpul > 0)
                {
                    potonganTerkumpul--;
                }

                // Lapor ke manager ada potongan keluar
                if (managerPersiapan != null)
                {
                    managerPersiapan.LaporPotonganKeluar();
                }
            }
        }
    }

    public void ProsesEvaluasiPotongan(PotonganBahan potongan)
    {
        if (managerPersiapan != null && managerPersiapan.areaSelesai) return;
        if (bendaDihitung.Contains(potongan.gameObject)) return;

        bendaDihitung.Add(potongan.gameObject);

        //potongan.transform.SetParent(this.transform);
        // EVALUASI BAHAN (Pisahkan antara yang Benar dan Salah)
        if (potongan.namaBahanAsal == bahanYangDiterima)
        {
            // Bahan BENAR: Tambah porsi terkumpul
            potonganTerkumpul++;
        }
        else
        {
            // Bahan SALAH: Catat pelanggaran kontaminasi
            if (!bahanSalahPernahMasuk.Contains(potongan.namaBahanAsal))
            {
                bahanSalahPernahMasuk.Add(potongan.namaBahanAsal);

                if (managerPersiapan != null)
                {
                    managerPersiapan.rincianPelanggaran.Add($"- Salah Wadah: {potongan.namaBahanAsal} dicampur ke dalam wadah khusus {bahanYangDiterima}.");
                    managerPersiapan.jumlahPelanggaranKontaminasi++;

                    if (managerPersiapan.audioSource != null && managerPersiapan.suaraErrorKontaminasi != null)
                        managerPersiapan.audioSource.PlayOneShot(managerPersiapan.suaraErrorKontaminasi);
                }
            }
        }

        // SELALU lapor ada benda fisik masuk ke Manager (agar totalSemuaPotongan tetap tercapai)
        if (managerPersiapan != null)
        {
            managerPersiapan.LaporPotonganMasuk();
        }
    }

    public void ResetWadah()
    {
        potonganTerkumpul = 0; // Reset penghitung porsi
        bendaDihitung.Clear();
        bahanSalahPernahMasuk.Clear();
    }
}   