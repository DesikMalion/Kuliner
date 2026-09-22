using UnityEngine;

public class SensorTalenan : MonoBehaviour
{
    [Header("Pengaturan Talenan")]
    public TipeTalenan jenisTalenanIni;

    [Header("Referensi Manager")]
    public EvaluasiPersiapan managerPersiapan;

    private void OnTriggerEnter(Collider other)
    {
        BahanPersiapan bahan = other.GetComponent<BahanPersiapan>();
        if (bahan == null) bahan = other.GetComponentInParent<BahanPersiapan>();

        if (bahan != null)
        {
            // Memberi tahu bahan bahwa dia sedang berada di atas talenan ini
            bahan.SetTalenan(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        BahanPersiapan bahan = other.GetComponent<BahanPersiapan>();
        if (bahan == null) bahan = other.GetComponentInParent<BahanPersiapan>();

        if (bahan != null)
        {
            // Menghapus data jika benda ditarik keluar dari talenan
            bahan.HapusTalenan(this);
            //bahan.sudahDievaluasi = false; // Reset agar bisa dinilai lagi jika ditaruh ulang
        }
    }

    // Fungsi ini sekarang dipanggil/diperintah oleh objek bahan SAAT DILEPAS
    public void ProsesEvaluasiBahan(BahanPersiapan bahan)
    {
        if (bahan.talenanYangBenar != jenisTalenanIni)
        {
            if (managerPersiapan != null)
            {
                managerPersiapan.CatatPelanggaranKontaminasi(bahan.namaBahan, jenisTalenanIni, bahan.talenanYangBenar);
            }
        }
        else
        {
            if (managerPersiapan != null)
            {
                managerPersiapan.CatatPenempatanBenar();
            }
        }
    }
}