
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BackToMainMenu : MonoBehaviour
{
    [Header("Posisi Kembali")]
    [SerializeField] private Transform objekYangDipindahkan;
    [SerializeField] private Transform posisiTujuan;

    [Header("Menu dan Simulasi")]
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject gameSimulasi1;
    [SerializeField] private GameObject gameSimulasi2;

    [Header("Transisi")]
    [SerializeField] private CanvasGroup layarHitam;
    [SerializeField] private float durasiFade = 0.5f;

    private bool sedangTransisi;

    public void Kembali()
    {
        if (sedangTransisi)
            return;

        sedangTransisi = true;
        StartCoroutine(ProsesKembali());
    }

    private IEnumerator ProsesKembali()
    {
        // Fade in layar hitam
        if (layarHitam != null)
            yield return Fade(0f, 1f);

        // Kembalikan posisi objek
        if (objekYangDipindahkan != null && posisiTujuan != null)
        {
            objekYangDipindahkan.SetPositionAndRotation(
                posisiTujuan.position,
                posisiTujuan.rotation
            );
        }

        // Aktifkan Main Menu
        if (mainMenu != null)
            mainMenu.SetActive(true);

        // Nonaktifkan simulasi
        if (gameSimulasi1 != null)
            gameSimulasi1.SetActive(false);

        if (gameSimulasi2 != null)
            gameSimulasi2.SetActive(false);

        Time.timeScale = 1f;

        // Fade out layar hitam
        if (layarHitam != null)
            yield return Fade(1f, 0f);

        sedangTransisi = false;
    }

    private IEnumerator Fade(float dari, float ke)
    {
        float waktu = 0f;
        float durasi = Mathf.Max(0f, durasiFade);

        layarHitam.gameObject.SetActive(true);
        layarHitam.alpha = dari;

        while (waktu < durasi)
        {
            waktu += Time.unscaledDeltaTime;

            layarHitam.alpha = Mathf.Lerp(
                dari,
                ke,
                waktu / durasi
            );

            yield return null;
        }

        layarHitam.alpha = ke;
    }
}
