using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRGrabInteractable))]
public class ObjectGrabEvent : MonoBehaviour
{
    bool isGrabbed = false;
    bool isHovered = false;

    public UnityEvent onGrabbedEvent;
    public UnityEvent onReleaseEvent;
    
    private XRGrabInteractable grabInteractable;

    public UnityEvent onHoverEnterEvent;
    public UnityEvent onHoverExitEvent;

    XRSocketInteractor xRSocketInteractor;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.selectEntered.AddListener(OnGrabbed);
        grabInteractable.selectExited.AddListener(OnReleased);

        grabInteractable.hoverEntered.AddListener(OnHoverEntered);
        grabInteractable.hoverExited.AddListener(OnHoverExited);
    }

    private void OnDestroy()
    {
        if (grabInteractable == null)
            return;

        grabInteractable.selectEntered.RemoveListener(OnGrabbed);
        grabInteractable.selectExited.RemoveListener(OnReleased);

        grabInteractable.hoverEntered.RemoveListener(OnHoverEntered);
        grabInteractable.hoverExited.RemoveListener(OnHoverExited);
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        Debug.Log("Object sedang ditunjuk!");

        IXRHoverInteractor interactor = args.interactorObject;

        //Debug.Log("Interactor: " + interactor.transform.name);

        if(!isGrabbed)
        {
            isHovered = true;
            onHoverEnterEvent.Invoke();
        }
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        Debug.Log("Object tidak lagi ditunjuk!");

        IXRHoverInteractor interactor = args.interactorObject;

        //Debug.Log("Interactor keluar: " + interactor.transform.name);

        if (isHovered)
        {
            isHovered = false;
            onHoverExitEvent.Invoke();
        }
    }


    private void OnGrabbed(SelectEnterEventArgs args)
    {
        Debug.Log("Object di grab");

        IXRSelectInteractor interactor = args.interactorObject;

        //Debug.Log("Di grab oleh: " + interactor.transform.name);
        isGrabbed = true;
        onGrabbedEvent.Invoke();

        if (isHovered)
        {
            isHovered = false;
            onHoverExitEvent.Invoke();
        }
    }

    private void OnReleased(SelectExitEventArgs args)
    {
        Debug.Log("Object dilepas");
        isGrabbed = false;
        onReleaseEvent.Invoke();
    }
}