using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
// using ITISKIRUHERE; // Hapus tanda komentar (//) di awal baris ini jika sempat error merah sebelumnya

public class EvaluasiPenyajian : MonoBehaviour
{
    public static EvaluasiPenyajian Instance { get; private set; }

    [Header("Referensi Sistem")]
    public UIManagerPenyajian uiManager; // Evaluasi kini hanya butuh referensi UI Manager

    [Header("Audio Feedback")]
    public AudioSource audioSource;
    public AudioClip suaraBenar;
    public AudioClip suaraSalah;

    private int pesananSelesai = 0;
    private int totalKesalahan = 0;

    private List<string> pesananBelumDisajikan = new List<string>();
    private bool dataSudahDisalin = false;

    private void Awake()
    {
        Instance = this;
    }

    public void ProsesPlating(IdentitasMakanan makanan, PiringPenyajian piring, UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable interactableObj)
    {
        bool sengajaDitaruh = makanan.sedangDipegangTangan || (Time.time - makanan.waktuTerakhirDilepas < 1.0f);
        if (!sengajaDitaruh)
        {
            // Jika tidak sengaja tertabrak piring, kembalikan diam-diam TANPA mencatat error
            StartCoroutine(KembalikanSilently(interactableObj, makanan));
            return; // Hentikan fungsi di sini agar skor tidak berkurang
        }

        if (!dataSudahDisalin && uiManager != null)
        {
            pesananBelumDisajikan = new List<string>(uiManager.pesananAktif);
            dataSudahDisalin = true;
        }

        string namaMakanan = makanan.namaMenu;
        bool piringCocok = (namaMakanan == piring.menuSeharusnya);
        bool sedangDipesan = pesananBelumDisajikan.Contains(namaMakanan);

        if (piringCocok && sedangDipesan)
        {
            pesananBelumDisajikan.Remove(namaMakanan);
            pesananSelesai++;

            if (audioSource != null && suaraBenar != null) audioSource.PlayOneShot(suaraBenar);

            var grabObj = interactableObj as UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable;
            if (grabObj != null) grabObj.enabled = false;

            Rigidbody rbMakanan = makanan.GetComponent<Rigidbody>();
            if (rbMakanan != null) rbMakanan.isKinematic = true;
            Collider colMakanan = makanan.GetComponent<Collider>();
            if (colMakanan != null) colMakanan.enabled = false;
            makanan.transform.SetParent(piring.transform);

            if (pesananBelumDisajikan.Count == 0 && pesananSelesai > 0)
            {
                SelesaikanArea();
            }
        }
        else
        {
            totalKesalahan++;
            if (audioSource != null && suaraSalah != null) audioSource.PlayOneShot(suaraSalah);

            StartCoroutine(KembalikanMakananSalah(interactableObj, makanan));
        }
    }
    private IEnumerator KembalikanSilently(IXRSelectInteractable interactableObj, IdentitasMakanan makanan)
    {
        yield return new WaitForEndOfFrame();
        var grabObj = interactableObj as UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable;

        if (grabObj != null)
        {
            grabObj.enabled = false;

            makanan.transform.position = makanan.posisiAwal;
            makanan.transform.rotation = makanan.rotasiAwal;

            Rigidbody rb = makanan.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            yield return new WaitForSeconds(0.1f);
            grabObj.enabled = true;
        }
    }
    private IEnumerator KembalikanMakananSalah(UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable interactableObj, IdentitasMakanan makanan)
    {
        yield return new WaitForEndOfFrame();
        var grabObj = interactableObj as UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable;

        if (grabObj != null)
        {
            grabObj.enabled = false;
            makanan.transform.position = makanan.posisiAwal;
            makanan.transform.rotation = makanan.rotasiAwal;

            Rigidbody rb = makanan.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = Vector3.zero;
                rb.angularVelocity = Vector3.zero;
            }

            yield return new WaitForSeconds(0.1f);
            grabObj.enabled = true;
        }
    }

    private void SelesaikanArea()
    {
        // Lapor ke UI Manager bahwa tugas selesai, kirimkan skor kesalahan
        if (uiManager != null)
        {
            uiManager.MunculkanTombolLaporan(totalKesalahan);
        }
    }
    public void ResetData()
    {
        pesananSelesai = 0;
        totalKesalahan = 0;
        dataSudahDisalin = false;
        pesananBelumDisajikan.Clear();
    }
    public void ResetEvaluasi()
    {
        pesananSelesai = 0;
        totalKesalahan = 0;
        pesananBelumDisajikan.Clear();
        dataSudahDisalin = false;
    }
}