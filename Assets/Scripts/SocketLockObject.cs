using ITISKIRUHERE;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SocketLockObject : MonoBehaviour
{
    public bool TestEvent = false;
    public UnityEvent onObjectSnappedEvent;
    [SerializeField] private XRSocketInteractor socket;
    public bool isFinishLocked = false;
    public bool interactables = true;
    public bool EventOnly = false;

    private void Awake()
    {
        if (socket == null)
            socket = GetComponent<XRSocketInteractor>();

        socket.selectEntered.AddListener(OnObjectSnapped);
    }

    private void OnDestroy()
    {
        if (socket != null)
            socket.selectEntered.RemoveListener(OnObjectSnapped);
    }

    private void OnEnable()
    {
        if (TestEvent) {

            onObjectSnappedEvent.Invoke();
        }
    }

    private void OnObjectSnapped(SelectEnterEventArgs args)
    {
        Debug.Log("Object masuk socket: " + args.interactableObject.transform.name);

        if (EventOnly) {
            isFinishLocked = true;
            onObjectSnappedEvent.Invoke();
            return;
        }

        if (!interactables)
            return;
        
        XRGrabInteractable grabInteractable =
            args.interactableObject.transform.GetComponent<XRGrabInteractable>();

        if (grabInteractable != null)
        {
            // Hapus semua interaction layer
            // Controller / tangan tidak bisa grab lagi
            //grabInteractable.interactionLayers = 0;
           // socket.interactionLayers = 0; // Disable socket interaction layers to prevent further interactions
            socket.attachTransform.gameObject.SetActive(false);
            //socket.GetComponent<BoxCollider>().enabled = false; // Disable the socket's collider to prevent further interactions

            Debug.Log("Object berhasil di-lock");
            AdvancedOutline advancedOutline = args.interactableObject.transform.GetComponent<AdvancedOutline>();
            if (advancedOutline != null) {

                advancedOutline.PulseWidth = false;
                advancedOutline.OutlineMode = AdvancedOutline.Mode.OutlineHidden;
                advancedOutline.OutlineWidth = 0;
            }

            args.interactableObject.transform.gameObject.isStatic = true;

           //get all child box colliders and disable them
           BoxCollider[] boxColliders = args.interactableObject.transform.GetComponentsInChildren<BoxCollider>();
            foreach (BoxCollider boxCollider in boxColliders)
            {
                boxCollider.enabled = false;
            }

            isFinishLocked = true;
            onObjectSnappedEvent.Invoke();
            //gameObject.SetActive(false);
        }

    }

}