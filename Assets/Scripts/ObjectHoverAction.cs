using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRGrabInteractable))]
public class ObjectHoverAction : MonoBehaviour
{

    public UnityEvent onHoverEnterEvent;
    public UnityEvent onHoverExitEvent;

    private XRGrabInteractable grabInteractable;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        grabInteractable.hoverEntered.AddListener(OnHoverEntered);
        grabInteractable.hoverExited.AddListener(OnHoverExited);
    }

    private void OnDestroy()
    {
        if (grabInteractable == null)
            return;

        grabInteractable.hoverEntered.RemoveListener(OnHoverEntered);
        grabInteractable.hoverExited.RemoveListener(OnHoverExited);
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        Debug.Log("Object sedang ditunjuk!");

        IXRHoverInteractor interactor = args.interactorObject;

        Debug.Log("Interactor: " + interactor.transform.name);
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        Debug.Log("Object tidak lagi ditunjuk!");

        IXRHoverInteractor interactor = args.interactorObject;

        Debug.Log("Interactor keluar: " + interactor.transform.name);
    }
}