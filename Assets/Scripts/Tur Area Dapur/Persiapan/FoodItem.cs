using UnityEngine;

// Kategori bahan makanan, dipakai untuk validasi talenan & cross-contamination nanti.
public enum FoodCategory
{
    Daging,
    Unggas,
    Seafood,
    Sayur,
    SiapSantap
}

// Nempel di setiap prefab bahan (wortel, daging, dsb).
// Komponen ini murni menyimpan DATA & STATUS bahan, tidak mendeteksi collision sendiri.
// Deteksi collision/trigger dilakukan oleh komponen lain (CuttingBoard, Sink, dsb)
// yang lalu membaca/mengubah status di sini.
public class FoodItem : MonoBehaviour
{
    [Header("Identitas Bahan")]
    public FoodCategory category;

    [Header("Status (read-only saat runtime, diubah oleh komponen lain)")]
    public bool isWashed = false;
    public bool isSliced = false;

    // Dipanggil oleh komponen Sink saat bahan selesai dicuci.
    public void MarkAsWashed()
    {
        if (isWashed) return; // hindari trigger event dobel kalau sudah dicuci

        isWashed = true;
        Debug.Log($"[FoodItem] {name} ({category}) sudah dicuci.");
    }

    // Dipanggil oleh SliceObject saat bahan ini berhasil dipotong.
    public void MarkAsSliced()
    {
        isSliced = true;
    }
}