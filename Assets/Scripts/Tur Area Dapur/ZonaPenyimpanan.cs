using UnityEngine;

public class ZonaPenyimpanan : MonoBehaviour
{
    [Header("Pengaturan Zona")]
    public KategoriSimpan jenisZona; // Tentukan ini rak apa
    public EvaluasiPenyimpanan managerEvaluasi;

    private void OnTriggerEnter(Collider other)
    {
        BahanData bahan = other.GetComponent<BahanData>();
        if (bahan != null)
        {
            bahan.SetZonaSimpanSaatIni(this);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        BahanData bahan = other.GetComponent<BahanData>();
        if (bahan != null)
        {
            bahan.HapusZonaSimpanSaatIni(this);
        }
    }

    public void ProsesBahanMasuk(BahanData bahan)
    {
        if (managerEvaluasi != null)
        {
            // Kirim data bahan dan tipe zona ini ke manager untuk dinilai
            managerEvaluasi.ProsesPenyimpanan(bahan, jenisZona);
        }
    }
}