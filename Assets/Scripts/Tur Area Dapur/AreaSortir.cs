using UnityEngine;

public class AreaSortir : MonoBehaviour
{
    public bool iniAreaTerima;
    public EvaluasiPenerimaan managerEvaluasi;

    private void OnTriggerEnter(Collider other)
    {
        BahanData bahan = other.GetComponent<BahanData>();
        if (bahan != null)
        {
            // Beritahu bahan bahwa ia sedang berada di dalam area ini
            bahan.SetAreaSaatIni(this);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        BahanData bahan = other.GetComponent<BahanData>();
        if (bahan != null)
        {
            // Hapus data area jika bahan ditarik keluar lagi
            bahan.HapusAreaSaatIni(this);
        }
    }
    public void ProsesBahanMasuk(BahanData bahan)
    {
        if (managerEvaluasi != null)
        {
            managerEvaluasi.ProsesBahan(bahan, iniAreaTerima);
        }
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
