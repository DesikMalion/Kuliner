using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class PiringPenyajian : MonoBehaviour
{
    [Header("Pengaturan Piring")]
    public string menuSeharusnya;

    [HideInInspector] public Vector3 posisiAwal;
    [HideInInspector] public Quaternion rotasiAwal;

    private void Start()
    {
        posisiAwal = transform.position;
        rotasiAwal = transform.rotation;
    }

    public void CekMakananMasuk(SelectEnterEventArgs args)
    {
        GameObject makanan = args.interactableObject.transform.gameObject;
        IdentitasMakanan identitas = makanan.GetComponent<IdentitasMakanan>();

        if (identitas != null && EvaluasiPenyajian.Instance != null)
        {
            EvaluasiPenyajian.Instance.ProsesPlating(identitas, this, args.interactableObject);
        }
    }
}