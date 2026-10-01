using ITISKIRUHERE;
using MikeNspired.XRIStarterKit;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SceneTeknikPanasBasah : MonoBehaviour
{
    public bool isTest = false;

    public int indexMateri = 0;
    public GameObject[] ObjParentMateri;
    public GameObject[] ObjShapes;
    public GameObject[] ObjSocket;

    public GameObject[] ObjShapesSiomay;
    public GameObject[] ObjSocketSiomay;
    public GameObject[] ObjShapesSiomayMatang;
    public GameObject[] ObjSocketSiomayMatang;

    public GameObject[] ObjSelection;
    public GameObject[] ObjUiNarasi;

    XRKnob JetBurnerKnob;
    XRKnob BurnerKnob;

    public bool isJetBurnerOn = false;
    public bool isControlBurnerOn = false;
    public bool isJetBurnerOff = false;
    public bool isControlBurnerOff = false;


    void Start()
    {
        
        if (!isTest)
        {
            NarasiAwal();
        }
        else {

            if (indexMateri == 0)
            {
                UIStartMengukus();
            }
            else if (indexMateri == 1)
            {
                UIStartSoup();
            }
            else if (indexMateri == 2)
            {
                UIStartMerebus();
            }
            else if (indexMateri == 3)
            {
                UIStartPoaching();
            }
            else if (indexMateri == 4)
            {
                UIStartBlanching();
            }
        }
    }


    void Update()
    {
        CaseMengukus_ControlBurnerOn();
        CaseMengukus_JetBurnerOn();
        CaseMengukus_ControlBurnerOff();
        CaseMengukus_JetBurnerOff();

        CaseSoup_ControlBurnerOn();
        CaseSoup_JetBurnerOn();
        CaseSoup_ControlBurnerOff();
        CaseSoup_JetBurnerOff();

        CaseRebus_ControlBurnerOn();
        CaseRebus_JetBurnerOn();
        CaseRebus_ControlBurnerOff();
        CaseRebus_JetBurnerOff();

        CasePoaching_ControlBurnerOn();
        CasePoaching_JetBurnerOn();
        CasePoaching_ControlBurnerOff();
        CasePoaching_JetBurnerOff();

        CaseBlanching_ControlBurnerOn();
        CaseBlanching_JetBurnerOn();
        CaseBlanching_ControlBurnerOff();
        CaseBlanching_JetBurnerOff();
    }

    void NarasiAwal() {

        indexMateri = 0;
        DisableUINarasi();
        ObjUiNarasi[0].SetActive(true);

        for (int i = 0; i < ObjParentMateri.Length; i++) {

            ObjParentMateri[i].SetActive(false);

        }

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

        JetBurnerKnob = ObjShapes[1].GetComponent<XRKnob>();
        BurnerKnob = ObjShapes[0].GetComponent<XRKnob>();

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

    public void NarasiFinal() {
        DisableUINarasi();
        ObjUiNarasi[12].SetActive(true);

    }

    #region Mengukus

    public void UIStartMengukus()
    {
        NarasiAwal();

        DisableUINarasi();
        ObjUiNarasi[1].SetActive(true);

        for (int i = 0; i < ObjParentMateri.Length; i++)
        {

            ObjParentMateri[i].SetActive(false);

        }
    }

    public void StartCaseMengukus() {

        DisableUINarasi();

        for (int i = 0; i < ObjShapesSiomay.Length; i++)
        {
            ObjShapesSiomay[i].SetActive(true);
        }

        ObjParentMateri[0].SetActive(true);
        indexMateri = 0;

        ObjShapes[2].SetActive(true);
        ObjShapes[3].SetActive(true);
        ObjShapes[4].SetActive(true);
        ObjShapes[5].SetActive(true);

        ObjSelection[7].SetActive(true);
        ObjSelection[8].SetActive(true);
        ObjSelection[10].SetActive(true);

        //aktifkan gelas
        ObjSocket[0].SetActive(true);
        OutlineOn(ObjShapes[2]);
        ObjSetColliderKinematic(ObjShapes[2], true, false);

        OutlineOff(ObjShapes[5]);
        ObjSetColliderKinematic(ObjShapes[5], false, true);

        ObjSelection[4].SetActive(false);
        ObjSelection[5].SetActive(false);
        JetBurnerKnob.Value = 1;
        BurnerKnob.Value = 1;

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
        for (int i = 0; i < ObjShapesSiomay.Length; i++) {
            OutlineOff(ObjShapesSiomay[i]);
        }

        for (int i = 0; i < ObjSocketSiomay.Length; i++) {
            OutlineOff(ObjSocketSiomay[i]);
            ObjSocketSiomay[i].GetComponent<Collider>().enabled = false;
        }

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

        OutlineOn(ObjShapes[0]);
        ObjSetColliderKinematic(ObjShapes[0], true, true);
        isControlBurnerOn = true;
    }


    void CaseMengukus_ControlBurnerOn()
    {
        if (indexMateri != 0) return;
        if (!isControlBurnerOn) return;
        if (BurnerKnob.Value <= 0.055f)
        {

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
        if (indexMateri != 0) return;
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
        yield return new WaitForSeconds(4f);

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
        if (indexMateri != 0) return;
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

    void CaseMengukus_ControlBurnerOff()
    {
        if (indexMateri != 0) return;
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
        ObjSetColliderKinematic(ObjShapes[5], true, true);

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
            ObjSocketSiomayMatang[i].GetComponentInChildren<Collider>().enabled = false;
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

        ObjSocketSiomayMatang[indexSiomayMatang].transform.GetChild(0).gameObject.SetActive(true);
        ObjSocketSiomayMatang[indexSiomayMatang].transform.GetChild(1).gameObject.SetActive(false);

        indexSiomayMatang++;

        if (indexSiomayMatang >= ObjShapesSiomayMatang.Length || isTest)
        {
            CaseMengukus_Finish();
            StartCoroutine(CaseMengukus_CapitanDisable());
        }
        else
        {

            OutlineOn(ObjShapesSiomayMatang[indexSiomayMatang]);
            ObjSetColliderKinematic(ObjShapesSiomayMatang[indexSiomayMatang], true, true);

            OutlineOn(ObjSocketSiomayMatang[indexSiomayMatang]);
            ObjSocketSiomayMatang[indexSiomayMatang].GetComponentInChildren<Collider>().enabled = true;

        }

    }

    IEnumerator CaseMengukus_CapitanDisable()
    {

        yield return new WaitForSeconds(2);
        ObjSelection[6].SetActive(false);
    }

    void CaseMengukus_Finish() {

        for (int i = 0; i < ObjShapesSiomayMatang.Length; i++)
        {
            ObjShapesSiomayMatang[i].SetActive(false);

            OutlineOff(ObjShapesSiomayMatang[i]);
            ObjSetColliderKinematic(ObjShapesSiomayMatang[i], false, true);
        }

        for (int i = 0; i < ObjSocketSiomayMatang.Length; i++)
        {
            //ObjSocketSiomayMatang[i].SetActive(false);

            ObjSocketSiomayMatang[i].transform.GetChild(0).gameObject.SetActive(true);
            ObjSocketSiomayMatang[i].transform.GetChild(1).gameObject.SetActive(false);
        }

        //ObjSelection[6].SetActive(false);
        ObjShapes[5].SetActive(false);
        ObjSetColliderKinematic(ObjShapes[5], false, true);

        DisableUINarasi();
        ObjUiNarasi[3].SetActive(true);

    }


    #endregion

    #region Simmering -  Soup
    public void UIStartSoup()
    {
        NarasiAwal();
        DisableUINarasi();
        ObjUiNarasi[4].SetActive(true);

        for (int i = 0; i < ObjParentMateri.Length; i++)
        {

            ObjParentMateri[i].SetActive(false);

        }
    }

    public void StartCaseSoup()
    {

        DisableUINarasi();

        ObjParentMateri[1].SetActive(true);
        indexMateri = 1;

        ObjSelection[0].SetActive(true);
        ObjSelection[1].SetActive(true);

        ObjShapes[6].SetActive(true);
        ObjShapes[7].SetActive(true);
        ObjShapes[8].SetActive(true);
        ObjShapes[9].SetActive(true);
        ObjShapes[10].SetActive(true);
        ObjShapes[11].SetActive(true);
        ObjShapes[12].SetActive(true);
        ObjShapes[13].SetActive(true);
        ObjShapes[14].SetActive(true);
        ObjShapes[15].SetActive(true);
        ObjShapes[16].SetActive(true);
        ObjShapes[17].SetActive(true);

        ObjSelection[10].SetActive(true);
        ObjSelection[11].SetActive(true);

        //aktifkan gelas
        ObjSocket[4].SetActive(true);
        OutlineOn(ObjShapes[16]);
        ObjSetColliderKinematic(ObjShapes[16], true, false);

        JetBurnerKnob.Value = 1;
        BurnerKnob.Value = 1;

    }

    public void CaseSoup_TutupPanci()
    {

        StartCoroutine(WaitSoupTutupPanci());

    }

    IEnumerator WaitSoupTutupPanci()
    {
        OutlineOff(ObjShapes[16]);
        ObjSelection[21].SetActive(true);
        yield return new WaitForSeconds(3f);

        ObjSelection[21].SetActive(false);
        ObjSocket[4].SetActive(false);
        ObjShapes[16].SetActive(false);
        ObjSelection[13].SetActive(true);

        //tutup panci
        ObjSocket[5].SetActive(true);
        OutlineOn(ObjShapes[6]);
        ObjSetColliderKinematic(ObjShapes[6], true, false);
    }


    public void CaseSoup_CekBurner()
    {
        //ObjSocket[2].SetActive(false);
        OutlineOff(ObjSocket[5]);
        ObjSocket[5].GetComponent<Collider>().enabled = false;

        OutlineOff(ObjShapes[6]);
        ObjSetColliderKinematic(ObjShapes[6], false, true);

        OutlineOn(ObjShapes[0]);
        ObjSetColliderKinematic(ObjShapes[0], true, true);
        isControlBurnerOn = true;
    }


    void CaseSoup_ControlBurnerOn()
    {
        if (indexMateri != 1) return;
        if (!isControlBurnerOn) return;
        if (BurnerKnob.Value <= 0.055f)
        {

            isControlBurnerOn = false;
            isJetBurnerOn = true;

            OutlineOff(ObjShapes[0]);
            ObjSetColliderKinematic(ObjShapes[0], false, true);

            OutlineOn(ObjShapes[1]);
            ObjSetColliderKinematic(ObjShapes[1], true, true);

            ObjSelection[4].SetActive(true);
        }


    }

    void CaseSoup_JetBurnerOn()
    {
        if (indexMateri != 1) return;
        if (!isJetBurnerOn) return;
        if (JetBurnerKnob.Value <= 0.5f)
        {

            isJetBurnerOn = false;
            OutlineOff(ObjShapes[1]);
            ObjSetColliderKinematic(ObjShapes[1], false, true);

            ObjSelection[5].SetActive(true);
            StartCoroutine(WaitAirSoupMatang());
        }


    }

    IEnumerator WaitAirSoupMatang()
    {
        ObjSelection[29].SetActive(true);
        yield return new WaitForSeconds(5f);
        ObjSelection[12].SetActive(true);
        yield return new WaitForSeconds(5f);
        ObjSelection[29].SetActive(false);

        //buka tutup
        ObjShapes[6].isStatic = false;
        OutlineOn(ObjShapes[6]);
        ObjSetColliderKinematic(ObjShapes[6], true, false);
        ObjSocket[5].SetActive(false);
        ObjSocket[6].SetActive(true);
    }


    public void CaseSoup_PilihKaldu() {

        OutlineOff(ObjShapes[6]);
        ObjSetColliderKinematic(ObjShapes[6], true, true);
        ObjSocket[6].SetActive(false);

        ObjSocket[9].SetActive(true);
        OutlineOn(ObjShapes[7]);
        ObjSetColliderKinematic(ObjShapes[7], true, false);
        OutlineOn(ObjShapes[8]);
        ObjSetColliderKinematic(ObjShapes[8], true, false);
        OutlineOn(ObjShapes[9]);
        ObjSetColliderKinematic(ObjShapes[9], true, false);

    }

    public void CaseSoup_SocketKaldu() {

        StartCoroutine(WaitSoupSocketKaldu());
    }

    IEnumerator WaitSoupSocketKaldu()
    {
        OutlineOff(ObjShapes[7]);
        OutlineOff(ObjShapes[8]);
        OutlineOff(ObjShapes[9]);
        ObjSelection[30].SetActive(true);
        yield return new WaitForSeconds(3f);
        ObjSelection[30].SetActive(false);
        ObjSelection[13].SetActive(false);
        ObjSocket[9].SetActive(false);


        ObjShapes[7].SetActive(false);
        ObjShapes[8].SetActive(false);
        ObjShapes[9].SetActive(false);

        ObjSelection[14].SetActive(true);

        CaseSoupLoyangSayur();
    }

    void CaseSoupLoyangSayur() {

        ObjSocket[10].SetActive(true);

        OutlineOn(ObjShapes[10]);
        ObjShapes[10].GetComponent<XRGrabInteractable>().colliders[0].enabled = true;
        ObjShapes[10].GetComponent<Rigidbody>().isKinematic = false;

        OutlineOn(ObjShapes[11]);
        ObjShapes[11].GetComponent<XRGrabInteractable>().colliders[0].enabled = true;
        ObjShapes[11].GetComponent<Rigidbody>().isKinematic = false;
        OutlineOn(ObjShapes[12]);
        ObjShapes[12].GetComponent<XRGrabInteractable>().colliders[0].enabled = true;
        ObjShapes[12].GetComponent<Rigidbody>().isKinematic = false;
        OutlineOn(ObjShapes[13]);
        ObjShapes[13].GetComponent<XRGrabInteractable>().colliders[0].enabled = true;
        ObjShapes[13].GetComponent<Rigidbody>().isKinematic = false;
        OutlineOn(ObjShapes[14]);
        ObjShapes[14].GetComponent<XRGrabInteractable>().colliders[0].enabled = true;
        ObjShapes[14].GetComponent<Rigidbody>().isKinematic = false;
        OutlineOn(ObjShapes[15]);
        ObjShapes[15].GetComponent<XRGrabInteractable>().colliders[0].enabled = true;
        ObjShapes[15].GetComponent<Rigidbody>().isKinematic = false;

    }

    int sayurMasuk = 0;
    public void CaseSoupSayurPanci() {
        sayurMasuk++;
        StartCoroutine(WaitSoupSocketSayurPanci());
    }

    IEnumerator WaitSoupSocketSayurPanci()
    {

        GameObject ObjSnapped = ObjSocket[10].GetComponent<SocketLockObject>().ObjSnapped;
        ObjSnapped.GetComponent<XRGrabInteractable>().colliders[0].enabled = false;

        Rigidbody[] rigidbodies = ObjSnapped.GetComponentsInChildren<Rigidbody>();

        for (int i = 0; i < rigidbodies.Length; i++)
        {
            if (rigidbodies[i].gameObject.name.ToLower().Contains("sayur")) {
                rigidbodies[i].isKinematic = false;
            }
        }

        OutlineOff(ObjSnapped);

        string objSnappedName = ObjSnapped.name;

        if (objSnappedName.ToLower().Contains("wortel"))
        {

            ObjSelection[15].SetActive(true);

        }
        else if (objSnappedName.ToLower().Contains("kubis"))
        {

            ObjSelection[16].SetActive(true);

        }
        else if (objSnappedName.ToLower().Contains("brokoli"))
        {

            ObjSelection[17].SetActive(true);

        }
        else if (objSnappedName.ToLower().Contains("kentang"))
        {

            ObjSelection[18].SetActive(true);

        }
        else if (objSnappedName.ToLower().Contains("buncis"))
        {

            ObjSelection[19].SetActive(true);

        }
        else if (objSnappedName.ToLower().Contains("kembang"))
        {

            ObjSelection[16].SetActive(true);

        }

        yield return new WaitForSeconds(2f);

        ObjSnapped.SetActive(false);
        ObjSocket[10].SetActive(false);

        yield return new WaitForSeconds(0.1f);

        ObjSocket[10].SetActive(true);
        OutlineOn(ObjSocket[10]);
        ObjSocket[10].GetComponent<Collider>().enabled = true;
        ObjSocket[10].GetComponent<XRSocketInteractor>().attachTransform.gameObject.SetActive(true);




        if (sayurMasuk >= 6)
        {
            yield return new WaitForSeconds(1.5f);
            ObjSocket[10].SetActive(false);
            CaseSoupTutupPanciMatang();

            //StartCoroutine(WaitSoupSayurMatang());
        }
    }


    void CaseSoupTutupPanciMatang() {

        ObjSocket[7].SetActive(true);
        OutlineOn(ObjShapes[6]);
        ObjSetColliderKinematic(ObjShapes[6], true, false);
        ObjShapes[6].isStatic = false;
    }
    public void CaseSoupPanciMatangTutup()
    {

        StartCoroutine(WaitSoupSayurMatang());

    }

    IEnumerator WaitSoupSayurMatang()
    {
        ObjSocket[7].SetActive(false);
        OutlineOff(ObjShapes[6]);
        ObjSelection[31].SetActive(true);
        yield return new WaitForSeconds(10f);
        ObjSelection[31].SetActive(false);
        //CaseSoupTutupPanciMatang();

        ObjSocket[8].SetActive(true);
        OutlineOn(ObjShapes[6]);
        ObjSetColliderKinematic(ObjShapes[6], true, false);
        ObjShapes[6].isStatic = false;
    }

    public void CaseSoupPanciMatangBuka()
    {
        OutlineOff(ObjShapes[6]);
        OutlineOn(ObjShapes[17]);
        ObjSetColliderKinematic(ObjShapes[17], true, true);

    }

    public void CaseSoupSendokSayurGrab() {

        ObjShapes[17].SetActive(false);
        ObjSelection[22].SetActive(true);
        ObjSelection[23].SetActive(true);
        ObjSelection[24].SetActive(false);
        ObjSelection[25].SetActive(false);
        ObjSelection[26].SetActive(false);
        ObjSelection[27].SetActive(false);
        ObjSelection[28].SetActive(false);

        ObjShapes[18].SetActive(true);
        ObjSetColliderKinematic(ObjShapes[18], true, true);
        OutlineOn(ObjShapes[18]);
        ObjSocket[11].SetActive(true);
        ObjSetColliderKinematic(ObjSocket[11], true, true);

    }

    int objDiSendok = 0;
    public void CaseSoupSendokGetObj() {
        ObjSelection[24].SetActive(true);
        if (objDiSendok == 0) {
            ObjShapes[18].SetActive(false);
        } else if (objDiSendok == 1)
        {
            ObjShapes[19].SetActive(false);
            ObjSocket[12].SetActive(true);
            ObjSetColliderKinematic(ObjSocket[12], true, true);
        } else if (objDiSendok == 2)
        {
            ObjShapes[20].SetActive(false);
            ObjSocket[13].SetActive(true);
            ObjSetColliderKinematic(ObjSocket[13], true, true);
        }

    }

    public void CaseSoupReleaseObj() {
        
        ObjSelection[24].SetActive(false);
        ObjSelection[25].SetActive(false);
        ObjSelection[26].SetActive(false);
        ObjSelection[27].SetActive(false);

        if (objDiSendok == 0)
        {
            ObjSocket[11].SetActive(false);
            ObjShapes[19].SetActive(true);
            ObjSetColliderKinematic(ObjShapes[19], true, true);
            OutlineOn(ObjShapes[19]);
            ObjSelection[27].SetActive(true);
            ObjSelection[28].SetActive(true);
        }
        else if (objDiSendok == 1)
        {
            ObjSocket[12].SetActive(false);
            ObjShapes[20].SetActive(true);
            ObjSetColliderKinematic(ObjShapes[20], true, true);
            OutlineOn(ObjShapes[20]);
            ObjSelection[26].SetActive(true);
        }
        else if (objDiSendok == 2)
        {
            ObjSocket[13].SetActive(false);
            ObjSelection[25].SetActive(true);
            //StartCoroutine(UISoupFinish());
            isJetBurnerOff = true;
            OutlineOn(ObjShapes[1]);
            ObjSetColliderKinematic(ObjShapes[1], true, true);
            StartCoroutine(CaseSoup_CapitanDisable());
        }

        objDiSendok++;
    }


    void CaseSoup_JetBurnerOff()
{
    if (indexMateri != 1) return;
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

void CaseSoup_ControlBurnerOff()
{
    if (indexMateri != 1) return;
    if (!isControlBurnerOff) return;
    if (BurnerKnob.Value >= 0.945f)
    {
        OutlineOff(ObjShapes[0]);
        ObjSetColliderKinematic(ObjShapes[0], false, true);

            ObjSelection[4].SetActive(false);

            StartCoroutine(UISoupFinish());
            isControlBurnerOff =false;
    }


}

    IEnumerator CaseSoup_CapitanDisable()
    {

        yield return new WaitForSeconds(2);
        ObjSelection[22].SetActive(false);
    }


    IEnumerator UISoupFinish()
    {

        
        yield return new WaitForSeconds(2);
        DisableUINarasi();
        ObjUiNarasi[5].SetActive(true);

        ObjSelection[22].SetActive(false);
    }



    #endregion

    #region Merebus

    public void UIStartMerebus()
    {
        NarasiAwal();
        DisableUINarasi();
        ObjUiNarasi[6].SetActive(true);

        for (int i = 0; i < ObjParentMateri.Length; i++)
        {

            ObjParentMateri[i].SetActive(false);

        }
    }

    public void StartCaseMerebus()
    {

        DisableUINarasi();

        ObjParentMateri[2].SetActive(true);
        indexMateri = 2;

        ObjSelection[0].SetActive(true);
        ObjSelection[1].SetActive(true);

        ObjShapes[21].SetActive(true);
        ObjShapes[22].SetActive(true);
        ObjShapes[23].SetActive(true);
        ObjShapes[24].SetActive(true);
        ObjShapes[25].SetActive(true);
        ObjShapes[26].SetActive(true);
        ObjShapes[27].SetActive(true);

        ObjSelection[10].SetActive(true);
        ObjSelection[11].SetActive(true);

        ObjSetColliderKinematic(ObjShapes[22], true, true);

        //aktifkan gelas
        ObjSocket[14].SetActive(true);
        OutlineOn(ObjShapes[23]);
        ObjSetColliderKinematic(ObjShapes[23], true, false);

        JetBurnerKnob.Value = 1;
        BurnerKnob.Value = 1;

    }

    public void CaseRebus_TutupPanci()
    {

        StartCoroutine(WaitRebusTutupPanci());

    }

    IEnumerator WaitRebusTutupPanci()
    {
        OutlineOff(ObjShapes[23]);
        ObjSelection[37].SetActive(true);
        yield return new WaitForSeconds(3f);

        ObjSelection[37].SetActive(false);
        ObjSocket[14].SetActive(false);
        ObjShapes[23].SetActive(false);
        ObjSelection[32].SetActive(true);

        //tutup panci
        ObjSocket[16].SetActive(true);
        OutlineOn(ObjShapes[21]);
        ObjSetColliderKinematic(ObjShapes[21], true, false);
    }

    public void CaseRebus_CekBurner()
    {
        //ObjSocket[2].SetActive(false);
        OutlineOff(ObjSocket[16]);
        ObjSocket[16].GetComponent<Collider>().enabled = false;

        OutlineOff(ObjShapes[21]);
        ObjSetColliderKinematic(ObjShapes[21], false, true);

        OutlineOn(ObjShapes[0]);
        ObjSetColliderKinematic(ObjShapes[0], true, true);
        isControlBurnerOn = true;
    }


    void CaseRebus_ControlBurnerOn()
    {
        if (indexMateri != 2) return;
        if (!isControlBurnerOn) return;
        if (BurnerKnob.Value <= 0.055f)
        {

            isControlBurnerOn = false;
            isJetBurnerOn = true;

            OutlineOff(ObjShapes[0]);
            ObjSetColliderKinematic(ObjShapes[0], false, true);

            OutlineOn(ObjShapes[1]);
            ObjSetColliderKinematic(ObjShapes[1], true, true);

            ObjSelection[4].SetActive(true);
        }


    }

    void CaseRebus_JetBurnerOn()
    {
        if (indexMateri != 2) return;
        if (!isJetBurnerOn) return;
        if (JetBurnerKnob.Value <= 0.055f)
        {

            isJetBurnerOn = false;
            OutlineOff(ObjShapes[1]);
            ObjSetColliderKinematic(ObjShapes[1], false, true);

            ObjSelection[5].SetActive(true);
            StartCoroutine(WaitAirRebusMatang());
        }


    }

    IEnumerator WaitAirRebusMatang()
    {
        ObjSelection[38].SetActive(true);
        yield return new WaitForSeconds(5f);
        ObjSelection[33].SetActive(true);
        yield return new WaitForSeconds(5f);
        ObjSelection[38].SetActive(false);

        //buka tutup
        OutlineOn(ObjShapes[21]);
        ObjSetColliderKinematic(ObjShapes[21], true, false);
        ObjShapes[21].isStatic = false;
        ObjSocket[16].SetActive(false);
        ObjSocket[18].SetActive(true);
    }

    public void CaseRebusLoyangSayur() {

        OutlineOff(ObjShapes[21]);

        OutlineOn(ObjShapes[24]);
        ObjShapes[24].GetComponent<XRGrabInteractable>().colliders[0].enabled = true;
        ObjShapes[24].GetComponent<Rigidbody>().isKinematic = false;

        OutlineOn(ObjShapes[25]);
        ObjShapes[25].GetComponent<XRGrabInteractable>().colliders[0].enabled = true;
        ObjShapes[25].GetComponent<Rigidbody>().isKinematic = false;

        OutlineOn(ObjShapes[26]);
        ObjShapes[26].GetComponent<XRGrabInteractable>().colliders[0].enabled = true;
        ObjShapes[26].GetComponent<Rigidbody>().isKinematic = false;

        ObjSocket[15].SetActive(true);
    
    }

    int sayurRebusMasuk = 0;
    public void CaseRebusSayurPanci()
    {
        sayurRebusMasuk++;
        StartCoroutine(WaitRebusSocketSayurPanci());
    }

    IEnumerator WaitRebusSocketSayurPanci()
    {

        GameObject ObjSnapped = ObjSocket[15].GetComponent<SocketLockObject>().ObjSnapped;
        ObjSnapped.GetComponent<XRGrabInteractable>().colliders[0].enabled = false;

        Rigidbody[] rigidbodies = ObjSnapped.GetComponentsInChildren<Rigidbody>();

        for (int i = 0; i < rigidbodies.Length; i++)
        {
            if (rigidbodies[i].gameObject.name.ToLower().Contains("sayur"))
            {
                rigidbodies[i].isKinematic = false;
            }
        }

        OutlineOff(ObjSnapped);

        string objSnappedName = ObjSnapped.name;

        if (objSnappedName.ToLower().Contains("wortel"))
        {

            ObjSelection[34].SetActive(true);

        }

        else if (objSnappedName.ToLower().Contains("brokoli"))
        {

            ObjSelection[35].SetActive(true);

        }

        else if (objSnappedName.ToLower().Contains("kembang"))
        {

            ObjSelection[36].SetActive(true);

        }

        yield return new WaitForSeconds(2f);

        ObjSnapped.SetActive(false);
        ObjSocket[15].SetActive(false);

        yield return new WaitForSeconds(0.1f);

        ObjSocket[15].SetActive(true);
        OutlineOn(ObjSocket[15]);
        ObjSocket[15].GetComponent<Collider>().enabled = true;
        ObjSocket[15].GetComponent<XRSocketInteractor>().attachTransform.gameObject.SetActive(true);

        if (sayurRebusMasuk >= 3)
        {
            yield return new WaitForSeconds(1.5f);
            ObjSocket[15].SetActive(false);
            CaseRebusTutupPanciMatang();
        }
    }

    void CaseRebusTutupPanciMatang() {

        ObjSocket[17].SetActive(true);
        ObjSocket[18].SetActive(false);
        OutlineOn(ObjShapes[21]);
        ObjSetColliderKinematic(ObjShapes[21], true, false);
        ObjShapes[21].isStatic = false;

    }

    public void CaseRebusPanciMatangTutup()
    {

        StartCoroutine(WaitRebusSayurMatang());

    }

    IEnumerator WaitRebusSayurMatang()
    {
        OutlineOff(ObjShapes[21]);
        ObjSelection[39].SetActive(true);
        yield return new WaitForSeconds(10f);
        ObjSelection[39].SetActive(false);

        ObjSocket[17].SetActive(false);
        ObjSocket[19].SetActive(true);
        OutlineOn(ObjShapes[21]);
        ObjSetColliderKinematic(ObjShapes[21], true, false);
        ObjShapes[21].isStatic = false;
    }

    public void CaseRebbusPanciMatangBuka()
    {
        OutlineOff(ObjShapes[21]);
        OutlineOn(ObjShapes[27]);
        ObjSetColliderKinematic(ObjShapes[27], true, true);

    }

    public void CaseRebusSendokSayurGrab()
    {

        ObjShapes[27].SetActive(false);
        ObjSelection[40].SetActive(true);
        ObjSelection[42].SetActive(true);
        ObjSelection[41].SetActive(false);
        ObjSelection[43].SetActive(false);
        ObjSelection[44].SetActive(false);
        ObjSelection[45].SetActive(false);
        ObjSelection[46].SetActive(false);

        ObjShapes[28].SetActive(true);
        ObjSetColliderKinematic(ObjShapes[28], true, true);
        OutlineOn(ObjShapes[28]);
        ObjSocket[20].SetActive(true);
        ObjSetColliderKinematic(ObjSocket[20], true, true);

    }

    int objDiSendokRebus = 0;
    public void CaseRebusSendokGetObj()
    {
        ObjSelection[41].SetActive(true);
        if (objDiSendokRebus == 0)
        {
            ObjShapes[28].SetActive(false);
        }
        else if (objDiSendokRebus == 1)
        {
            ObjShapes[29].SetActive(false);
            ObjSocket[21].SetActive(true);
            ObjSetColliderKinematic(ObjSocket[21], true, true);
        }
        else if (objDiSendokRebus == 2)
        {
            ObjShapes[30].SetActive(false);
            ObjSocket[22].SetActive(true);
            ObjSetColliderKinematic(ObjSocket[22], true, true);
        }

    }

    public void CaseRebusReleaseObj()
    {

        ObjSelection[41].SetActive(false);
        ObjSelection[43].SetActive(false);
        ObjSelection[44].SetActive(false);
        ObjSelection[45].SetActive(false);

        if (objDiSendokRebus == 0)
        {
            ObjSocket[20].SetActive(false);
            ObjShapes[29].SetActive(true);
            ObjSetColliderKinematic(ObjShapes[29], true, true);
            OutlineOn(ObjShapes[29]);
            ObjSelection[45].SetActive(true);
            ObjSelection[46].SetActive(true);
        }
        else if (objDiSendokRebus == 1)
        {
            ObjSocket[21].SetActive(false);
            ObjShapes[30].SetActive(true);
            ObjSetColliderKinematic(ObjShapes[30], true, true);
            OutlineOn(ObjShapes[30]);
            ObjSelection[44].SetActive(true);
        }
        else if (objDiSendokRebus == 2)
        {
            ObjSocket[22].SetActive(false);
            ObjSelection[43].SetActive(true);
            //StartCoroutine(UIRebusFinish());
            isJetBurnerOff = true;
            OutlineOn(ObjShapes[1]);
            ObjSetColliderKinematic(ObjShapes[1], true, true);
            StartCoroutine(CaseRebus_CapitanDisable());
        }

        objDiSendokRebus++;
    }


    void CaseRebus_JetBurnerOff()
    {
        if (indexMateri != 2) return;
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

    void CaseRebus_ControlBurnerOff()
    {
        if (indexMateri != 2) return;
        if (!isControlBurnerOff) return;
        if (BurnerKnob.Value >= 0.945f)
        {
            OutlineOff(ObjShapes[0]);
            ObjSetColliderKinematic(ObjShapes[0], false, true);
            ObjSelection[4].SetActive(false);
            isControlBurnerOff = false;

            StartCoroutine(UIRebusFinish());
        }


    }

    IEnumerator CaseRebus_CapitanDisable()
    {

        yield return new WaitForSeconds(2);
        ObjSelection[40].SetActive(false);
    }


    IEnumerator UIRebusFinish()
        {


            yield return new WaitForSeconds(2);
        DisableUINarasi();
        ObjUiNarasi[7].SetActive(true);
        ObjSelection[40].SetActive(false);
        }

    #endregion

    #region Poaching

    public void UIStartPoaching()
    {
        NarasiAwal();
        DisableUINarasi();
        ObjUiNarasi[8].SetActive(true);

        for (int i = 0; i < ObjParentMateri.Length; i++)
        {

            ObjParentMateri[i].SetActive(false);

        }
    }

    public void StartCasePoaching()
    {
        DisableUINarasi();

        ObjParentMateri[3].SetActive(true);
        indexMateri = 3;

        ObjSelection[0].SetActive(true);
        ObjSelection[1].SetActive(true);

        ObjShapes[45].SetActive(true);
        ObjShapes[46].SetActive(true);
        ObjShapes[47].SetActive(true);
        ObjShapes[48].SetActive(true);
        ObjShapes[49].SetActive(true);


        ObjSelection[10].SetActive(true);

        ObjSetColliderKinematic(ObjShapes[45], true, true);

        //aktifkan gelas
        ObjSocket[35].SetActive(true);
        OutlineOn(ObjShapes[47]);
        ObjSetColliderKinematic(ObjShapes[47], true, false);

        JetBurnerKnob.Value = 1;
        BurnerKnob.Value = 1;
    }

    public void CasePouching_TutupPanci()
    {

        StartCoroutine(WaitPouchingTutupPanci());

    }

    IEnumerator WaitPouchingTutupPanci()
    {
        OutlineOff(ObjShapes[47]);
        ObjSelection[71].SetActive(true);
        yield return new WaitForSeconds(3f);

        ObjSelection[71].SetActive(false);
        ObjSocket[35].SetActive(false);
        ObjShapes[47].SetActive(false);
        ObjSelection[66].SetActive(true);

        //tutup panci
        ObjSocket[38].SetActive(true);
        OutlineOn(ObjShapes[46]);
        ObjSetColliderKinematic(ObjShapes[46], true, false);
    }

    public void CasePouching_CekBurner()
    {

        OutlineOff(ObjSocket[38]);
        ObjSocket[38].GetComponent<Collider>().enabled = false;

        OutlineOff(ObjShapes[46]);
        ObjSetColliderKinematic(ObjShapes[46], false, true);

        OutlineOn(ObjShapes[0]);
        ObjSetColliderKinematic(ObjShapes[0], true, true);
        isControlBurnerOn = true;
    }


    void CasePoaching_ControlBurnerOn()
    {
        if (indexMateri != 3) return;
        if (!isControlBurnerOn) return;
        if (BurnerKnob.Value <= 0.055f)
        {

            isControlBurnerOn = false;
            isJetBurnerOn = true;

            OutlineOff(ObjShapes[0]);
            ObjSetColliderKinematic(ObjShapes[0], false, true);

            OutlineOn(ObjShapes[1]);
            ObjSetColliderKinematic(ObjShapes[1], true, true);

            ObjSelection[4].SetActive(true);
        }


    }

    void CasePoaching_JetBurnerOn()
    {
        if (indexMateri != 3) return;
        if (!isJetBurnerOn) return;
        if (JetBurnerKnob.Value <= 0.55f)
        {

            isJetBurnerOn = false;
            OutlineOff(ObjShapes[1]);
            ObjSetColliderKinematic(ObjShapes[1], false, true);

            ObjSelection[5].SetActive(true);
            StartCoroutine(WaitAirPoachingMatang());
        }


    }

    IEnumerator WaitAirPoachingMatang()
    {
        ObjSelection[72].SetActive(true);
        yield return new WaitForSeconds(5f);
        ObjSelection[67].SetActive(true);
        yield return new WaitForSeconds(5f);
        ObjSelection[72].SetActive(false);

        //buka tutup
        OutlineOn(ObjShapes[46]);
        ObjSetColliderKinematic(ObjShapes[46], true, false);
        ObjShapes[46].isStatic = false;
        ObjSocket[38].SetActive(false);
        ObjSocket[40].SetActive(true);
    }

    public void CasePoachingPecahTelur()
    {

        OutlineOff(ObjShapes[46]);

        OutlineOn(ObjShapes[48]);
        ObjSetColliderKinematic(ObjShapes[48], true, false);

        ObjSocket[36].SetActive(true);


    }

    public void CasePoachingTelurPecahAnim()
    {
        StartCoroutine(WaitPoachingAnimPecahTelur());
    }

    IEnumerator WaitPoachingAnimPecahTelur()
    {
        ObjShapes[48].SetActive(false);
        ObjSocket[36].SetActive(false);
        ObjSelection[70].SetActive(true);
        yield return new WaitForSeconds(4);
        OutlineOn(ObjShapes[49]);
        ObjSetColliderKinematic(ObjShapes[49], true, true);

    }

    public void CasePoachingGrabSendok() {
        if (isTelurMatang) return;

        OutlineOff(ObjShapes[49]);
        ObjSetColliderKinematic(ObjShapes[49], false, true);
        ObjShapes[49].SetActive(false);

        ObjSelection[75].SetActive(true);
        ObjSelection[76].SetActive(true);
        
        ObjSelection[77].SetActive(false);

        ObjSocket[37].SetActive(true);

    }

    bool isTelurMatang = false;
    public void CasePoachingSendokGetObj()
    {
        if (isTelurMatang) return; 
        Debug.Log("CasePoachingSendokGetObj");
        ObjSelection[70].SetActive(false);
        ObjSelection[76].SetActive(false);
        ObjSocket[37].SetActive(false);

        ObjSelection[68].SetActive(true);


        StartCoroutine(WaitPoachingTelurMatang());
    }

    IEnumerator WaitPoachingTelurMatang()
    {
        ObjSelection[73].SetActive(true);

        yield return new WaitForSeconds(5f);

        ObjSelection[68].SetActive(false);
        ObjSelection[69].SetActive(true);

        yield return new WaitForSeconds(5f);

        ObjSelection[73].SetActive(false);
        ObjSelection[75].GetComponentInChildren<CapitanMgr>().ResetObj();
        ObjSocket[43].SetActive(true);

        isTelurMatang = true;

    }

    public void CasePoachingMatangSendokGetObj()
    {
        if (!isTelurMatang) return;
        Debug.Log("CasePoachingMatangSendokGetObj");
        ObjSelection[68].SetActive(false);
        ObjSelection[69].SetActive(false);
        ObjSelection[70].SetActive(false);
        ObjSelection[76].SetActive(false);
        ObjSocket[43].SetActive(false);

        ObjSocket[42].SetActive(true);
        ObjSelection[77].SetActive(true);

    }

    public void CasePoachingMatangSendokReleaseObj()
    {
        if (!isTelurMatang) return;
        Debug.Log("CasePoachingMatangSendokReleaseObj");
        ObjSocket[42].SetActive(false);
        ObjSelection[77].SetActive(false);
        ObjSelection[74].SetActive(true);

        isJetBurnerOff = true;
        OutlineOn(ObjShapes[1]);
        ObjSetColliderKinematic(ObjShapes[1], true, true);
        StartCoroutine(CasePoaching_SendokDisable());

    }

    void CasePoaching_JetBurnerOff()
    {
        if (indexMateri != 3) return;
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

    void CasePoaching_ControlBurnerOff()
    {
        if (indexMateri != 3) return;
        if (!isControlBurnerOff) return;
        if (BurnerKnob.Value >= 0.945f)
        {
            OutlineOff(ObjShapes[0]);
            ObjSetColliderKinematic(ObjShapes[0], false, true);
            ObjSelection[4].SetActive(false);
            isControlBurnerOff = false;

            StartCoroutine(UIPoachingFinish());
        }


    }

    IEnumerator CasePoaching_SendokDisable()
    {

        yield return new WaitForSeconds(2);
        ObjSelection[75].SetActive(false);
        ObjSelection[70].SetActive(false);
        ObjShapes[49].SetActive(true);

    }


    IEnumerator UIPoachingFinish()
    {

        yield return new WaitForSeconds(2);
        DisableUINarasi();
        ObjUiNarasi[9].SetActive(true);
        ObjSelection[75].SetActive(false);
    }

#endregion

    #region Blanching

    public void UIStartBlanching()
    {
        NarasiAwal();
        DisableUINarasi();
        ObjUiNarasi[10].SetActive(true);

        for (int i = 0; i < ObjParentMateri.Length; i++)
        {

            ObjParentMateri[i].SetActive(false);

        }
    }

    public void StartCaseBlanching()
    {

        DisableUINarasi();

        ObjParentMateri[4].SetActive(true);
        indexMateri = 4;

        ObjSelection[0].SetActive(true);
        ObjSelection[1].SetActive(true);

        ObjShapes[31].SetActive(true);
        ObjShapes[32].SetActive(true);
        ObjShapes[33].SetActive(true);
        ObjShapes[34].SetActive(true);
        ObjShapes[35].SetActive(true);
        ObjShapes[36].SetActive(true);
        ObjShapes[37].SetActive(true);
        ObjShapes[38].SetActive(true);

        ObjSelection[10].SetActive(true);

        ObjSetColliderKinematic(ObjShapes[32], true, true);

        //aktifkan gelas
        ObjSocket[23].SetActive(true);
        OutlineOn(ObjShapes[38]);
        ObjSetColliderKinematic(ObjShapes[38], true, false);

        JetBurnerKnob.Value = 1;
        BurnerKnob.Value = 1;

    }

    public void CaseBlanching_TutupPanci()
    {

        StartCoroutine(WaitBlanchingTutupPanci());

    }

    IEnumerator WaitBlanchingTutupPanci()
    {
        OutlineOff(ObjShapes[38]);
        ObjSelection[53].SetActive(true);
        yield return new WaitForSeconds(3f);

        ObjSelection[53].SetActive(false);
        ObjSocket[23].SetActive(false);
        ObjShapes[38].SetActive(false);
        ObjSelection[47].SetActive(true);

        //tutup panci
        ObjSocket[25].SetActive(true);
        OutlineOn(ObjShapes[31]);
        ObjSetColliderKinematic(ObjShapes[31], true, false);
    }

    public void CaseBlanching_CekBurner()
    {
        //ObjSocket[2].SetActive(false);
        OutlineOff(ObjSocket[25]);
        ObjSocket[25].GetComponent<Collider>().enabled = false;

        OutlineOff(ObjShapes[31]);
        ObjSetColliderKinematic(ObjShapes[31], false, true);

        OutlineOn(ObjShapes[0]);
        ObjSetColliderKinematic(ObjShapes[0], true, true);
        isControlBurnerOn = true;
    }


    void CaseBlanching_ControlBurnerOn()
    {
        if (indexMateri != 4) return;
        if (!isControlBurnerOn) return;
        if (BurnerKnob.Value <= 0.055f)
        {

            isControlBurnerOn = false;
            isJetBurnerOn = true;

            OutlineOff(ObjShapes[0]);
            ObjSetColliderKinematic(ObjShapes[0], false, true);

            OutlineOn(ObjShapes[1]);
            ObjSetColliderKinematic(ObjShapes[1], true, true);

            ObjSelection[4].SetActive(true);
        }


    }

    void CaseBlanching_JetBurnerOn()
    {
        if (indexMateri != 4) return;
        if (!isJetBurnerOn) return;
        if (JetBurnerKnob.Value <= 0.055f)
        {

            isJetBurnerOn = false;
            OutlineOff(ObjShapes[1]);
            ObjSetColliderKinematic(ObjShapes[1], false, true);

            ObjSelection[5].SetActive(true);
            StartCoroutine(WaitAirBlanchingMatang());
        }


    }

    IEnumerator WaitAirBlanchingMatang()
    {
        ObjSelection[54].SetActive(true);
        yield return new WaitForSeconds(5f);
        ObjSelection[48].SetActive(true);
        yield return new WaitForSeconds(5f);
        ObjSelection[54].SetActive(false);

        //buka tutup
        OutlineOn(ObjShapes[31]);
        ObjSetColliderKinematic(ObjShapes[31], true, false);
        ObjShapes[31].isStatic = false;
        ObjSocket[25].SetActive(false);
        ObjSocket[27].SetActive(true);
    }

    public void CaseBlanchingLoyangSayur()
    {

        OutlineOff(ObjShapes[31]);

        OutlineOn(ObjShapes[33]);
        ObjShapes[33].GetComponent<XRGrabInteractable>().colliders[0].enabled = true;
        ObjShapes[33].GetComponent<Rigidbody>().isKinematic = false;

        OutlineOn(ObjShapes[34]);
        ObjShapes[34].GetComponent<XRGrabInteractable>().colliders[0].enabled = true;
        ObjShapes[34].GetComponent<Rigidbody>().isKinematic = false;

        OutlineOn(ObjShapes[35]);
        ObjShapes[35].GetComponent<XRGrabInteractable>().colliders[0].enabled = true;
        ObjShapes[35].GetComponent<Rigidbody>().isKinematic = false;

        OutlineOn(ObjShapes[36]);
        ObjShapes[36].GetComponent<XRGrabInteractable>().colliders[0].enabled = true;
        ObjShapes[36].GetComponent<Rigidbody>().isKinematic = false;

        ObjSocket[24].SetActive(true);

    }

    int sayurBlanchingMasuk = 0;
    public void CaseBlanchingSayurPanci()
    {
        sayurBlanchingMasuk++;
        StartCoroutine(WaitBlanchingSocketSayurPanci());
    }

    IEnumerator WaitBlanchingSocketSayurPanci()
    {

        GameObject ObjSnapped = ObjSocket[24].GetComponent<SocketLockObject>().ObjSnapped;
        ObjSnapped.GetComponent<XRGrabInteractable>().colliders[0].enabled = false;

        Rigidbody[] rigidbodies = ObjSnapped.GetComponentsInChildren<Rigidbody>();

        for (int i = 0; i < rigidbodies.Length; i++)
        {
            if (rigidbodies[i].gameObject.name.ToLower().Contains("sayur"))
            {
                rigidbodies[i].isKinematic = false;
            }
        }

        OutlineOff(ObjSnapped);

        string objSnappedName = ObjSnapped.name;

        if (objSnappedName.ToLower().Contains("buncis"))
        {

            ObjSelection[49].SetActive(true);

        }
        else if (objSnappedName.ToLower().Contains("brokoli"))
        {

            ObjSelection[50].SetActive(true);

        }else 
        if (objSnappedName.ToLower().Contains("wortel"))
        {

            ObjSelection[51].SetActive(true);

        }

        else if (objSnappedName.ToLower().Contains("kembang"))
        {

            ObjSelection[52].SetActive(true);

        }

        yield return new WaitForSeconds(2f);

        ObjSnapped.SetActive(false);
        ObjSocket[24].SetActive(false);

        yield return new WaitForSeconds(0.1f);

        ObjSocket[24].SetActive(true);
        OutlineOn(ObjSocket[24]);
        ObjSocket[24].GetComponent<Collider>().enabled = true;
        ObjSocket[24].GetComponent<XRSocketInteractor>().attachTransform.gameObject.SetActive(true);

        if (sayurBlanchingMasuk >= 4)
        {
            yield return new WaitForSeconds(1.5f);
            ObjSocket[24].SetActive(false);
            CaseBlanchingTutupPanciMatang();
        }
    }

    void CaseBlanchingTutupPanciMatang()
    {

        ObjSocket[26].SetActive(true);
        ObjSocket[27].SetActive(false);
        OutlineOn(ObjShapes[31]);
        ObjSetColliderKinematic(ObjShapes[31], true, false);
        ObjShapes[31].isStatic = false;

    }

    public void CaseBlanchingPanciMatangTutup()
    {

        StartCoroutine(WaitBlanchingSayurMatang());

    }

    IEnumerator WaitBlanchingSayurMatang()
    {
        OutlineOff(ObjShapes[31]);
        ObjSelection[55].SetActive(true);
        yield return new WaitForSeconds(10f);
        ObjSelection[55].SetActive(false);

        ObjSocket[26].SetActive(false);
        ObjSocket[28].SetActive(true);
        OutlineOn(ObjShapes[31]);
        ObjSetColliderKinematic(ObjShapes[31], true, false);
        ObjShapes[31].isStatic = false;
    }

    public void CaseBlanchingPanciMatangBuka()
    {
        OutlineOff(ObjShapes[31]);
        OutlineOn(ObjShapes[37]);
        ObjSetColliderKinematic(ObjShapes[37], true, true);

    }

    public void CaseBlanchingSendokSayurGrab()
    {

        ObjShapes[37].SetActive(false);
        ObjSelection[56].SetActive(true);
        ObjSelection[58].SetActive(true);
        ObjSelection[59].SetActive(true);

        ObjSelection[57].SetActive(false);
        ObjSelection[60].SetActive(false);
        ObjSelection[61].SetActive(false);
        ObjSelection[62].SetActive(false);
        ObjSelection[46].SetActive(false);

        ObjShapes[39].SetActive(true);
        ObjSetColliderKinematic(ObjShapes[39], true, true);
        OutlineOn(ObjShapes[39]);
        ObjSocket[29].SetActive(true);
        ObjSetColliderKinematic(ObjSocket[29], true, true);

    }

    int objDiSendokBlanching = 0;
    bool isSendokDariEs = false;
    public void CaseBlanchingSendokGetObj()
    {
        if (isSendokDariEs || objDiSendokBlanching > 2) return;

        ObjSelection[57].SetActive(true);
        if (objDiSendokBlanching == 0)
        {
            ObjShapes[39].SetActive(false);
        }
        else if (objDiSendokBlanching == 1)
        {
            ObjShapes[40].SetActive(false);
            ObjSocket[30].SetActive(true);
            ObjSetColliderKinematic(ObjSocket[30], true, true);
        }
        else if (objDiSendokBlanching == 2)
        {
            ObjShapes[41].SetActive(false);
            ObjSocket[31].SetActive(true);
            ObjSetColliderKinematic(ObjSocket[31], true, true);
        }

    }

    public void CaseBlanchingReleaseObj()
    {
        if (isSendokDariEs || objDiSendokBlanching >2) return;

        ObjSelection[57].SetActive(false);

        ObjSelection[60].SetActive(false);
        ObjSelection[61].SetActive(false);
        ObjSelection[62].SetActive(false);

        if (objDiSendokBlanching == 0)
        {
            ObjSocket[29].SetActive(false);
            ObjShapes[40].SetActive(true);
            ObjSetColliderKinematic(ObjShapes[40], true, true);
            OutlineOn(ObjShapes[40]);
            ObjSelection[62].SetActive(true);
        }
        else if (objDiSendokBlanching == 1)
        {
            ObjSocket[30].SetActive(false);
            ObjShapes[41].SetActive(true);
            ObjSetColliderKinematic(ObjShapes[41], true, true);
            OutlineOn(ObjShapes[41]);
            ObjSelection[61].SetActive(true);
        }
        else if (objDiSendokBlanching == 2)
        {
            ObjSocket[31].SetActive(false);
            ObjSelection[60].SetActive(true);
            

            StartCoroutine(WaitCaseBlanchingEs());
        }

        //if (!isSendokDariEs)
        objDiSendokBlanching++;
    }

    IEnumerator WaitCaseBlanchingEs()
    {

        yield return new WaitForSeconds(3);
        ObjShapes[42].SetActive(true);
        ObjSetColliderKinematic(ObjShapes[42], true, true);
        OutlineOn(ObjShapes[42]);
        ObjSocket[32].SetActive(true);
        ObjSetColliderKinematic(ObjSocket[32], true, true);

        isSendokDariEs = true;
        objDiSendokBlanching = 0;
    }

    public void CaseBlanchingSendokGetObjEs()
    {
        if (!isSendokDariEs) return;

        ObjSelection[57].SetActive(true);
        if (objDiSendokBlanching == 0)
        {
            ObjShapes[42].SetActive(false);

            ObjSelection[60].SetActive(false);
            ObjSelection[61].SetActive(true);
        }
        else if (objDiSendokBlanching == 1)
        {
            ObjShapes[43].SetActive(false);
            ObjSocket[33].SetActive(true);
            ObjSetColliderKinematic(ObjSocket[33], true, true);

            ObjSelection[61].SetActive(false);
            ObjSelection[62].SetActive(true);
        }
        else if (objDiSendokBlanching == 2)
        {
            ObjShapes[44].SetActive(false);
            ObjSocket[34].SetActive(true);
            ObjSelection[62].SetActive(false);
            ObjSetColliderKinematic(ObjSocket[34], true, true);
        }

    }

    public void CaseBlanchingReleaseObjEs()
    {
        if (!isSendokDariEs) return;

        ObjSelection[57].SetActive(false);

        ObjSelection[63].SetActive(false);
        ObjSelection[64].SetActive(false);
        ObjSelection[65].SetActive(false);

        if (objDiSendokBlanching == 0)
        {
            ObjSocket[32].SetActive(false);
            ObjShapes[43].SetActive(true);
            ObjSetColliderKinematic(ObjShapes[43], true, true);
            OutlineOn(ObjShapes[43]);
            ObjSelection[65].SetActive(true);
        }
        else if (objDiSendokBlanching == 1)
        {
            ObjSocket[33].SetActive(false);
            ObjShapes[44].SetActive(true);
            ObjSetColliderKinematic(ObjShapes[44], true, true);
            OutlineOn(ObjShapes[44]);
            ObjSelection[64].SetActive(true);
        }
        else if (objDiSendokBlanching == 2)
        {
            ObjSocket[34].SetActive(false);
            ObjSelection[63].SetActive(true);

            isJetBurnerOff = true;
            OutlineOn(ObjShapes[1]);
            ObjSetColliderKinematic(ObjShapes[1], true, true);
            StartCoroutine(CaseBlanching_CapitanDisable());
        }

            objDiSendokBlanching++;
    }

    void CaseBlanching_JetBurnerOff()
    {
        if (indexMateri != 4) return;
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

    void CaseBlanching_ControlBurnerOff()
    {
        if (indexMateri != 4) return;
        if (!isControlBurnerOff) return;
        if (BurnerKnob.Value >= 0.945f)
        {
            OutlineOff(ObjShapes[0]);
            ObjSetColliderKinematic(ObjShapes[0], false, true);
            ObjSelection[4].SetActive(false);
            isControlBurnerOff = false;

            StartCoroutine(UIBlanchingFinish());
        }


    }
    IEnumerator CaseBlanching_CapitanDisable()
    {

        yield return new WaitForSeconds(2);
        ObjSelection[56].SetActive(false);
    }


    IEnumerator UIBlanchingFinish()
    {
        yield return new WaitForSeconds(2);
        DisableUINarasi();
        ObjUiNarasi[11].SetActive(true);
        ObjSelection[56].SetActive(false);
    }


    #endregion

}
