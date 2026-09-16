using UnityEngine;

public class SensorTalenan : MonoBehaviour
{
    [Header("Pengaturan Talenan")]
    [Tooltip("Tentukan jenis talenan ini")]
    public TipeTalenan jenisTalenanIni;

    [Header("Referensi Manager")]
    public EvaluasiPersiapan managerPersiapan;

    private void OnTriggerEnter(Collider other)
    {
        // Cek apakah benda yang menyentuh talenan adalah Bahan Persiapan
        BahanPersiapan bahan = other.GetComponent<BahanPersiapan>();

        // Jika yang menyentuh adalah parent, coba cari di komponennya
        if (bahan == null) bahan = other.GetComponentInParent<BahanPersiapan>();

        if (bahan != null && !bahan.sedangDiTalenan)
        {
            bahan.sedangDiTalenan = true;

            // Evaluasi Kontaminasi Silang
            if (bahan.talenanYangBenar != jenisTalenanIni)
            {
                // Lapor ke manager bahwa terjadi pelanggaran
                if (managerPersiapan != null)
                {
                    managerPersiapan.CatatPelanggaranKontaminasi(bahan.namaBahan, jenisTalenanIni, bahan.talenanYangBenar);
                }
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        BahanPersiapan bahan = other.GetComponent<BahanPersiapan>();
        if (bahan == null) bahan = other.GetComponentInParent<BahanPersiapan>();

        if (bahan != null)
        {
            bahan.sedangDiTalenan = false;
        }
    }
}