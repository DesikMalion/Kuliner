using ITISKIRUHERE;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;

public class SceneP3KMgr : MonoBehaviour
{
    public bool isGrabbed = false;
    public bool isHovered = false;

    bool isStarted = false;
    public GameObject Shot1, ShotFinal, TombolKuis;
    public GameObject[] ObjShapes;
    public GameObject[] ObjSocket;
    public GameObject[] UIShapes;
    public GameObject[] ObjListKuis;

    List<Vector3> ShapesPos = new List<Vector3>();
    List<Vector3> ShapesRot = new List<Vector3>();


    void Start()
    {
        ShowShot1();
    }


    void Update()
    {

    }

    private void ResetShapesSocket()
    {
        for (int i = 0; i < ObjSocket.Length; i++)
        {
            ObjSocket[i].GetComponent<P3KSocket>().Socket.SetActive(false);

        }

        ResetObjShapesPos();

        for (int i = 0; i < ObjShapes.Length; i++)
        {
            ObjSetKinematic(ObjShapes[i], false, false);
        }

        HideUIShapes();
    }

    void ResetObjShapesPos()
    {

        for (int i = 0; i < ObjShapes.Length; i++)
        {
            //OutlineOff(ObjShapes[i]);
            ObjShapes[i].transform.position = ShapesPos[i];
            ObjShapes[i].transform.eulerAngles = ShapesRot[i];
        }
    }

    public void ShowShot1()
    {
        for (int i = 0; i < ObjShapes.Length; i++)
        {
            OutlineOff(ObjShapes[i]);

            ShapesPos.Add(ObjShapes[i].transform.position);
            ShapesRot.Add(ObjShapes[i].transform.eulerAngles);
        }

        for (int i = 0; i < ObjListKuis.Length; i++)
        {

            ObjListKuis[i].SetActive(false);
        }

        ObjListKuis[6].SetActive(true);
        ObjListKuis[7].SetActive(true);

        HideUIShapes();

        Shot1.gameObject.SetActive(true);
        TombolKuis.gameObject.SetActive(false);
        ShotFinal.gameObject.SetActive(false);
        //isStarted = true;

    }

    public void StartScene()
    {
        TombolKuis.gameObject.SetActive(true);
        Shot1.gameObject.SetActive(false);
        ShotFinal.gameObject.SetActive(false);
        isStarted = true;
    }

    public void OutlineOn(GameObject obj)
    {
        OutlineEnabler(true, obj);
    }

    public void OutlineOff(GameObject obj)
    {
        OutlineEnabler(false, obj);
    }

    public void OutlineEnabler(bool isEnable, GameObject obj)
    {
        Debug.Log("OutlineEnabler called with isEnable: " + isEnable + " for object: " + obj.name);
        AdvancedOutline advancedOutline = obj.GetComponent<AdvancedOutline>();

        if (advancedOutline == null)
        {
            advancedOutline = obj.GetComponentInChildren<AdvancedOutline>(true);
        }
        if (advancedOutline == null)
        {
            //Debug.LogError("AdvancedOutline component not found on the object or its children.");
            return;
        }
        if (isEnable)
        {
            advancedOutline.PulseWidth = true;
            advancedOutline.OutlineMode = AdvancedOutline.Mode.OutlineVisible;
            advancedOutline.OutlineWidth = 10;
            advancedOutline.gameObject.SetActive(true);

        }
        else
        {
            advancedOutline.PulseWidth = false;
            advancedOutline.OutlineMode = AdvancedOutline.Mode.OutlineHidden;
            advancedOutline.OutlineWidth = 0;
            advancedOutline.gameObject.SetActive(false);

        }

    }

    public void ShowUIShapes(GameObject obj)
    {
        Debug.Log("ShowUIShapes called for object: " + obj.name);
        for (int i = 0; i < ObjShapes.Length; i++)
        {
            if (ObjShapes[i].name == obj.name)
            {
                Debug.Log("Showing UI for shape: " + obj.name);
                UIShapes[i].SetActive(true);
            }
            else
            {
                UIShapes[i].SetActive(false);
            }
        }

    }

    public void ObjSetKinematic(GameObject obj, bool enable, bool setAll = true)
    {
        // Debug.Log("ObjSetKinematic called with enable: " + enable + " for object: " + obj.name);

        if (setAll)
        {
            for (int i = 0; i < ObjShapes.Length; i++)
            {
                if (ObjShapes[i].name != obj.name)
                {
                    ObjShapes[i].GetComponentInChildren<BoxCollider>().enabled = enable;
                    ObjShapes[i].GetComponent<Rigidbody>().isKinematic = !enable;

                }
            }
        }
        else
        {
            obj.GetComponentInChildren<BoxCollider>().enabled = enable;
            obj.GetComponent<Rigidbody>().isKinematic = !enable;
        }

    }

    public void HideUIShapes()
    {
        for (int i = 0; i < UIShapes.Length; i++)
        {
            UIShapes[i].SetActive(false);
        }
    }

    public void OnGrab(GameObject obj)
    {

        if (!isStarted) return;

        if (isKuis1)
        {
            Kuis1SarungTangan();

            return;
        }

        if (isKuis2)
        {
            StartCoroutine(Kuis2Jawab(obj));

            return;
        }

        if (isKuis3)
        {
            StartCoroutine(Kuis3Jawab(obj));

            return;
        }

        if (isKuis4)
        {
            StartCoroutine(Kuis4Jawab(obj));

            return;
        }

        if (isKuis)
            return;

        OutlineOff(obj);
        isGrabbed = true;
        ShowUIShapes(obj);
        //ObjSetKinematic(obj, false);




    }

    public void OnRelease(GameObject obj)
    {
        if (!isStarted) return;

        if (isKuis)
            return;

        OutlineOff(obj);
        isGrabbed = false;
        HideUIShapes();
        //ObjSetKinematic(obj, false);
    }

    public void OnHoverEnter(GameObject obj)
    {
        if (!isStarted) return;

        if (isKuis)
            return;

        if (!isGrabbed)
        {
            OutlineOn(obj);
            isHovered = true;
            ShowUIShapes(obj);
        }
    }

    public void OnHoverExit(GameObject obj)
    {
        if (!isStarted) return;

        if (isKuis)
            return;

        OutlineOff(obj);
        isHovered = false;
        HideUIShapes();
    }


    #region Kuis 1

    bool isKuis = false;
    public void ButtonStartKuis()
    {

        TombolKuis.SetActive(false);
        ResetShapesSocket();
        for (int i = 0; i < ObjSocket.Length; i++)
        {
            ObjSetKinematic(ObjSocket[i], false, false);
        }

        UIShapes[17].SetActive(true);
        ObjListKuis[0].SetActive(true);
        ObjListKuis[1].SetActive(true);


        ObjSetKinematic(ObjShapes[16], true, false);
        OutlineOn(ObjShapes[16]);
        isKuis1 = true;
        isKuis = true;

    }

    bool isKuis1 = false;

    IEnumerator WaitSetKinematic(GameObject obj, bool enable)
    {

        yield return new WaitForSeconds(.15f);
        ObjSetKinematic(obj, enable, false);
        ResetObjShapesPos();

    }
    public void Kuis1SarungTangan()
    {

        ObjSetKinematic(ObjShapes[16], false, false);
        OutlineOff(ObjShapes[16]);
        ResetObjShapesPos();

        OutlineOn(ObjShapes[2]);
        ObjSetKinematic(ObjShapes[2], true, false);

        ObjListKuis[6].SetActive(false);
        ObjListKuis[7].SetActive(false);
        HideUIShapes();

        UIShapes[17].SetActive(true);
        ObjListKuis[2].SetActive(true);
        ObjListKuis[8].SetActive(true);
        ObjListKuis[9].SetActive(true);

        isKuis1 = false;
    }

    public void Kuis1Plester()
    {


        //StartCoroutine(WaitSetKinematic(ObjShapes[2], false));


        //ObjSetKinematic(ObjShapes[15], true, false);

        StartCoroutine(WaitKuis1Plester());
    }

    IEnumerator WaitKuis1Plester()
    {
        ResetObjShapesPos();
        yield return new WaitForSeconds(.5f);
        StartCoroutine(WaitSetKinematic(ObjShapes[15], true));
        ObjShapes[2].SetActive(false);
        OutlineOn(ObjShapes[15]);
        OutlineOff(ObjShapes[2]);
        ObjListKuis[1].SetActive(false);
        ObjListKuis[2].SetActive(false);

        ObjListKuis[3].SetActive(true);
        ObjListKuis[5].SetActive(true);

    }

    public void Kuis1Finish()
    {

        ObjSetKinematic(ObjShapes[2], false, false);
        ResetObjShapesPos();
        StartCoroutine(WaitSetKinematic(ObjShapes[15], false));

        HideUIShapes();
        OutlineOff(ObjShapes[15]);
        ObjListKuis[5].SetActive(false);

        ObjListKuis[4].SetActive(true);
        UIShapes[18].SetActive(true);

    }



    #endregion

    #region Kuis 2

    bool isKuis2 = false;
    public void Kuis2Start()
    {

        isKuis2 = true;
        HideUIShapes();
        ResetObjShapesPos();
        ObjShapes[2].SetActive(true);
        UIShapes[19].SetActive(true);

        for (int i = 0; i < ObjListKuis.Length; i++)
        {

            ObjListKuis[i].SetActive(false);

        }

        ObjListKuis[6].SetActive(true);
        ObjListKuis[7].SetActive(true);

        OutlineOn(ObjShapes[15]);
        ObjSetKinematic(ObjShapes[15], true, false);

        OutlineOn(ObjShapes[8]);
        ObjSetKinematic(ObjShapes[8], true, false);

        OutlineOn(ObjShapes[10]);
        ObjSetKinematic(ObjShapes[10], true, false);

        OutlineOn(ObjShapes[14]);
        ObjSetKinematic(ObjShapes[14], true, false);

    }


    IEnumerator Kuis2Jawab(GameObject obj)
    {
        HideUIShapes();
        // benar = ObjShapes[15]
        bool benar = false;
        if (obj.name == ObjShapes[15].name)
        {
            benar = true;
        }

        StartCoroutine(KuisPanelJawaban(benar));

        yield return new WaitForSeconds(3f);
        
        UIShapes[19].SetActive(true);
        if (benar)
        {
            isKuis2 = false;
            Kuis3Start();
        }

    }

    IEnumerator KuisPanelJawaban(bool benar)
    {
        if (benar)
        {
            UIShapes[21].SetActive(true);
        }
        else
        {
            UIShapes[20].SetActive(true);
        }
        yield return new WaitForSeconds(2.5f);
        
            UIShapes[21].SetActive(false);

            UIShapes[20].SetActive(false);


    }

    #endregion

    #region Kuis 3

    bool isKuis3 = false;
    void Kuis3Start()
    {
        HideUIShapes();

        OutlineOff(ObjShapes[15]);
        ObjSetKinematic(ObjShapes[15], false, false);

        OutlineOff(ObjShapes[8]);
        ObjSetKinematic(ObjShapes[8], false, false);

        OutlineOff(ObjShapes[10]);
        ObjSetKinematic(ObjShapes[10], false, false);

        OutlineOff(ObjShapes[14]);
        ObjSetKinematic(ObjShapes[14], false, false);

        ResetObjShapesPos();

        isKuis3 = true;
        HideUIShapes();
        ResetObjShapesPos();

        UIShapes[22].SetActive(true);

        for (int i = 0; i < ObjListKuis.Length; i++)
        {

            ObjListKuis[i].SetActive(false);

        }


        OutlineOn(ObjShapes[10]);
        ObjSetKinematic(ObjShapes[10], true, false);

        OutlineOn(ObjShapes[2]);
        ObjSetKinematic(ObjShapes[2], true, false);

        OutlineOn(ObjShapes[16]);
        ObjSetKinematic(ObjShapes[16], true, false);

        OutlineOn(ObjShapes[12]);
        ObjSetKinematic(ObjShapes[12], true, false);
    }

    IEnumerator Kuis3Jawab(GameObject obj)
    {
        HideUIShapes();
        // benar = ObjShapes[15]
        bool benar = false;
        if (obj.name == ObjShapes[10].name)
        {
            benar = true;
        }

        StartCoroutine(KuisPanelJawaban(benar));

        yield return new WaitForSeconds(3f);

        UIShapes[22].SetActive(true);
        if (benar)
        {
            isKuis3 = false;
            Kuis4Start();
        }

    }



    #endregion

    #region Kuis 4
    bool isKuis4 = false;
    void Kuis4Start()
    {
        HideUIShapes();

        OutlineOff(ObjShapes[10]);
        ObjSetKinematic(ObjShapes[10], false, false);

        OutlineOff(ObjShapes[2]);
        ObjSetKinematic(ObjShapes[2], false, false);

        OutlineOff(ObjShapes[16]);
        ObjSetKinematic(ObjShapes[16], false, false);

        OutlineOff(ObjShapes[12]);
        ObjSetKinematic(ObjShapes[12], false, false);

        isKuis4 = true;
        HideUIShapes();
        ResetObjShapesPos();

        UIShapes[23].SetActive(true);

        for (int i = 0; i < ObjListKuis.Length; i++)
        {

            ObjListKuis[i].SetActive(false);

        }

        OutlineOn(ObjShapes[16]);
        ObjSetKinematic(ObjShapes[16], true, false);

        OutlineOn(ObjShapes[1]);
        ObjSetKinematic(ObjShapes[1], true, false);

        OutlineOn(ObjShapes[7]);
        ObjSetKinematic(ObjShapes[7], true, false);

        OutlineOn(ObjShapes[15]);
        ObjSetKinematic(ObjShapes[15], true, false);


    }

    IEnumerator Kuis4Jawab(GameObject obj)
    {
        HideUIShapes();
        // benar = ObjShapes[15]
        bool benar = false;
        if (obj.name == ObjShapes[16].name)
        {
            benar = true;
        }

        StartCoroutine(KuisPanelJawaban(benar));

        yield return new WaitForSeconds(3f);

        
        if (benar)
        {
            isKuis4 = false;
            ShotFinal.SetActive(true);
        }
        else {

            UIShapes[22].SetActive(true);
        } 
    

    }

    #endregion

}
