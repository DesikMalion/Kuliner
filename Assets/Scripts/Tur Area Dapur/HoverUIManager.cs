using UnityEngine;
using TMPro;

public class HoverUIManager : MonoBehaviour
{
    [Header("Referensi UI")]
    public GameObject panelHover;
    public TextMeshProUGUI teksNamaBahan;
    public TextMeshProUGUI teksDeskripsi;

    [Header("Pengaturan Posisi Panel")]
    [Tooltip("Jarak panel dari titik pusat bahan (X, Y, Z). Y = 0.2 berarti naik 20cm.")]
    public Vector3 offsetPosisi = new Vector3(0f, 0.2f, 0f);

    public Transform kameraVR;

    private void Start()
    {
        if (panelHover != null)
        {
            panelHover.SetActive(false);
        }

        // Mencari kamera utama VR secara otomatis
        if (Camera.main != null)
        {
            kameraVR = Camera.main.transform;
        }
    }

    // Kini fungsi ini meminta tambahan data posisi (Transform) dari bahan yang di-hover
    public void TampilkanInfo(BahanData bahan, Transform posisiBahan)
    {
        if (bahan != null && panelHover != null)
        {
            teksNamaBahan.text = bahan.namaBahan;
            teksDeskripsi.text = bahan.deskripsiKondisi;

            // 1. Pindahkan posisi UI ke lokasi bahan ditambah offset tinggi
            panelHover.transform.position = posisiBahan.position + offsetPosisi;

            // 2. Putar UI agar selalu menghadap ke pemain (Camera VR)
            if (kameraVR != null)
            {
                //panelHover.transform.LookAt(panelHover.transform.position + kameraVR.rotation * Vector3.forward, kameraVR.rotation * Vector3.up);
                Vector3 arahKeKamera = panelHover.transform.position - kameraVR.position;
                arahKeKamera.y = 0; // Kunci sumbu Y agar panel tidak ikut menunduk/mendongak

                if (arahKeKamera != Vector3.zero)
                {
                    panelHover.transform.rotation = Quaternion.LookRotation(arahKeKamera);
                }
            }

            panelHover.SetActive(true);
        }
    }

    public void SembunyikanInfo()
    {
        if (panelHover != null)
        {
            panelHover.SetActive(false);
        }
    }
}