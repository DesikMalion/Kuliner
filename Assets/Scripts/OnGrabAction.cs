using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRGrabInteractable))]
public class ObjectGrabEvent : MonoBehaviour
{
    public bool TestGrabbed = false;
    public bool TestHovered = false;
    public bool TestRelease = false;
    public bool TestHoverExit = false;

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

    private void OnEnable()
    {
        if (TestGrabbed)
        {
            onGrabbedEvent.Invoke();
        }

        if (TestHovered)
        {
            onHoverEnterEvent.Invoke();
        }

        if (TestRelease)
        {
            onReleaseEvent.Invoke();
        }

        if (TestHoverExit)
        {
            onHoverExitEvent.Invoke();
        }

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

        try
        {
            IXRHoverInteractor interactor = args.interactorObject;

            xRSocketInteractor = interactor as XRSocketInteractor;
            SocketLockObject socketLockObject = xRSocketInteractor.GetComponent<SocketLockObject>();
            if (!socketLockObject.interactables)
                return;
        }
        catch (System.NullReferenceException e)
        {
            Debug.Log("OnGrabbed: SocketLockObject null");
        }

        //Debug.Log("Interactor: " + interactor.transform.name);

        if (!isGrabbed)
        {
            isHovered = true;
            onHoverEnterEvent.Invoke();
        }
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        Debug.Log("Object tidak lagi ditunjuk!");

        //IXRHoverInteractor interactor = args.interactorObject;

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

        try
        {
            IXRSelectInteractor interactor = args.interactorObject;

            xRSocketInteractor = interactor as XRSocketInteractor;
            SocketLockObject socketLockObject = xRSocketInteractor.GetComponent<SocketLockObject>();
            if (!socketLockObject.interactables)
                return;
        }
        catch (System.NullReferenceException e)
        {
            Debug.Log("OnGrabbed: SocketLockObject null");
        }


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
        try
        {
            IXRSelectInteractor interactor = args.interactorObject;

            xRSocketInteractor = interactor as XRSocketInteractor;
            SocketLockObject socketLockObject = xRSocketInteractor.GetComponent<SocketLockObject>();
            if (!socketLockObject.interactables)
                return;
        }
        catch (System.NullReferenceException e)
        {
            Debug.Log("OnGrabbed: SocketLockObject null");
        }
        isGrabbed = false;
        onReleaseEvent.Invoke();
    }
}