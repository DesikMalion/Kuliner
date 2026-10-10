using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class ToolHandAttach : MonoBehaviour
{
    [Header("XR Grab")]
    public XRGrabInteractable grabInteractable;

    [Header("Attach Point")]
    public Transform attachPoint;

    private Rigidbody rb;

    private bool attached;


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        if (grabInteractable == null)
        {
            grabInteractable =
                GetComponent<XRGrabInteractable>();
        }

        rb = GetComponent<Rigidbody>();
    }


    // =========================================================
    // ENABLE
    // =========================================================

    private void OnEnable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(
                OnGrab
            );
        }
    }


    // =========================================================
    // DISABLE
    // =========================================================

    private void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(
                OnGrab
            );
        }
    }


    // =========================================================
    // GRAB
    // =========================================================

    private void OnGrab(
        SelectEnterEventArgs args)
    {
        if (attached)
            return;


        // =====================================================
        // CEK ATTACH POINT
        // =====================================================

        if (attachPoint == null)
        {
            Debug.LogWarning(
                gameObject.name +
                " belum memiliki Attach Point."
            );

            return;
        }


        // =====================================================
        // LEPASKAN DARI XR GRAB
        // =====================================================

        IXRSelectInteractor interactor =
            args.interactorObject;

        if (interactor != null)
        {
            grabInteractable.interactionManager.SelectExit(
                interactor,
                grabInteractable
            );
        }


        // =====================================================
        // MATIKAN PHYSICS
        // =====================================================

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }


        // =====================================================
        // JADIKAN CHILD ATTACH POINT
        // =====================================================

        transform.SetParent(
            attachPoint,
            false
        );


        // =====================================================
        // RESET LOCAL TRANSFORM
        // =====================================================

        transform.localPosition =
            Vector3.zero;

        transform.localRotation =
            Quaternion.identity;

        transform.localScale =
            Vector3.one;


        // =====================================================
        // STATUS
        // =====================================================

        attached = true;


        Debug.Log(
            gameObject.name +
            " → CHILD DARI ATTACH POINT"
        );
    }


    // =========================================================
    // RELEASE TOOL
    // =========================================================

    public void ReleaseTool()
    {
        if (!attached)
            return;


        attached = false;


        // =====================================================
        // LEPAS DARI ATTACH POINT
        // =====================================================

        transform.SetParent(
            null,
            true
        );


        // =====================================================
        // AKTIFKAN PHYSICS
        // =====================================================

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }


        // =====================================================
        // RESET STATUS
        // =====================================================

        Debug.Log(
            gameObject.name +
            " → TOOL DILEPAS"
        );
    }
}