using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class PencucianManager : MonoBehaviour
{

    public GameObject [] ShapePiring;
    public GameObject [] SocketPiring;
    public GameObject [] OtherSocket;
    public GameObject [] OtherInteraction;

    public FireExtinguisherSpray fireExtinguisherSpray;
    GameObject PiringSpray = null;
    ObjectPiringPencucian objectPiringPencucian;

    public AreaProgressManager progressManager;

    int snappedObj = 0;

    List<Vector3> posisiAwalObj = new List<Vector3>();
    List<Vector3> rotasiAwalObj = new List<Vector3>();

    [Header("Referensi UI Panel (Satu Pintu)")]
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
        GetAllObjPos();
        StartPencucian();
    }

    void GetAllObjPos() {

        for (int i = 0; i < ShapePiring.Length; i++)
        {
            posisiAwalObj.Add(ShapePiring[i].transform.position);
            rotasiAwalObj.Add(ShapePiring[i].transform.eulerAngles);
        }


            posisiAwalObj.Add(OtherInteraction[3].transform.position);
            rotasiAwalObj.Add(OtherInteraction[3].transform.eulerAngles);
        

    }


    void StartPencucian() { 

        for (int i = 0; i < ShapePiring.Length; i++)
            {
                ObjSetColliderKinematic(ShapePiring[i], false, true);
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

        ObjSetColliderKinematic(ShapePiring[0], true, true);
        ObjSetColliderKinematic(OtherInteraction[3], true, false);
        SocketPiring[0].SetActive(true);
        OtherSocket[0].SetActive(true);
        OtherInteraction[0].SetActive(true);
        OtherInteraction[3].SetActive(true);

    }

    void Update()
    {
        
    }

    public void ObjSetColliderKinematic(GameObject obj, bool ColEnable, bool KinematicEnable)
    {
        // Debug.Log("ObjSetKinematic called with enable: " + enable + " for object: " + obj.name);
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


    public void PiringSnapped(GameObject objPiring) {

        for (int i = 0; i < SocketPiring.Length; i++)
        {
            if (SocketPiring[i].name == objPiring.name) {
                SocketPiring[i].GetComponent<XRSocketInteractor>().attachTransform.gameObject.SetActive(false);
                SocketPiring[i].GetComponent<SocketLockObject>().ObjSnapped.GetComponent<ObjectPiringPencucian>().isSnapped = true;
                ObjSetColliderKinematic(ShapePiring[i], true, true);
                // With this line, using the new 'interactionLayers' property and disabling interaction by setting it to '0':
                //ShapePiring[i].GetComponent<XRGrabInteractable>().interactionLayers = 0;
                snappedObj++;

                if (snappedObj < ShapePiring.Length)
                {
                    ObjSetColliderKinematic(ShapePiring[snappedObj], true, true);
                    SocketPiring[snappedObj].SetActive(true);
                }
            }
        }
    
    }

    public void PiringSprayDetected() {
        
        GameObject objPiring = fireExtinguisherSpray.GetSprayObject();

        if (objPiring == null)
        {
            return;
        }

        if (PiringSpray != objPiring)
        {
            PiringSpray = objPiring;
            objectPiringPencucian = PiringSpray.GetComponent<ObjectPiringPencucian>();
            
        }

        objectPiringPencucian.CuciPiring();
    }

    public void RakPiringSnapped() {
        OtherInteraction[1].SetActive(true);


    }

    bool isMesinCuciOpen = false;
    public void GrabMesinCuci() {

        OtherInteraction[1].SetActive(false);

        if (!isMesinCuciOpen)
        {
            OtherInteraction[0].GetComponent<Animator>().Play("MesinCuciTutup");
            isMesinCuciOpen = true;
            StartCoroutine(WaitMesinCuciFinish());
        }
        else {

            OtherInteraction[0].GetComponent<Animator>().Play("MesinCuciBuka");
            isMesinCuciOpen = false;
        }
    }

    IEnumerator WaitMesinCuciFinish()
    {

        yield return new WaitForSeconds(5f);
        OtherInteraction[1].SetActive(true);
        //OtherInteraction[0].GetComponent<Animator>().Play("MesinCuciBuka");
        OtherInteraction[2].SetActive(true);

        ObjSetColliderKinematic(OtherInteraction[3],true,false);
        OtherInteraction[3].isStatic = false;
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

    public void LaporanPencucian() {
        objekInstruksi.SetActive(false);
        objekLaporan.SetActive(true);
        tombolLanjutkan.SetActive(true);
        tombolReset.SetActive(true);
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
        // Menyembunyikan seluruh Canvas/Panel Induk ini dari hadapan pemain
        if (objekInstruksi != null)
        {
            objekInstruksi.SetActive(true);
            objekLaporan.SetActive(false);
        }

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
            OtherSocket[i].isStatic = false;
        }

        for (int i = 0; i < SocketPiring.Length; i++)
        {
            SocketPiring[i].GetComponent<XRSocketInteractor>().attachTransform.gameObject.SetActive(true);
            SocketPiring[i].isStatic = false;
        }


        PiringSpray = null;
        snappedObj = 0;
        objekInstruksi.SetActive(true);
        objekLaporan.SetActive(false);

        StartPencucian();
    }

    void ResetAllObjPos()
    {
        int index = 0;
        for (int i = 0; i < ShapePiring.Length; i++)
        {
            ShapePiring[i].transform.position = posisiAwalObj[index];
            ShapePiring[i].transform.eulerAngles = rotasiAwalObj[index];
            ShapePiring[i].isStatic = false;
            index++;
        }


            OtherInteraction[3].transform.position = posisiAwalObj[index];
            OtherInteraction[3].transform.eulerAngles = rotasiAwalObj[index];
            OtherInteraction[3].isStatic = false;
            index++;


    }

}
