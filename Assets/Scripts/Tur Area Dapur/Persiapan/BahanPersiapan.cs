using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public enum TipeTalenan
{
    SayurBuah, //hijau
    Daging, //merah
    AyamUnggas, //kuning
    Seafood, //biru
    RotiMatang //putih
}

[RequireComponent(typeof(XRGrabInteractable))]
public class BahanPersiapan : MonoBehaviour
{
    [Header("Identitas Bahan")]
    public string namaBahan;
    public TipeTalenan talenanYangBenar;

    public Material materialBagianDalam;
    // Variabel VR
    private XRGrabInteractable grabInteractable;
    private SensorTalenan talenanSaatIni;
    [HideInInspector] public bool sudahDievaluasi = false;
    [HideInInspector] public bool sudahDievaluasiPisau = false;
    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
    }

    private void OnEnable()
    {
        if (grabInteractable != null)
        {
            // Mendengarkan kapan benda ini dilepas dari tangan VR
            grabInteractable.selectExited.AddListener(OnDilepas);
        }
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectExited.RemoveListener(OnDilepas);
        }
    }

    // Dipanggil oleh sensor saat benda masuk ke area talenan
    public void SetTalenan(SensorTalenan talenan) { talenanSaatIni = talenan; }

    // Dipanggil oleh sensor saat benda keluar dari area talenan
    public void HapusTalenan(SensorTalenan talenan) { if (talenanSaatIni == talenan) talenanSaatIni = null; }

    // Dieksekusi hanya saat tombol grip VR dilepas
    private void OnDilepas(SelectExitEventArgs args)
    {
        // Jika dilepas tepat di atas talenan dan belum dinilai
        if (talenanSaatIni != null && !sudahDievaluasi)
        {
            sudahDievaluasi = true;
            talenanSaatIni.ProsesEvaluasiBahan(this);
        }
    }
}