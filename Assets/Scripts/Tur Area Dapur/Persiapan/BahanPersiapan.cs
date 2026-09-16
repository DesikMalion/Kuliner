using UnityEngine;

public enum TipeTalenan
{
    Hijau_SayurBuah,
    Merah_DagingMentah,
    Kuning_AyamUnggas,
    Biru_Seafood,
    Putih_RotiMatang
}
public class BahanPersiapan : MonoBehaviour
{
    [Header("Identitas Bahan")]
    public string namaBahan;

    [Tooltip("Pilih talenan warna apa yang BOLEH digunakan untuk bahan ini")]
    public TipeTalenan talenanYangBenar;

    [HideInInspector] public bool sedangDiTalenan = false;
}
