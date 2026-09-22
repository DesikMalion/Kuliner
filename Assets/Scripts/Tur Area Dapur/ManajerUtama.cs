using UnityEngine;

public class ManajerUtama : MonoBehaviour
{
    [Header("Referensi Manager Area")]
    public EvaluasiPersiapan managerPersiapan;
    // Tambahkan referensi manager area lain di sini nanti
    // public EvaluasiPenerimaan managerPenerimaan; 
    // public EvaluasiPenyimpanan managerPenyimpanan;

    // Dipanggil oleh Tombol "Mulai Area Persiapan"
    public void AktifkanAreaPersiapan()
    {
        // 1. Sapu bersih semua bahan dari area sebelumnya (Penerimaan / Penyimpanan)
        // Ini mencegah siswa membawa-bawa bahan dari ruang depan ke dapur
        BersihkanSemuaBahanSisa();

        // 2. Munculkan HANYA bahan-bahan yang dibutuhkan untuk tugas Persiapan
        if (managerPersiapan != null)
        {
            managerPersiapan.MulaiAreaPersiapan();
        }

        Debug.Log("Tugas Persiapan Dimulai: Bahan telah diletakkan di atas meja.");
    }

    // Fungsi pembersih global untuk menghapus bahan yang tertinggal
    private void BersihkanSemuaBahanSisa()
    {
        // Cari dan hancurkan semua bahan utuh yang masih ada di scene
        BahanPersiapan[] sisaBahanUtuh = FindObjectsOfType<BahanPersiapan>();
        foreach (var b in sisaBahanUtuh)
        {
            Destroy(b.gameObject);
        }

        // Cari dan hancurkan semua potongan bahan yang tercecer
        PotonganBahan[] sisaPotongan = FindObjectsOfType<PotonganBahan>();
        foreach (var p in sisaPotongan)
        {
            Destroy(p.gameObject);
        }

        // Jika ada script bahan khusus dari area penerimaan, hancurkan juga di sini
        // BahanPenerimaan[] sisaPenerimaan = FindObjectsOfType<BahanPenerimaan>();
        // foreach (var bp in sisaPenerimaan) Destroy(bp.gameObject);
    }
}