using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class PencucianManager : MonoBehaviour
{
    public GameObject[] ShapePiring;
    public GameObject[] SocketPiring;
    public GameObject[] OtherSocket;
    public GameObject[] OtherInteraction;

    public FireExtinguisherSpray fireExtinguisherSpray;
    GameObject PiringSpray = null;
    ObjectPiringPencucian objectPiringPencucian;

    public AreaProgressManager progressManager;

    int snappedObj = 0;

    List<Vector3> posisiAwalObj = new List<Vector3>();
    List<Vector3> rotasiAwalObj = new List<Vector3>();

    [Header("Referensi UI Panel (Satu Pintu)")]
    public GameObject panelVisualUtama; // <-- Tambahan untuk integrasi sistem Satu Pintu
    public GameObject objekInstruksi;
    public GameObject objekLaporan;
    public GameObject tombolLanjutkan;
    public GameObject tombolReset;

    [Header("Teks Laporan")]
    public TextMeshProUGUI teksStatistik;
    public TextMeshProUGUI teksRemark;

    [Header("Audio Feedback")]
    public AudioSource audioSource;
    public AudioClip suaraBenar;
    public AudioClip suaraSalah;

    void Start()
    {
        // 1. Amankan layar dengan memaksa UI masuk ke Fase Instruksi
        AturFaseInstruksi();

        GetAllObjPos();
        StartPencucian();
    }

    private void AturFaseInstruksi()
    {
        if (panelVisualUtama != null) panelVisualUtama.SetActive(true);
        if (objekInstruksi != null) objekInstruksi.SetActive(true);
        if (objekLaporan != null) objekLaporan.SetActive(false);
        if (tombolLanjutkan != null) tombolLanjutkan.SetActive(false);
        if (tombolReset != null) tombolReset.SetActive(false);
    }

    void GetAllObjPos()
    {
        for (int i = 0; i < ShapePiring.Length; i++)
        {
            posisiAwalObj.Add(ShapePiring[i].transform.position);
            rotasiAwalObj.Add(ShapePiring[i].transform.eulerAngles);
        }
        posisiAwalObj.Add(OtherInteraction[3].transform.position);
        rotasiAwalObj.Add(OtherInteraction[3].transform.eulerAngles);
    }

    void StartPencucian()
    {
        // PERBAIKAN FISIKA: Biarkan semua piring aktif fisiknya (Collider ON, Kinematic OFF) sejak awal
        for (int i = 0; i < ShapePiring.Length; i++)
        {
            ObjSetColliderKinematic(ShapePiring[i], true, false);
        }

        for (int i = 0; i < SocketPiring.Length; i++)
        {
            SocketPiring[i].SetActive(false);
        }
        for (int i = 0; i < OtherSocket.Length; i++)
        {
            OtherSocket[i].SetActive(false);
        }
        for (int i = 0; i < OtherInteraction.Length; i++)
        {
            OtherInteraction[i].SetActive(false);
        }

        // Aktifkan Socket pertama dan interaksi lainnya
        ObjSetColliderKinematic(OtherInteraction[3], true, false);
        SocketPiring[0].SetActive(true);
        OtherSocket[0].SetActive(true);
        OtherInteraction[0].SetActive(true);
        OtherInteraction[3].SetActive(true);
    }

    public void ObjSetColliderKinematic(GameObject obj, bool ColEnable, bool KinematicEnable)
    {
        Collider[] Colliders = obj.transform.GetComponentsInChildren<Collider>(true);
        foreach (Collider Collider in Colliders)
        {
            Collider.enabled = ColEnable;
        }

        BoxCollider boxCollider = obj.GetComponent<BoxCollider>();
        if (boxCollider != null)
            boxCollider.enabled = ColEnable;

        Rigidbody rb = obj.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = obj.GetComponentInChildren<Rigidbody>(true);
        }
        if (rb != null)
            rb.isKinematic = KinematicEnable;
    }

    public void PiringSnapped(GameObject objPiring)
    {
        for (int i = 0; i < SocketPiring.Length; i++)
        {
            if (SocketPiring[i].name == objPiring.name)
            {
                SocketPiring[i].GetComponent<XRSocketInteractor>().attachTransform.gameObject.SetActive(false);
                SocketPiring[i].GetComponent<SocketLockObject>().ObjSnapped.GetComponent<ObjectPiringPencucian>().isSnapped = true;

                // Kunci posisi piring yang sudah menempel di rak (Kinematic ON)
                ObjSetColliderKinematic(ShapePiring[i], true, true);
                snappedObj++;

                // Aktifkan socket piring selanjutnya
                if (snappedObj < ShapePiring.Length)
                {
                    SocketPiring[snappedObj].SetActive(true);
                }
            }
        }
    }

    public void PiringSprayDetected()
    {
        GameObject objPiring = fireExtinguisherSpray.GetSprayObject();

        if (objPiring == null) return;

        if (PiringSpray != objPiring)
        {
            PiringSpray = objPiring;
            objectPiringPencucian = PiringSpray.GetComponent<ObjectPiringPencucian>();
        }

        objectPiringPencucian.CuciPiring();
    }

    public void RakPiringSnapped()
    {
        OtherInteraction[1].SetActive(true);
    }

    bool isMesinCuciOpen = false;
    public void GrabMesinCuci()
    {
        OtherInteraction[1].SetActive(false);

        if (!isMesinCuciOpen)
        {
            OtherInteraction[0].GetComponent<Animator>().Play("MesinCuciTutup");
            isMesinCuciOpen = true;
            StartCoroutine(WaitMesinCuciFinish());
        }
        else
        {
            OtherInteraction[0].GetComponent<Animator>().Play("MesinCuciBuka");
            isMesinCuciOpen = false;
        }
    }

    IEnumerator WaitMesinCuciFinish()
    {
        yield return new WaitForSeconds(5f);
        OtherInteraction[1].SetActive(true);
        OtherInteraction[2].SetActive(true);

        ObjSetColliderKinematic(OtherInteraction[3], true, false);
        // Hapus OtherInteraction[3].isStatic karena isStatic tidak bisa dimodifikasi di script saat runtime
        OtherSocket[0].SetActive(false);
        OtherSocket[1].SetActive(true);

        for (int i = 0; i < ShapePiring.Length; i++)
        {
            ObjectPiringPencucian objectPiringPencucian = ShapePiring[i].GetComponent<ObjectPiringPencucian>();

            for (int j = 0; j < objectPiringPencucian.KotoranPiring.Length; j++)
            {
                objectPiringPencucian.KotoranPiring[j].SetActive(false);
            }

            if (!objectPiringPencucian.isBersih && objectPiringPencucian.isSnapped)
            {
                objectPiringPencucian.KotoranPiring[2].SetActive(true);
            }
        }
    }

    public void LaporanPencucian()
    {
        if (objekInstruksi != null) objekInstruksi.SetActive(false);
        if (objekLaporan != null) objekLaporan.SetActive(true);
        if (tombolLanjutkan != null) tombolLanjutkan.SetActive(true);
        if (tombolReset != null) tombolReset.SetActive(true);

        int piringBersih = 0;
        for (int i = 0; i < ShapePiring.Length; i++)
        {
            ObjectPiringPencucian objectPiringPencucian = ShapePiring[i].GetComponent<ObjectPiringPencucian>();
            if (objectPiringPencucian.isBersih)
            {
                piringBersih++;
            }
        }

        teksStatistik.text = "Jumlah Piring Bersih: " + piringBersih + "/" + ShapePiring.Length;

        if (piringBersih == ShapePiring.Length)
        {
            teksRemark.text = "Selamat! Semua piring telah dicuci dengan bersih.";
            audioSource.PlayOneShot(suaraBenar);
        }
        else
        {
            teksRemark.text = "Beberapa piring masih kotor. Silakan cuci semua piring.";
            audioSource.PlayOneShot(suaraSalah);
        }
    }

    public void KlikLanjutkan()
    {
        // Matikan seluruh panel visual layaknya area lain
        if (panelVisualUtama != null) panelVisualUtama.SetActive(false);

        if (progressManager != null)
        {
            progressManager.TambahTugasSelesai();
        }
    }

    public void KlikReset()
    {
        ResetAllObjPos();

        for (int i = 0; i < ShapePiring.Length; i++)
        {
            ObjectPiringPencucian objectPiringPencucian = ShapePiring[i].GetComponent<ObjectPiringPencucian>();
            objectPiringPencucian.ResetPiring();
        }

        for (int i = 0; i < OtherSocket.Length; i++)
        {
            OtherSocket[i].GetComponent<XRSocketInteractor>().attachTransform.gameObject.SetActive(true);
        }

        for (int i = 0; i < SocketPiring.Length; i++)
        {
            SocketPiring[i].GetComponent<XRSocketInteractor>().attachTransform.gameObject.SetActive(true);
        }

        PiringSpray = null;
        snappedObj = 0;

        // Kembalikan UI ke fase instruksi
        AturFaseInstruksi();
        StartPencucian();
    }

    void ResetAllObjPos()
    {
        int index = 0;
        for (int i = 0; i < ShapePiring.Length; i++)
        {
            ShapePiring[i].transform.position = posisiAwalObj[index];
            ShapePiring[i].transform.eulerAngles = rotasiAwalObj[index];

            // Nyalakan gravitasi agar piring tidak melayang saat di-reset
            ObjSetColliderKinematic(ShapePiring[i], true, false);
            index++;
        }

        OtherInteraction[3].transform.position = posisiAwalObj[index];
        OtherInteraction[3].transform.eulerAngles = rotasiAwalObj[index];
        index++;
    }
}