using ITISKIRUHERE;
using MikeNspired.XRIStarterKit;
using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SceneTeknikPanasBasah : MonoBehaviour
{
    public bool isTest = false;

    int indexMateri = 0;
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
        NarasiAwal();
    }

    // Update is called once per frame
    void Update()
    {
        CaseMengukus_ControlBurnerOn();
        CaseMengukus_JetBurnerOn();
        CaseMengukus_ControlBurnerOff();
        CaseMengukus_JetBurnerOff();

        CaseSoup_ControlBurnerOn();
        CaseSoup_JetBurnerOn();
        //CaseMengukus_ControlBurnerOff();
        //CaseMengukus_JetBurnerOff();
    }

    void NarasiAwal() {

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

        ObjSelection[6].SetActive(false);
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
        if (JetBurnerKnob.Value <= 0.055f)
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
            StartCoroutine(UISoupFinish());
        }

        objDiSendok++;
    }

    IEnumerator UISoupFinish()
    {
        DisableUINarasi();
        ObjUiNarasi[5].SetActive(true);
        
        yield return new WaitForSeconds(1);

        ObjSelection[22].SetActive(false);
    }



    #endregion

}
