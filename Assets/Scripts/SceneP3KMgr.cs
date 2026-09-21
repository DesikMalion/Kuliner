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

    List<Vector3>ShapesPos = new List<Vector3>();
    List<Vector3>ShapesRot = new List<Vector3>();


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
            ObjSetKinematic(ObjShapes[i], false,false);
        }

        HideUIShapes();
    }

    void ResetObjShapesPos() {

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

        for (int i = 0; i < ObjListKuis.Length; i++) {

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

    public void ObjSetKinematic(GameObject obj, bool enable, bool setAll = true) {
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
        else {
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

    public void OnGrab(GameObject obj) {

        if (!isStarted)return;

        if (isKuis1)
        {
            Kuis1SarungTangan();

            return;
        }


        if (isKuis)
            return;

        OutlineOff(obj);
        isGrabbed = true;
        ShowUIShapes(obj);
        //ObjSetKinematic(obj, false);




    }

    public void OnRelease(GameObject obj) {
        if (!isStarted) return;

        if (isKuis)
            return;

        OutlineOff(obj);
        isGrabbed = false;
        HideUIShapes();
        //ObjSetKinematic(obj, false);
    }

    public void OnHoverEnter(GameObject obj) {
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

    public void OnHoverExit(GameObject obj) {
        if (!isStarted) return;

        if (isKuis)
            return;

        OutlineOff(obj);
        isHovered = false;
        HideUIShapes();
    }


    #region Kuis

    bool isKuis = false;
    public void ButtonStartKuis() {

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
    public void Kuis1SarungTangan() {

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

    public void Kuis1Plester() {


        StartCoroutine(WaitSetKinematic(ObjShapes[2], false));
        

        //ObjSetKinematic(ObjShapes[15], true, false);

        StartCoroutine(WaitKuis1Plester());
    }

    IEnumerator WaitKuis1Plester()
    {
        yield return new WaitForSeconds(.25f);
        StartCoroutine(WaitSetKinematic(ObjShapes[15], true));
        OutlineOn(ObjShapes[15]);
        OutlineOff(ObjShapes[2]);
        ObjListKuis[1].SetActive(false);
        ObjListKuis[2].SetActive(false);

        ObjListKuis[3].SetActive(true);
        ObjListKuis[5].SetActive(true);

    }

    public void Kuis1Finish() {

        //ObjSetKinematic(ObjShapes[15], false, false);
       // ObjShapes[15].GetComponentInChildren<BoxCollider>().enabled = false;
        //ObjShapes[15].GetComponent<Rigidbody>().isKinematic = true;
        StartCoroutine(WaitSetKinematic(ObjShapes[15], false));
        
        HideUIShapes();
        OutlineOff(ObjShapes[15]);
        ObjListKuis[5].SetActive(false);

        ObjListKuis[4].SetActive(true);
        UIShapes[18].SetActive(true);
    }



    #endregion



}
