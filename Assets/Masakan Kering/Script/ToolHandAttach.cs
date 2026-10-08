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

    private Transform handTransform;

    private Rigidbody rb;

    private bool attached;

    private void Awake()
    {
        if (grabInteractable == null)
        {
            grabInteractable =
                GetComponent<XRGrabInteractable>();
        }

        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(
                OnGrab
            );
        }
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(
                OnGrab
            );
        }
    }

    private void OnGrab(
        SelectEnterEventArgs args)
    {
        if (attached)
            return;

        IXRSelectInteractor interactor =
            args.interactorObject;

        if (interactor == null)
            return;

        // =====================================================
        // TRANSFORM TANGAN / CONTROLLER
        // =====================================================

        handTransform =
            interactor.transform;

        if (handTransform == null)
            return;

        // =====================================================
        // LEPASKAN DARI XR GRAB
        // =====================================================

        grabInteractable.interactionManager.SelectExit(
            interactor,
            grabInteractable
        );

        // =====================================================
        // MATIKAN PHYSICS
        // =====================================================

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // =====================================================
        // ATTACH TOOL KE TANGAN
        // =====================================================

        if (attachPoint != null)
        {
            // Posisi dan rotasi AttachPoint
            // akan dijadikan posisi tool di tangan.

            Vector3 worldPosition =
                attachPoint.position;

            Quaternion worldRotation =
                attachPoint.rotation;

            transform.SetParent(
                handTransform,
                true
            );

            // Setelah menjadi child tangan,
            // kita sesuaikan posisi berdasarkan AttachPoint.

            transform.position =
                worldPosition;

            transform.rotation =
                worldRotation;
        }
        else
        {
            // Fallback kalau AttachPoint belum dipasang.

            transform.SetParent(
                handTransform,
                true
            );
        }

        attached = true;

        Debug.Log(
            gameObject.name +
            " → TOOL MENEMPEL KE TANGAN"
        );
    }

    public void ReleaseTool()
    {
        if (!attached)
            return;

        attached = false;

        // =====================================================
        // LEPAS DARI TANGAN
        // =====================================================

        transform.SetParent(null);

        // =====================================================
        // AKTIFKAN PHYSICS KEMBALI
        // =====================================================

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
        }

        handTransform = null;

        Debug.Log(
            gameObject.name +
            " → TOOL DILEPAS"
        );
    }
}