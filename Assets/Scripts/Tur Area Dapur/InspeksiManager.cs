using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class InspeksiManager : MonoBehaviour
{
    public GameObject panelPopUp;
    public TextMeshProUGUI teksKondisi;
    public AreaProgressManager progressManager;
    public XRSocketInteractor socketInspeksi;
    private BahanData bahanSaatIni;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    public void TampilkanPopUpInspeksi()
    {
        IXRSelectInteractable objekDiSocket = socketInspeksi.firstInteractableSelected;
        if (objekDiSocket != null) 
        {
            bahanSaatIni = objekDiSocket.transform.GetComponent<BahanData>();
            if (bahanSaatIni != null)
            {
                teksKondisi.text = bahanSaatIni.deskripsiKondisi;
                panelPopUp.SetActive(true);
            }
        }
    }
    public void KlikTerima()
    {
        if (bahanSaatIni.isLayakDiterima)
            InspeksiBerhasil();
        else
            Debug.Log("tambahkan feedback error seperti suara");
    }
    public void KlikTolak()
    {
        if (!bahanSaatIni.isLayakDiterima)
            InspeksiBerhasil();
        else
            Debug.Log("Jawaban Salah. Bahan ini masih bagus dan layak diterima");
    }
    void InspeksiBerhasil()
    {
        panelPopUp.SetActive(false);
        progressManager.TambahTugasSelesai();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
