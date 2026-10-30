using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PersiapanDasarMasakMgr : MonoBehaviour
{

    public GameObject[] UIList;

    public GameObject[] ShapeResep;
    public GameObject[] SocketResep;

    public GameObject[] ShapeTag;
    public GameObject[] SocketTag;

    List<Vector3> posisiAwalObjResep = new List<Vector3>();
    List<Vector3> rotasiAwalObjResep = new List<Vector3>();

    public GameObject[] ObjInteraction;

    // knife skill & gamasi
    public WaterPourDetect waterPourDetect;
    public TimbanganMemasak[] timbanganMemasak;
    public GameObject OutlineGramasi;


    void Start()
    {

        GetAllObjPos();
        StartMateri();
    }


    void Update()
    {

    }


    public void ObjSetColliderKinematic(GameObject obj, bool ColEnable, bool KinematicEnable)
    {
        BoxCollider[] Colliders = obj.transform.GetComponentsInChildren<BoxCollider>(true);
        foreach (BoxCollider Collider in Colliders)
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

    public void ShowUI(int UIShow)
    {
        foreach (GameObject UI in UIList)
        {
            UI.SetActive(false);
        }

        UIList[UIShow].SetActive(true);
    }

    void GetAllObjPos()
    {

        for (int i = 0; i < ShapeResep.Length; i++)
        {
            posisiAwalObjResep.Add(ShapeResep[i].transform.position);
            rotasiAwalObjResep.Add(ShapeResep[i].transform.eulerAngles);
        }


    }

    void StartMateri()
    {
        ShowUI(1);
        UIList[12].SetActive(true);

        for (int i = 0; i < ObjInteraction.Length; i++)
        {
            ObjInteraction[i].SetActive(false);
        }

        for (int i = 0;i < ShapeTag.Length; i++)
        {
            ShapeTag[i].SetActive(false);
        }

        for (int i = 0;i < SocketTag.Length; i++)
        {
            SocketTag[i].SetActive(false);
        }

        ObjInteraction[0].SetActive(true);
        OutlineGramasi.SetActive(false);


        if (PlayerPrefs.GetInt("TeknikPisau", 0) == 1)
        {
            StartTeknikPisau();
        }
    }

    #region 1 Resep

    public void CekMateriResep() {

        bool isResepFinish = true;
        bool isAllLocked = true;
        for (int i = 0; i < SocketResep.Length; i++)
        {
            if (!SocketResep[i].GetComponent<SocketLockObject>().isFinishLocked)
            {
                isResepFinish = false;
                isAllLocked = false;
                //return;
                break;
            }
            else
            {
                if (SocketResep[i].GetComponent<SocketLockObject>().ObjSnapped.tag != "Target")
                {
                    isResepFinish = false;
                    //break;
                }
            }
        }

        if (!isAllLocked)
        {
            return;
        }

        StartCoroutine(WaitShowResepFinish(isResepFinish));

    }

    IEnumerator WaitShowResepFinish(bool isResepFinish)
    {
        yield return new WaitForSeconds(1f);
        if (isResepFinish)
        {
            ShowUI(4);
        }
        else
        {
            ShowUI(3);
        }
    }

    public void ResetObjResep()
    {
        //StartCoroutine(WaitResetObjResep());

        //load ulang scene ini
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    IEnumerator WaitResetObjResep()
    {
        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < SocketResep.Length; i++)
        {
            SocketResep[i].SetActive(false);
            SocketResep[i].GetComponent<SocketLockObject>().isFinishLocked = false;
        }

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < ShapeResep.Length; i++)
        {
            ShapeResep[i].transform.position = posisiAwalObjResep[i];
            ShapeResep[i].transform.eulerAngles = rotasiAwalObjResep[i];
        }

        ShowUI(0);

        yield return new WaitForSeconds(0.1f);

        for (int i = 0; i < SocketResep.Length; i++)
        {
            SocketResep[i].SetActive(true);
            SocketResep[i].GetComponent<SocketLockObject>().onObjectUnsnappedEvent.Invoke();
        }

    }

    public void StartPenataan() { 
        ShowUI(8);
        for (int i = 0; i < ShapeTag.Length; i++)
        {
            ShapeTag[i].SetActive(true);
        }

        for (int i = 0; i < SocketTag.Length; i++)
        {
            SocketTag[i].SetActive(true);
        }

        ObjSetColliderKinematic(ShapeResep[0],false,true);
        ObjSetColliderKinematic(ShapeResep[1],false,true);
        ObjSetColliderKinematic(ShapeResep[4],false,true);
        ObjSetColliderKinematic(ShapeResep[8],false,true);
        ObjSetColliderKinematic(ShapeResep[9],false,true);
        ObjSetColliderKinematic(ShapeResep[10],false,true);

    }

    public void CekLabelResep()
    {

        bool isResepFinish = true;
        bool isAllLocked = true;
        for (int i = 0; i < SocketTag.Length; i++)
        {
            if (!SocketTag[i].GetComponent<SocketLockObject>().isFinishLocked)
            {
                isResepFinish = false;
                isAllLocked = false;
                //return;
                break;
            }
            else
            {
                string shapename = SocketTag[i].GetComponent<SocketLockObject>().ObjSnapped.name.ToLower();
                string socketname = SocketTag[i].name.ToLower();
                if (!socketname.Contains(shapename))
                {
                    isResepFinish = false;
                    //break;
                }
            }
        }

        if (!isAllLocked)
        {
            return;
        }

        StartCoroutine(WaitShowLabelFinish(isResepFinish));

    }

    IEnumerator WaitShowLabelFinish(bool isResepFinish)
    {
        yield return new WaitForSeconds(1f);
        if (isResepFinish)
        {
            ShowUI(10);
        }
        else
        {
            ShowUI(9);
        }
    }

    #endregion

    #region 2 Teknik Pisau

    public void StartTeknikPisau()
    {
        PlayerPrefs.SetInt("TeknikPisau", 0);
        ShowUI(5);
        for (int i = 0; i < ShapeResep.Length; i++)
        {
            ShapeResep[i].SetActive(false);
        }

        for (int i = 0; i < ObjInteraction.Length; i++)
        {
            ObjInteraction[i].SetActive(false);
        }

        ObjInteraction[1].SetActive(true);

    }

    public void ResetTeknikPisau() { 
        PlayerPrefs.SetInt("TeknikPisau", 1);

        //load ulang scene ini
        UnityEngine.SceneManagement.SceneManager.LoadScene(UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);

    }

    float beratWortelResep = 150;
    float beratKentangResep = 200;
    float beratSawiResep = 50;
    float volumeAirResep = 800;

    public void CekTeknikPisau(bool isbutton) {

        float beratWortelTimbangan = 0;
        float beratKentangTimbangan = 0;
        float beratSawiTimbangan = 0;
        int volumeAirTimbangan = 0;

        for (int i = 0; i < timbanganMemasak.Length; i++) {
            for (int j = 0; j < timbanganMemasak[i].objWeighingCol.Length; j++) {

                if (timbanganMemasak[i].objWeighingCol[j].gameObject.name.ToLower().Contains("wortel")) {
                    float mass = timbanganMemasak[i].objWeighingCol[j].gameObject.GetComponent<Rigidbody>().mass;
                    beratWortelTimbangan += mass;

                } else if (timbanganMemasak[i].objWeighingCol[j].gameObject.name.ToLower().Contains("kentang"))
                {
                    float mass = timbanganMemasak[i].objWeighingCol[j].gameObject.GetComponent<Rigidbody>().mass;
                    beratKentangTimbangan += mass;

                }
                else if (timbanganMemasak[i].objWeighingCol[j].gameObject.name.ToLower().Contains("sawi"))
                {
                    float mass = timbanganMemasak[i].objWeighingCol[j].gameObject.GetComponent<Rigidbody>().mass;
                    beratSawiTimbangan += mass;

                }

            }
        }

        volumeAirTimbangan = Mathf.FloorToInt(waterPourDetect.scaleNow * 1000);
        int beratWortelTimbangan_ = Mathf.FloorToInt(beratWortelTimbangan);
        int beratKentangTimbangan_ = Mathf.FloorToInt(beratKentangTimbangan);
        int beratSawiTimbangan_ = Mathf.FloorToInt(beratSawiTimbangan);

        bool wortelBenar = Mathf.Abs(beratWortelTimbangan_ - beratWortelResep) <= 6;
        bool kentangBenar = Mathf.Abs(beratKentangTimbangan_ - beratKentangResep) <= 6;
        bool sawiBenar = Mathf.Abs(beratSawiTimbangan_ - beratSawiResep) <= 6;
        bool airBenar = Mathf.Abs(volumeAirTimbangan - volumeAirResep) <= 55;
        bool semuabenar = true;

        Debug.Log("Wortel: " + (wortelBenar ? "Benar" : "Salah"));
        Debug.Log("Kentang: " + (kentangBenar ? "Benar" : "Salah"));
        Debug.Log("Sawi: " + (sawiBenar ? "Benar" : "Salah"));
        Debug.Log("Air: " + (airBenar ? "Benar" : "Salah"));

        if (isbutton)
        {
            if (airBenar && sawiBenar && wortelBenar && kentangBenar)
            {

                ShowUI(7);
            }
            else
            {

                ShowUI(6);
            }
        }
        else
        {
            if (airBenar && sawiBenar && wortelBenar && kentangBenar)
            {

                OutlineGramasi.SetActive(true);
            }
            else
            {

                OutlineGramasi.SetActive(false);
            }

        } 
    
    }

   #endregion

}