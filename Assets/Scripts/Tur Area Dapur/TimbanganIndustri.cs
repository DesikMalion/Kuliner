using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class TimbanganIndustri : MonoBehaviour
{
    [Header("Referensi Layar Timbangan")]
    public TextMeshProUGUI teksLayarTimbangan;

    // List untuk menyimpan benda apa saja yang sedang berada di atas timbangan
    private List<Rigidbody> bendaDiTimbangan = new List<Rigidbody>();

    private void Start()
    {
        // Set angka 0 saat awal mulai
        UpdateLayar(0f);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Cek apakah benda tersebut memiliki komponen BahanData dan Rigidbody
        BahanData bahan = other.GetComponent<BahanData>();
        Rigidbody rb = other.GetComponent<Rigidbody>();

        // Jika benda valid dan belum ada di dalam list, tambahkan ke perhitungan
        if (bahan != null && rb != null)
        {
            if (!bendaDiTimbangan.Contains(rb))
            {
                bendaDiTimbangan.Add(rb);
                HitungTotalBerat();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();

        // Jika benda diangkat dari timbangan, hapus dari perhitungan
        if (rb != null && bendaDiTimbangan.Contains(rb))
        {
            bendaDiTimbangan.Remove(rb);
            HitungTotalBerat();
        }
    }

    private void HitungTotalBerat()
    {
        float totalBerat = 0f;

        // Jumlahkan massa semua benda yang ada di atas timbangan
        foreach (Rigidbody rb in bendaDiTimbangan)
        {
            if (rb != null)
            {
                totalBerat += rb.mass; // Mengambil nilai 'Mass' dari Rigidbody benda
            }
        }

        UpdateLayar(totalBerat);
    }

    private void UpdateLayar(float berat)
    {
        if (teksLayarTimbangan != null)
        {
            // Format "F2" akan memunculkan 2 angka desimal (Contoh: 1.50)
            teksLayarTimbangan.text = berat.ToString("F2");
        }
    }
}