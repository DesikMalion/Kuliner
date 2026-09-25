using ITISKIRUHERE;
using JetBrains.Annotations;
using MikeNspired.XRIStarterKit;
using System.Collections;
using UnityEngine;

public class SceneTeknikPanasBasah : MonoBehaviour
{
    public bool isTest = false;

    public GameObject[] ObjShapes;
    public GameObject[] ObjSocket;

    public GameObject[] ObjShapesSiomay;
    public GameObject[] ObjSocketSiomay;
    public GameObject[] ObjShapesSiomayMatang;
    public GameObject[] ObjSocketSiomayMatang;

    public GameObject[] ObjSelection;
    public GameObject[] ObjUiNarasi;

    void Start()
    {
        NarasiAwal();
    }

    // Update is called once per frame
    void Update()
    {
        CaseMengukus_ControlBurnerOn();
        CaseMengukus_JetBurnerOn();
        CaseMengukus_ControlBurnerOff();
        CaseMengukus_JetBurnerOff();
    }

    void NarasiAwal() {

        DisableUINarasi();
        ObjUiNarasi[0].SetActive(true);

        for (int i = 0; i < ObjSelection.Length; i++) {

            ObjSelection[i].SetActive(false);

        }

        for (int i = 0; i < ObjShapes.Length; i++)
        {

            OutlineOff(ObjShapes[i]);
            ObjSetColliderKinematic(ObjShapes[i], false, true);
            ObjShapes[i].SetActive(false);

        }

        for (int i = 0; i < ObjSocket.Length; i++)
        {
            ObjSocket[i].SetActive(false);
        }

        for (int i = 0; i < ObjShapesSiomay.Length; i++)
        {
            OutlineOff(ObjShapesSiomay[i]);
            ObjSetColliderKinematic(ObjShapesSiomay[i], false, true);
            ObjShapesSiomay[i].SetActive(false);
        }

        for (int i = 0; i < ObjSocketSiomay.Length; i++)
        {
            ObjSocketSiomay[i].SetActive(false);
        }

        for (int i = 0; i < ObjShapesSiomayMatang.Length; i++)
        {
            ObjShapesSiomayMatang[i].SetActive(false);
        }

        for (int i = 0; i < ObjSocketSiomayMatang.Length; i++)
        {
            ObjSocketSiomayMatang[i].SetActive(false);
        }

        ObjShapes[0].SetActive(true);
        ObjShapes[1].SetActive(true);

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

    void DisableUINarasi() {
        for (int i = 0; i < ObjUiNarasi.Length; i++)
        {
            ObjUiNarasi[i].SetActive(false);
        }

    }

    #region Mengukus

    public void UIStartMengukus()
    {
        DisableUINarasi();
        ObjUiNarasi[1].SetActive(true);
    }

    public void StartCaseMengukus() {

        DisableUINarasi();

        for (int i = 0; i < ObjShapesSiomay.Length; i++)
        {
            ObjShapesSiomay[i].SetActive(true);
        }

        ObjShapes[2].SetActive(true);
        ObjShapes[3].SetActive(true);
        ObjShapes[4].SetActive(true);
        ObjShapes[5].SetActive(true);

        ObjSelection[7].SetActive(true);
        ObjSelection[8].SetActive(true);

        //aktifkan gelas
        ObjSocket[0].SetActive(true);
        OutlineOn(ObjShapes[2]);
        ObjSetColliderKinematic(ObjShapes[2], true, false);

    }

    public void CaseMengukus_SekatKukus() {

        StartCoroutine(WaitSekatKukus());

    }

    IEnumerator WaitSekatKukus()
    {

        ObjSelection[2].SetActive(true);
        yield return new WaitForSeconds(3f);

        ObjSelection[2].SetActive(false);
        ObjSocket[0].SetActive(false);
        ObjShapes[2].SetActive(false);

        ObjSelection[1].SetActive(true);
        ObjSocket[1].SetActive(true);
        OutlineOn(ObjShapes[3]);
        ObjSetColliderKinematic(ObjShapes[3], true, false);
    }

    int indexSiomayMentah = 0;
    public void CaseMengukus_SocketSiomayMentah() {

        //ObjSocket[1].SetActive(false);
        ObjSocket[1].GetComponent<Collider>().enabled = false;
        OutlineOff(ObjShapes[3]);
        ObjSetColliderKinematic(ObjShapes[3], false, true);

        for (int i = 0; i < ObjSocketSiomay.Length; i++)
        {
            ObjSocketSiomay[i].SetActive(true);
            OutlineOff(ObjSocketSiomay[i]);
            ObjSocketSiomay[i].GetComponent<Collider>().enabled = false;
        }

        OutlineOn(ObjSocketSiomay[0]);
        ObjSocketSiomay[0].GetComponent<Collider>().enabled = true;

        OutlineOn(ObjShapesSiomay[0]);
        ObjSetColliderKinematic(ObjShapesSiomay[0], true, false);
    }

    public void CaseMengukus_SocketSiomayMentahPanci() {

        OutlineOff(ObjShapesSiomay[indexSiomayMentah]);
        OutlineOff(ObjSocketSiomay[indexSiomayMentah]);
        ObjSocketSiomay[indexSiomayMentah].GetComponent<Collider>().enabled = false;
        ObjSetColliderKinematic(ObjShapesSiomay[indexSiomayMentah], false, true);

        indexSiomayMentah++;

        if (indexSiomayMentah >= ObjShapesSiomay.Length || isTest)
        {
            CaseMengukus_TutupPanci();
        }
        else {

            OutlineOn(ObjShapesSiomay[indexSiomayMentah]);
            ObjSetColliderKinematic(ObjShapesSiomay[indexSiomayMentah], true, false);
            OutlineOn(ObjSocketSiomay[indexSiomayMentah]);
            ObjSocketSiomay[indexSiomayMentah].GetComponent<Collider>().enabled = true;

        }

    }

    public void CaseMengukus_TutupPanci()
    {
        ObjSocket[2].SetActive(true);
        OutlineOn(ObjSocket[2]);
        ObjSocket[2].GetComponent<Collider>().enabled = true;

        OutlineOn(ObjShapes[4]);
        ObjSetColliderKinematic(ObjShapes[4], true, false);


    }

    public void CaseMengukus_CekBurner()
    {
        //ObjSocket[2].SetActive(false);
        OutlineOff(ObjSocket[2]);
        ObjSocket[2].GetComponent<Collider>().enabled = false;

        OutlineOff(ObjShapes[4]);
        ObjSetColliderKinematic(ObjShapes[4], false, true);

        if (!BurnerKnob)
            BurnerKnob = ObjShapes[0].GetComponent<XRKnob>();

        OutlineOn(ObjShapes[0]);
        ObjSetColliderKinematic(ObjShapes[0], true, true);
        isControlBurnerOn = true;
    }

    XRKnob JetBurnerKnob;
    XRKnob BurnerKnob;
    public bool isJetBurnerOn = false;
    public bool isControlBurnerOn = false;
    public bool isJetBurnerOff = false;
    public bool isControlBurnerOff = false;

    void CaseMengukus_ControlBurnerOn()
    {
        if (!isControlBurnerOn) return;
        if (BurnerKnob.Value <= 0.055f)
        {
            if (!JetBurnerKnob)
                JetBurnerKnob = ObjShapes[1].GetComponent<XRKnob>();

            isControlBurnerOn = false;
            isJetBurnerOn = true;

            OutlineOff(ObjShapes[0]);
            ObjSetColliderKinematic(ObjShapes[0], false, true);

            OutlineOn(ObjShapes[1]);
            ObjSetColliderKinematic(ObjShapes[1], true, true);

            ObjSelection[4].SetActive(true);
        }


    }

    void CaseMengukus_JetBurnerOn()
    {
        if (!isJetBurnerOn) return;
        if (JetBurnerKnob.Value <= 0.055f)
        {

            isJetBurnerOn = false;
            OutlineOff(ObjShapes[1]);
            ObjSetColliderKinematic(ObjShapes[1], false, true);

            ObjSelection[5].SetActive(true);
            StartCoroutine(WaitSiomayMatang());
        }


    }

    IEnumerator WaitSiomayMatang()
    {
        ObjSelection[9].SetActive(true);
        yield return new WaitForSeconds(5f);
        ObjSelection[3].SetActive(true);
        yield return new WaitForSeconds(3f);

        for (int i = 0; i < ObjShapesSiomay.Length; i++)
        {
            ObjShapesSiomay[i].SetActive(false);
        }

        for (int i = 0; i < ObjSocketSiomay.Length; i++)
        {
            ObjSocketSiomay[i].SetActive(false);
        }

        for (int i = 0; i < ObjShapesSiomayMatang.Length; i++)
        {
            ObjShapesSiomayMatang[i].SetActive(true);
            OutlineOff(ObjShapesSiomayMatang[i]);
            ObjSetColliderKinematic(ObjShapesSiomayMatang[i], false, true);
        }

        yield return new WaitForSeconds(1f);

        ObjSelection[9].SetActive(false);

        isJetBurnerOff = true;
        OutlineOn(ObjShapes[1]);
        ObjSetColliderKinematic(ObjShapes[1], true, true);

    }

    void CaseMengukus_JetBurnerOff()
    {
        if (!isJetBurnerOff) return;
        if (JetBurnerKnob.Value >= 0.945f)
        {

            isJetBurnerOff = false;
            OutlineOff(ObjShapes[1]);
            ObjSetColliderKinematic(ObjShapes[1], false, true);
            ObjSelection[5].SetActive(false);

            OutlineOn(ObjShapes[0]);
            ObjSetColliderKinematic(ObjShapes[0], true, true);

            isControlBurnerOff = true;

        }


    }

    void CaseMengukus_ControlBurnerOff() {
        if (!isControlBurnerOff) return;
        if (BurnerKnob.Value >= 0.945f)
        {
            OutlineOff(ObjShapes[0]);
            ObjSetColliderKinematic(ObjShapes[0], false, true);
            ObjSelection[4].SetActive(false);
            ObjSocket[2].SetActive(false);

            isControlBurnerOff = false;

            OutlineOn(ObjShapes[4]);
            ObjShapes[4].isStatic = false;
            ObjSetColliderKinematic(ObjShapes[4], true, false);
            ObjSocket[3].SetActive(true);
        }


    }

    public void CaseMengukus_CapitOn() {

        OutlineOff(ObjShapes[4]);
        ObjSetColliderKinematic(ObjShapes[4], false, true);

        OutlineOn(ObjShapes[5]);
        ObjSetColliderKinematic(ObjShapes[5], true, false);

    }

    public void CaseMengukus_CapitAvatarOn()
    {
        ObjShapes[5].SetActive(false);
        ObjSelection[6].SetActive(true);

        for (int i = 0; i < ObjShapesSiomayMatang.Length; i++)
        {
            ObjShapesSiomayMatang[i].SetActive(true);

            OutlineOff(ObjShapesSiomayMatang[i]);
            ObjSetColliderKinematic(ObjShapesSiomayMatang[i], false, true);
        }

        for (int i = 0; i < ObjSocketSiomayMatang.Length; i++)
        {
            ObjSocketSiomayMatang[i].SetActive(true);
            OutlineOff(ObjSocketSiomayMatang[i]);
            ObjSocketSiomayMatang[0].GetComponentInChildren<Collider>().enabled = false;
        }

        OutlineOn(ObjShapesSiomayMatang[0]);
        ObjSetColliderKinematic(ObjShapesSiomayMatang[0], true, true);

        OutlineOn(ObjSocketSiomayMatang[0]);
        ObjSocketSiomayMatang[0].GetComponentInChildren<Collider>().enabled = true;
    }

    int indexSiomayMatang = 0;
    public void CaseMengukus_SocketSiomayMatang() {

        OutlineOff(ObjShapesSiomayMatang[indexSiomayMatang]);
        ObjSetColliderKinematic(ObjShapesSiomayMatang[indexSiomayMatang], false, true);

        OutlineOff(ObjSocketSiomayMatang[indexSiomayMatang]);
        ObjSocketSiomayMatang[indexSiomayMatang].GetComponentInChildren<Collider>().enabled = false;

        indexSiomayMatang++;

        if (indexSiomayMatang >= ObjShapesSiomayMatang.Length || isTest)
        {
            CaseMengukus_Finish();
        }
        else
        {

            OutlineOn(ObjShapesSiomayMatang[indexSiomayMatang]);
            ObjSetColliderKinematic(ObjShapesSiomayMatang[indexSiomayMatang], true, true);

            OutlineOn(ObjSocketSiomayMatang[indexSiomayMatang]);
            ObjSocketSiomayMatang[indexSiomayMatang].GetComponentInChildren<Collider>().enabled = true;

        }

    }

    void CaseMengukus_Finish() {

        for (int i = 0; i < ObjShapesSiomayMatang.Length; i++)
        {
            ObjShapesSiomayMatang[i].SetActive(true);

            OutlineOff(ObjShapesSiomayMatang[i]);
            ObjSetColliderKinematic(ObjShapesSiomayMatang[i], false, true);
        }

        for (int i = 0; i < ObjSocketSiomayMatang.Length; i++)
        {
            ObjSocketSiomayMatang[i].SetActive(false);


        }

        DisableUINarasi();
        ObjUiNarasi[3].SetActive(true);

    }


    #endregion

}
