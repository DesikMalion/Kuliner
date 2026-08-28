using ITISKIRUHERE;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Scene1Mgr : MonoBehaviour
{
    public GameObject NarasiAwal;

    public GameObject[] ObjShapes;
    public GameObject[] ObjSocket;
    public GameObject[] ObjUiNarasi;

    public GameObject NarasiFinal;

    void Start()
    {
        NarasiAwal.SetActive(true);
        ExitGrabShapes(null);

        for (int i = 0; i < ObjShapes.Length; i++)
        {
            ObjShapes[i].GetComponent<AdvancedOutline>().enabled = false;

        }

    }

    public void HideNarasiAwal() { 
        NarasiAwal.SetActive(false);
        for (int i = 0; i < ObjShapes.Length; i++)
        {
            ObjShapes[i].GetComponent<AdvancedOutline>().enabled = true;

        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void EnterGrabShapes(GameObject Obj) { 
        for (int i = 0; i < ObjShapes.Length; i++)
        {
            if (ObjShapes[i] == Obj)
            {
                if (!ObjSocket[i].GetComponent<SocketLockObject>().isFinishLocked)
                {
                    ObjSocket[i].GetComponent<XRSocketInteractor>().attachTransform.gameObject.SetActive(true);
                    ObjSocket[i].GetComponent<BoxCollider>().enabled = true;
                    ObjUiNarasi[i].SetActive(false);
                }
            
                break;
            }
        }

    }

    public void ExitGrabShapes(GameObject obj)
    {
        for (int i = 0; i < ObjShapes.Length; i++)
        {

            ObjUiNarasi[i].SetActive(false);
            ObjSocket[i].GetComponent<XRSocketInteractor>().attachTransform.gameObject.SetActive(false);
            ObjSocket[i].GetComponent<BoxCollider>().enabled = false;
        }
    }

    public void HoverShapes(GameObject obj) { 
        for (int i = 0; i < ObjShapes.Length; i++)
        {
            ObjUiNarasi[i].SetActive(false);
            if (ObjShapes[i] == obj)
            {
                ObjUiNarasi[i].SetActive(true);
            }
        }

    }


    public void ExitHoverShapes() { 
        for (int i = 0; i < ObjShapes.Length; i++)
        {
            ObjUiNarasi[i].SetActive(false);
        }
    }

    public void ShowFinalNarasi() { 
        bool allLocked = true;
        for (int i = 0; i < ObjSocket.Length; i++)
        {
            if (!ObjSocket[i].GetComponent<SocketLockObject>().isFinishLocked)
            {
                allLocked = false;
                break;
            }
        }

        if (allLocked)
        {
            NarasiFinal.SetActive(true);
        }

    }
}
