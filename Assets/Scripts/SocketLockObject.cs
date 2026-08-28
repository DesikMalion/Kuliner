using ITISKIRUHERE;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SocketLockObject : MonoBehaviour
{
    public UnityEvent onObjectSnappedEvent;
    [SerializeField] private XRSocketInteractor socket;
    public bool isFinishLocked = false;

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

    private void OnObjectSnapped(SelectEnterEventArgs args)
    {
        Debug.Log("Object masuk socket: " + args.interactableObject.transform.name);

        
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
            if (advancedOutline != null)
                advancedOutline.enabled = false;

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