using ITISKIRUHERE;
using MikeNspired.XRIStarterKit;
using System.Collections;
using UnityEngine;

public class Scene2Mgr : MonoBehaviour
{

    public GameObject Player;
    public GameObject NarasiAwal;
    public GameObject PintuKiri;
    public GameObject PintuKanan;

    public GameObject[] ObjShapes;
    public GameObject[] ObjSocket;
    public GameObject[] ObjSelection;
    public GameObject[] ObjUiNarasi;

    public GameObject NarasiFinal;

    public GameObject ObjHandAvatarDefaultR;
    public GameObject ObjHandAvatarDefaultL;

    //case 1 Kondisi Alat Panas 
    public GameObject[] ObjSocketCase1;
    public GameObject[] ObjShapesCase1;
    public GameObject[] ObjHandCase1;
    public GameObject[] ObjHandAvatarCase1;

    //case 2 Kobaran Api dari Kompor
    bool isCase2Started = false;
    public GameObject[] ObjShapesCase2;
    public GameObject[] ObjSelectCase2;
    public GameObject[] ObjSelectAvatarCase2;

    bool isCase5Started = false;

    bool isPintuKiriWatcher = false;
    bool isPintuKananWatcher = false;


    void Start()
    {
        NarasiAwal.SetActive(true);
        NarasiFinal.SetActive(false);

        for (int i = 0; i < ObjShapes.Length; i++)
        {
            OutlineEnabler(false, ObjShapes[i]);
        }
        for (int i = 0; i < ObjSocket.Length; i++)
        {
            ObjSocket[i].SetActive(false);
        }
        for (int i = 0; i < ObjSelection.Length; i++)
        {
            ObjSelection[i].SetActive(false);
        }
        for (int i = 0; i < ObjUiNarasi.Length; i++)
        {
            ObjUiNarasi[i].SetActive(false);
        }
    }

    void Update()
    {

        Case2JetBurnerOff();
        Case2ControlBurnerOff();
        Case2Finish();
        Case3BlenderOff();
        Case5CekPintu();
        Case5RegulatorOff();
        Case5CekTinggalDapur();

    }

    void OutlineEnabler(bool isEnable, GameObject obj)
    {

        AdvancedOutline advancedOutline = obj.GetComponent<AdvancedOutline>();

        if (advancedOutline == null)
        {
            advancedOutline = obj.GetComponentInChildren<AdvancedOutline>();
        }
        if (advancedOutline == null)
        {
            Debug.LogError("AdvancedOutline component not found on the object or its children.");
            return;
        }
        if (isEnable)
        {
            advancedOutline.OutlineMode = AdvancedOutline.Mode.OutlineVisible;
            advancedOutline.OutlineWidth = 10;
            advancedOutline.PulseWidth = true;
            Rigidbody rigidbody = obj.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.isKinematic = false;
            }
            Collider[] boxColliders = obj.transform.GetComponentsInChildren<Collider>();
            foreach (Collider boxCollider in boxColliders)
            {
                boxCollider.enabled = true;
            }
        }
        else
        {
            advancedOutline.OutlineMode = AdvancedOutline.Mode.OutlineHidden;
            advancedOutline.OutlineWidth = 0;
            advancedOutline.PulseWidth = false;
            Rigidbody rigidbody = obj.GetComponent<Rigidbody>();
            if (rigidbody != null)
            {
                rigidbody.isKinematic = true;
            }
            Collider[] boxColliders = obj.transform.GetComponentsInChildren<Collider>();
            foreach (Collider boxCollider in boxColliders)
            {
                boxCollider.enabled = false;
            }
        }

    }

    #region case 1 Kondisi Alat Panas
    public void CaseNarasiAwal()
    {
        NarasiAwal.SetActive(false);
        for (int i = 0; i < ObjUiNarasi.Length; i++)
        {
            ObjUiNarasi[i].SetActive(false);
        }

        ObjUiNarasi[0].SetActive(true);
        ObjSelection[12].SetActive(true);
        ObjShapesCase2[4].SetActive(false);
        ObjShapesCase2[10].SetActive(false);
    }

    public void Case1Selection() {

        for (int i = 0; i < ObjHandCase1.Length; i++)
        {
            ObjHandCase1[i].SetActive(true);
        }

        for (int i = 0; i < ObjHandAvatarCase1.Length; i++)
        {
            ObjHandAvatarCase1[i].SetActive(false);
        }

        for (int i = 0; i < ObjUiNarasi.Length; i++)
        {
            ObjUiNarasi[i].SetActive(false);
        }


        ObjHandAvatarDefaultR.SetActive(true);
        ObjHandAvatarDefaultL.SetActive(true);
    }

    GameObject selectedCase1_1 = null;
    GameObject selectedCase1_2 = null;
    GameObject TrueSelectedCase1_1 = null;
    GameObject TrueSelectedCase1_2 = null;
    public void Case1UserSelect(GameObject obj)
    {
        TrueSelectedCase1_1 = ObjHandAvatarCase1[0];
        TrueSelectedCase1_2 = ObjHandAvatarCase1[1];

        obj.SetActive(false);

        if (selectedCase1_1 == null)
        {
            selectedCase1_1 = obj;

        }
        else if (selectedCase1_2 == null)
        {
            selectedCase1_2 = obj;

        }

        for (int i = 0; i < ObjHandAvatarCase1.Length; i++)
        {
            if (ObjHandAvatarCase1[i].name == obj.name)
            {
                ObjHandAvatarCase1[i].SetActive(true);
            }

            if (obj.name.ToLower().Contains("right"))
            {
                ObjHandAvatarDefaultR.SetActive(false);
            }
            else if (obj.name.ToLower().Contains("left"))
            {
                ObjHandAvatarDefaultL.SetActive(false);

            }
        }

        if (selectedCase1_2 != null)
        {
            bool isCorrect = false;
            if ((selectedCase1_1.name == TrueSelectedCase1_1.name || selectedCase1_1.name == TrueSelectedCase1_2.name)
                && (selectedCase1_2.name == TrueSelectedCase1_1.name || selectedCase1_2.name == TrueSelectedCase1_2.name))
            {
                isCorrect = true;
            }

            if (isCorrect)
            {
                for (int i = 0; i < ObjHandCase1.Length; i++)
                {
                    ObjHandCase1[i].SetActive(false);
                }

                ObjUiNarasi[2].SetActive(true);
            }
            else {

                ObjUiNarasi[1].SetActive(true);
            }
        }


    }

    public void Case1ResetSelection()
    {
        selectedCase1_1 = null;
        selectedCase1_2 = null;
        ObjHandAvatarDefaultR.SetActive(true);
        ObjHandAvatarDefaultL.SetActive(true);
        for (int i = 0; i < ObjUiNarasi.Length; i++)
        {
            ObjUiNarasi[i].SetActive(false);
        }
        for (int i = 0; i < ObjHandAvatarCase1.Length; i++)
        {
            ObjHandAvatarCase1[i].SetActive(false);
        }
        for (int i = 0; i < ObjHandCase1.Length; i++)
        {
            ObjHandCase1[i].SetActive(true);
        }
    }

    public void Case1FinishSelection()
    {
        for (int i = 0; i < ObjUiNarasi.Length; i++)
        {
            ObjUiNarasi[i].SetActive(false);
        }

        for (int i = 0; i < ObjSocketCase1.Length; i++)
        {
            ObjSocketCase1[i].SetActive(true);
        }

        for (int i = 0; i < ObjShapesCase1.Length; i++)
        {
            OutlineEnabler(true, ObjShapesCase1[i]);
        }

    }

    public void Case1FinishLoyang()
    {
        bool allLocked = true;
        for (int i = 0; i < ObjShapesCase1.Length; i++)
        {
            if (!ObjSocketCase1[i].GetComponent<SocketLockObject>().isFinishLocked)
            {
                allLocked = false;
                break;
            }
        }
        
        if (allLocked)
        {

            ObjUiNarasi[3].SetActive(true);
        }


    }
    #endregion

    #region case 2 Kobaran Api dari Kompor
    public void Case2Naration() {

        Case1ResetSelection();


        for (int i = 0; i < ObjHandCase1.Length; i++)
        {
            ObjHandCase1[i].SetActive(false);
        }

        for (int i = 0; i < ObjUiNarasi.Length; i++)
        {
            ObjUiNarasi[i].SetActive(false);
        }
        ObjUiNarasi[4].SetActive(true);
        ObjSelection[12].SetActive(true);
        ObjShapesCase2[4].SetActive(false);
        ObjShapesCase2[10].SetActive(true);

    }



    public void Case2Start() {
        for (int i = 0; i < ObjUiNarasi.Length; i++)
        {
            ObjUiNarasi[i].SetActive(false);
        }

        OutlineEnabler(true, ObjShapesCase2[0]);

        
        isJetBurnerOn = true;
        isCase2Started = true;
    }

    XRKnob JetBurnerKnob;
    XRKnob BurnerKnob;
    public bool isJetBurnerOn = false;
    public bool isControlBurnerOn = false;

    public void Case2JetBurnerOff() {
        if (!isJetBurnerOn) return;
        if(!JetBurnerKnob)
            JetBurnerKnob = ObjShapesCase2[0].GetComponent<XRKnob>();
        if (!BurnerKnob)
            BurnerKnob = ObjShapesCase2[1].GetComponent<XRKnob>();
        //Debug.Log("Case2JetBurnerOff: " + JetBurnerKnob.Value);
        if (JetBurnerKnob.Value == 1)
        {
            OutlineEnabler(true, ObjShapesCase2[1]);
            OutlineEnabler(false, ObjShapesCase2[0]);

            ObjShapesCase2[2].SetActive(false);
            isJetBurnerOn = false;
            isControlBurnerOn = true;
        }
    }

    public void Case2ControlBurnerOff()
    {
        if (!isControlBurnerOn) return;
        if (BurnerKnob.Value == 1)
        {
            OutlineEnabler(false, ObjShapesCase2[1]);
            OutlineEnabler(false, ObjShapesCase2[0]);

            ObjShapesCase2[3].SetActive(false);
            //ObjShapesCase2[4].SetActive(true);

            for (int i = 0; i < ObjUiNarasi.Length; i++)
            {
                ObjUiNarasi[i].SetActive(false);
            }

            ObjUiNarasi[5].SetActive(true);
            isControlBurnerOn = false;
        }


    }

    public void Case2SelectAPD() {
        for (int i = 0; i < ObjUiNarasi.Length; i++)
        {
            ObjUiNarasi[i].SetActive(false);
        }
        for (int i = 0; i < ObjSelectCase2.Length; i++)
        {

                ObjSelectCase2[i].SetActive(true);

        }


    }

    GameObject selectedCase2_1 = null;
    GameObject selectedCase2_2 = null;

    public void Case2SelectAvatar(GameObject obj) {

        obj.SetActive(false);
        for (int i = 0; i < ObjSelectAvatarCase2.Length; i++)
        {
            if (ObjSelectAvatarCase2[i].name == obj.name)
            {
                ObjSelectAvatarCase2[i].SetActive(true);
            }

        }

        if (selectedCase2_1 == null)
        {
            selectedCase2_1 = obj;
        }
        else if (selectedCase2_2 == null)
        {
            //finish selection
            selectedCase2_2 = obj;
            OutlineEnabler(true, ObjShapesCase2[5]);
            ObjShapesCase2[6].SetActive(true);
        }
    }

    public void Case2PourWater() {
        OutlineEnabler(false, ObjShapesCase2[5]);
        ObjShapesCase2[7].SetActive(true);
        StartCoroutine(Case2PourWaterFinish());
    }


    IEnumerator Case2PourWaterFinish() {
        Debug.Log("Case2PourWaterFinish");
        yield return new WaitForSeconds(3f);
        ObjShapesCase2[5].SetActive(false);
        ObjShapesCase2[6].SetActive(false);
        ObjShapesCase2[7].SetActive(false);
        ObjShapesCase2[8].SetActive(true);
        ObjShapesCase2[4].SetActive(true);
        for (int i = 0; i < ObjUiNarasi.Length; i++)
        {
            ObjUiNarasi[i].SetActive(false);
        }
        yield return new WaitForSeconds(2f);
        ObjUiNarasi[6].SetActive(true);
    }


    public void Case2PintuExit() {
        for (int i = 0; i < ObjUiNarasi.Length; i++)
        {
            ObjUiNarasi[i].SetActive(false);
        }

        ObjShapesCase2[8].SetActive(true);
        //OutlineEnabler(true, ObjShapesCase2[8]);
        isPintuKiriWatcher = true;
    }

    public void Case2Finish()
    {
        if (!isPintuKiriWatcher || !isCase2Started) return;

        if (PintuKiri.transform.localEulerAngles.y > 45) {
            isPintuKiriWatcher = false;
            isCase2Started = false;
            for (int i = 0; i < ObjUiNarasi.Length; i++)
            {
                ObjUiNarasi[i].SetActive(false);
            }

            ObjShapesCase2[8].SetActive(false);
            ObjShapesCase2[9].SetActive(false);
            //OutlineEnabler(true, ObjShapesCase2[8]);
            ObjUiNarasi[7].SetActive(true);
        }
    }
    #endregion

    #region case 3 Blender
    public void Case3Start() {
        for (int i = 0; i < ObjUiNarasi.Length; i++)
        {
            ObjUiNarasi[i].SetActive(false);
        }
        ObjUiNarasi[8].SetActive(true);

        ObjSelection[9].SetActive(true);
        ObjSelection[10].SetActive(true);

    }

    public void Case3Peringatan() {
        for (int i = 0; i < ObjUiNarasi.Length; i++)
        {
            ObjUiNarasi[i].SetActive(false);
        }
        ObjUiNarasi[9].SetActive(true);
        PintuKiri.transform.localEulerAngles = new Vector3(0, 0, 0);
        //ObjSelection[10].SetActive(false);
    }

    public void Case3TombolBlender() {
        for (int i = 0; i < ObjUiNarasi.Length; i++)
        {
            ObjUiNarasi[i].SetActive(false);
        }
        //ObjUiNarasi[10].SetActive(true);

        OutlineEnabler(true, ObjShapes[9]);
        isBlenderOn = true;
    }

    XRKnob BlenderKnob;
    public bool isBlenderOn = false;

    public void Case3BlenderOff()
    {
        if (!isBlenderOn) return;
        if (!BlenderKnob)
            BlenderKnob = ObjShapes[9].GetComponent<XRKnob>();

        if (BlenderKnob.Value == 0)
        {
            OutlineEnabler(false, ObjShapes[9]);

            ObjUiNarasi[10].SetActive(true);

            ObjSelection[10].GetComponent<ithappy.Construction.RotationScript>().enabled =false;
            isBlenderOn = false;
        }
    }

    public void Case3CabutKabel(){
        for (int i = 0; i < ObjUiNarasi.Length; i++)
        {
            ObjUiNarasi[i].SetActive(false);
        }
        ObjSocket[4].SetActive(true);
        ObjSocket[5].SetActive(true);
        OutlineEnabler(true, ObjShapes[4]);

    }

    public void Case3Finish() {
        for (int i = 0; i < ObjUiNarasi.Length; i++)
        {
            ObjUiNarasi[i].SetActive(false);
        }
        ObjUiNarasi[11].SetActive(true);
    }
    #endregion

    #region case 4 Air Mendidih
    public void Case4Start() {

        for (int i = 0; i < ObjUiNarasi.Length; i++)
        {
            ObjUiNarasi[i].SetActive(false);
        }

        ObjUiNarasi[12].SetActive(true);
        ObjShapes[5].SetActive(true);
        ObjShapes[6].SetActive(true);

    }

    public void Case4APD() {
        for (int i = 0; i < ObjUiNarasi.Length; i++)
        {
            ObjUiNarasi[i].SetActive(false);
        }

        ObjUiNarasi[13].SetActive(true);

    }

    public void Case4APDSelect() {

        for (int i = 0; i < ObjUiNarasi.Length; i++)
        {
            ObjUiNarasi[i].SetActive(false);
        }

        ObjSelection[13].SetActive(true);
        ObjSelection[14].SetActive(true);
        ObjSelection[15].SetActive(true);
    }

    int case4SelectedCount = 0;
    public void Case4APDObjSelect(GameObject Obj) {
        case4SelectedCount++;
        Obj.SetActive(false);
        for (int i = 0; i < ObjHandAvatarCase1.Length; i++)
        {
            if (ObjHandAvatarCase1[i].name == Obj.name)
            {
                ObjHandAvatarCase1[i].SetActive(true);
            }

        }

        if (Obj.name.ToLower().Contains("right"))
        {
            ObjHandAvatarDefaultR.SetActive(false);
        }
        else if (Obj.name.ToLower().Contains("left"))
        {
            ObjHandAvatarDefaultL.SetActive(false);

        }

        if (case4SelectedCount >= 3)
        {
            Case4TutupPanci();
        }
    }

    public void Case4TutupPanci()
    {

        for (int i = 0; i < ObjUiNarasi.Length; i++)
        {
            ObjUiNarasi[i].SetActive(false);
        }

        ObjUiNarasi[14].SetActive(true);

    }

    public void Case4TutupPanciEnable() {
        for (int i = 0; i < ObjUiNarasi.Length; i++)
        {
            ObjUiNarasi[i].SetActive(false);
        }

        ObjSocket[6].SetActive(true);
        //OutlineEnabler(true, ObjShapes[6]);
        ObjShapes[5].SetActive(true);
        OutlineEnabler(true, ObjShapes[5]);

        Collider[] boxColliders = ObjShapes[6].transform.GetComponentsInChildren<Collider>();
        foreach (Collider boxCollider in boxColliders)
        {
            boxCollider.enabled = true;
        }

    }

    public void Case4Panci() {
        for (int i = 0; i < ObjUiNarasi.Length; i++)
        {
            ObjUiNarasi[i].SetActive(false);
        }
        ObjUiNarasi[15].SetActive(true);
    }

    public void Case4PanciEnable() {
        for (int i = 0; i < ObjUiNarasi.Length; i++)
        {
            ObjUiNarasi[i].SetActive(false);
        }
        ObjSocket[7].SetActive(true);
        ObjShapes[6].SetActive(true);
        OutlineEnabler(true, ObjShapes[6]);

    }

    public void Case4Finish() {
        for (int i = 0; i < ObjUiNarasi.Length; i++)
        {
            ObjUiNarasi[i].SetActive(false);
        }
        ObjUiNarasi[16].SetActive(true);
        OutlineEnabler(false, ObjShapes[6]);
    }
    #endregion

    public void Case5Start() {
        Case1Selection();
        for (int i = 0; i < ObjUiNarasi.Length; i++)
        {
            ObjUiNarasi[i].SetActive(false);
        }
        ObjUiNarasi[17].SetActive(true);

        isCase5Started = true;

        isPintuKiriWatcher = false;
        isPintuKananWatcher = false;
    }


    public void Case5Identifikasi()
    {

        for (int i = 0; i < ObjUiNarasi.Length; i++)
        {
            ObjUiNarasi[i].SetActive(false);
        }

        ObjUiNarasi[18].SetActive(true);

        ObjSelection[16].SetActive(true);
        ObjSelection[8].SetActive(true);
        ObjSelection[18].SetActive(true);

        PintuKiri.transform.localEulerAngles = new Vector3(0, 0, 0);
        PintuKanan.transform.localEulerAngles = new Vector3(0, 0, 0);

    }

    public void Case5SetPintu() {
        for (int i = 0; i < ObjUiNarasi.Length; i++)
        {
            ObjUiNarasi[i].SetActive(false);
        }

        isPintuKiriWatcher = true;
        isPintuKananWatcher = true;
    }


    void Case5CekPintu() {

        if (!isCase5Started && (!isPintuKiriWatcher || !isPintuKananWatcher))
            return;
        if (PintuKiri.transform.localEulerAngles.x > 25 &&
                PintuKiri.transform.localEulerAngles.x > 25) 
            {
                isPintuKiriWatcher = false;
                isPintuKananWatcher = false;
                ObjSelection[8].SetActive(false);
                ObjSelection[18].SetActive(false);

                ObjUiNarasi[19].SetActive(true);
            }

    }

    public void Case5CekRegulator() {

        for (int i = 0; i < ObjUiNarasi.Length; i++)
        {
            ObjUiNarasi[i].SetActive(false);
        }

        OutlineEnabler(true, ObjShapes[10]);
        isRegulatorEnable = true;
    }

    public bool isRegulatorEnable = false;
    XRKnob knobRegulator;
    void Case5RegulatorOff() {
        if (!isRegulatorEnable) return;

        if (!knobRegulator)
            knobRegulator = ObjShapes[10].GetComponent<XRKnob>();

        //Debug.Log("Case2JetBurnerOff: " + JetBurnerKnob.Value);
        if (knobRegulator.Value == 0)
        {

            OutlineEnabler(false, ObjShapes[10]);

            ObjUiNarasi[20].SetActive(true);
            ObjSelection[16].SetActive(false);
            isRegulatorEnable = false;

        }
    }

    public void Case5TinggalkanDapur() {

        for (int i = 0; i < ObjUiNarasi.Length; i++)
        {
            ObjUiNarasi[i].SetActive(false);
        }
        isCase5TinggalDapur = true;
    }

    bool isCase5TinggalDapur = false;
    void Case5CekTinggalDapur() { 
        if (!isCase5TinggalDapur) return;

        if (Player.transform.localPosition.z > 5.4f) {

            isCase5TinggalDapur = false;
            ObjUiNarasi[21].SetActive(true);
        }

    }

    public void Case5Finish()
    {

        for (int i = 0; i < ObjUiNarasi.Length; i++)
        {
            ObjUiNarasi[i].SetActive(false);
        }
        ObjUiNarasi[22].SetActive(true);
    }


}


