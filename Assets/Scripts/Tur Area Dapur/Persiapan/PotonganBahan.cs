using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PotonganBahan : MonoBehaviour
{
    public string namaBahanAsal;
    [HideInInspector] public Material materialBagianDalamAsal;

    private XRGrabInteractable grabInteractable;
    private SensorWadah wadahSaatIni;

    private void Start()
    {
        // Mengambil komponen Grab yang dipasang otomatis oleh EzySlice
        grabInteractable = GetComponent<XRGrabInteractable>();
        if (grabInteractable != null)
        {
            grabInteractable.selectExited.AddListener(OnDilepas);
        }
    }

    private void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectExited.RemoveListener(OnDilepas);
        }
    }

    // Dipanggil saat potongan bersentuhan dengan sensor wadah
    public void SetWadah(SensorWadah wadah) { wadahSaatIni = wadah; }

    // Dipanggil saat potongan keluar dari sensor wadah
    public void HapusWadah(SensorWadah wadah) { if (wadahSaatIni == wadah) wadahSaatIni = null; }

    // Dieksekusi otomatis hanya saat genggaman VR dilepas (Drop)
    private void OnDilepas(SelectExitEventArgs args)
    {
        if (wadahSaatIni != null)
        {
            wadahSaatIni.ProsesEvaluasiPotongan(this);
        }
    }
}