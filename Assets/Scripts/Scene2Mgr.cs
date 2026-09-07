using ITISKIRUHERE;
using MikeNspired.XRIStarterKit;
using System.Collections;
using UnityEngine;

public class Scene2Mgr : MonoBehaviour
{

    public GameObject NarasiAwal;

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
    public GameObject[] ObjShapesCase2;
    public GameObject[] ObjSelectCase2;
    public GameObject[] ObjSelectAvatarCase2;


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


    public void CaseNarasiAwal()
    {
        NarasiAwal.SetActive(false);
        for (int i = 0; i < ObjUiNarasi.Length; i++)
        {
            ObjUiNarasi[i].SetActive(false);
        }

        ObjUiNarasi[0].SetActive(true);
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

    public void Case2Naration() {

        for (int i = 0; i < ObjUiNarasi.Length; i++)
        {
            ObjUiNarasi[i].SetActive(false);
        }
        ObjUiNarasi[4].SetActive(true);

    }



    public void Case2Start() {
        for (int i = 0; i < ObjUiNarasi.Length; i++)
        {
            ObjUiNarasi[i].SetActive(false);
        }

        OutlineEnabler(true, ObjShapesCase2[0]);

        
        isJetBurnerOn = true;
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
        ObjShapesCase2[4].SetActive(true);
        for (int i = 0; i < ObjUiNarasi.Length; i++)
        {
            ObjUiNarasi[i].SetActive(false);
        }
        yield return new WaitForSeconds(2f);
        ObjUiNarasi[6].SetActive(true);
    }



}
