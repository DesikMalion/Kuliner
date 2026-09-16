using UnityEngine;
using TMPro;

public class EvaluasiPersiapan : MonoBehaviour
{
    [Header("Statistik Evaluasi")]
    public int jumlahPelanggaranKontaminasi = 0;

    [Header("Audio Feedback")]
    public AudioSource audioSource;
    public AudioClip suaraErrorKontaminasi; // Suara buzzer jika salah talenan

    // Fungsi ini akan dipanggil oleh Sensor Talenan saat mendeteksi kesalahan
    public void CatatPelanggaranKontaminasi(string namaBahan, TipeTalenan talenanDipakai, TipeTalenan talenanSeharusnya)
    {
        jumlahPelanggaranKontaminasi++;

        Debug.LogWarning($"KONTAMINASI SILANG: {namaBahan} diletakkan di talenan {talenanDipakai}. Seharusnya di talenan {talenanSeharusnya}!");

        if (audioSource != null && suaraErrorKontaminasi != null)
        {
            audioSource.PlayOneShot(suaraErrorKontaminasi);
        }
    }
}