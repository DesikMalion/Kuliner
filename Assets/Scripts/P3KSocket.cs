using ITISKIRUHERE;
using System.Collections;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class P3KSocket : MonoBehaviour
{
    [SerializeField] private XRSocketInteractor socket;
    public SceneP3KMgr sceneP3KMgr;
    public GameObject Socket,TriggerEnable;
    bool waitingActive = false;
    GameObject interactableObject;


    void Start()
    {
        socket.gameObject.SetActive(false);
        socket.transform.localPosition = Vector3.zero;
        TriggerEnable.SetActive(true);
        FindInteractObj();
    }

    private void Awake()
    {
        socket.selectEntered.AddListener(OnObjectSnapped);
    }

    private void OnDestroy()
    {
        if (socket != null)
            socket.selectEntered.RemoveListener(OnObjectSnapped);
    }

    void FindInteractObj() { 
        int index = 0;
        for (int i = 0; i < sceneP3KMgr.ObjShapes.Length; i++)
        {
            if (sceneP3KMgr.ObjSocket[i].transform.name == gameObject.name)
            {
                index = i;
                break;
            }
        }

        interactableObject = sceneP3KMgr.ObjShapes[index];
    }


    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.tag == "Target" && other.transform.parent.gameObject.name == interactableObject.name) { 
            if (!waitingActive)
            {
                //StopAllCoroutines();
                StartCoroutine(WaitActiveSocket(other.transform.parent.gameObject));

            }

        }
    }

    private void OnObjectSnapped(SelectEnterEventArgs args)
    {
        Debug.Log("Object masuk socket: " + args.interactableObject.transform.name);


        XRGrabInteractable grabInteractable =
            args.interactableObject.transform.GetComponent<XRGrabInteractable>();

        if (grabInteractable != null)
        {

            if (!waitingActive)
            {
                //StopAllCoroutines();
                StartCoroutine(WaitNonActiveSocket());
            }
        }

    }

    IEnumerator WaitActiveSocket(GameObject interactableObject)
    {
        waitingActive = true;
       // sceneP3KMgr.ObjSetKinematic(interactableObject, true);
        yield return new WaitForSeconds(1);
        //sceneP3KMgr.ShowUIShapes(interactableObject);
        sceneP3KMgr.ObjSetKinematic(interactableObject, false);
        Socket.SetActive(true);
        socket.attachTransform.gameObject.SetActive(true);
        TriggerEnable.SetActive(false);
        waitingActive = false;
    }

    void ObjSnapped() {
        StopAllCoroutines();
        waitingActive = false;

        Socket.SetActive(false);
        TriggerEnable.SetActive(true);

    }

    IEnumerator WaitNonActiveSocket()
    {
        //sceneP3KMgr.HideUIShapes();
        sceneP3KMgr.ObjSetKinematic(interactableObject, true);
        socket.attachTransform.gameObject.SetActive(false);
        waitingActive = true;
        yield return new WaitForSeconds(1);
        ObjSnapped();

    }


}
