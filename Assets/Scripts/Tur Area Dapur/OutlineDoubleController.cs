using ITISKIRUHERE; // Namespace untuk AdvancedOutline
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRGrabInteractable))]
public class OutlineDoubleController : MonoBehaviour
{
    [Header("Pengaturan Outline")]
    public AdvancedOutline outlinePisauAsli;
    public AdvancedOutline outlineBayanganSocket;
    public GameObject meshBayanganSocket;

    private XRGrabInteractable grabInteractable;
    private Rigidbody rb; // Tambahan untuk memanipulasi physics
    private bool isLockedInSocket = false;

    private XRSocketInteractor hoveredSocket;
    private bool originalTrackRotation;

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
        rb = GetComponent<Rigidbody>(); // Mengambil komponen Rigidbody

        if (outlinePisauAsli == null)
            outlinePisauAsli = GetComponent<AdvancedOutline>();

        originalTrackRotation = grabInteractable.trackRotation;

        SetOutlineState(false);

        grabInteractable.hoverEntered.AddListener(OnHoverEnter);
        grabInteractable.hoverExited.AddListener(OnHoverExit);
        grabInteractable.selectEntered.AddListener(OnSelectEnter);
        grabInteractable.selectExited.AddListener(OnSelectExit);
    }

    private void OnDestroy()
    {
        grabInteractable.hoverEntered.RemoveListener(OnHoverEnter);
        grabInteractable.hoverExited.RemoveListener(OnHoverExit);
        grabInteractable.selectEntered.RemoveListener(OnSelectEnter);
        grabInteractable.selectExited.RemoveListener(OnSelectExit);
    }

    private void Update()
    {
        if (hoveredSocket != null && !isLockedInSocket)
        {
            Transform targetTransform = hoveredSocket.attachTransform != null ? hoveredSocket.attachTransform : hoveredSocket.transform;
            transform.rotation = Quaternion.Lerp(transform.rotation, targetTransform.rotation, Time.deltaTime * 15f);
        }
    }

    private void OnHoverEnter(HoverEnterEventArgs args)
    {
        if (args.interactorObject is XRSocketInteractor socket)
        {
            hoveredSocket = socket;
            grabInteractable.trackRotation = false;
        }
        CekStatusOutline();
    }

    private void OnHoverExit(HoverExitEventArgs args)
    {
        if (args.interactorObject is XRSocketInteractor)
        {
            hoveredSocket = null;
            grabInteractable.trackRotation = originalTrackRotation;
        }
        CekStatusOutline();
    }

    private void OnSelectEnter(SelectEnterEventArgs args)
    {
        if (args.interactorObject is XRSocketInteractor)
        {
            isLockedInSocket = true;

            // LOGIKA BARU: Saat masuk socket, matikan gravitasi & jadikan Kinematic
            if (rb != null)
            {
                rb.useGravity = false;
                rb.isKinematic = true;
            }
        }
        CekStatusOutline();
    }

    private void OnSelectExit(SelectExitEventArgs args)
    {
        if (args.interactorObject is XRSocketInteractor)
        {
            isLockedInSocket = false;
            grabInteractable.trackRotation = originalTrackRotation;

            // LOGIKA BARU: Saat keluar socket/diambil, nyalakan lagi fisika normalnya
            if (rb != null)
            {
                rb.useGravity = true;
                rb.isKinematic = false;
            }
        }
        CekStatusOutline();
    }

    private void CekStatusOutline()
    {
        if (isLockedInSocket)
        {
            SetOutlineState(false);
            return;
        }

        bool sedangBerinteraksi = grabInteractable.isHovered || grabInteractable.isSelected;
        SetOutlineState(sedangBerinteraksi);
    }

    private void SetOutlineState(bool nyala)
    {
        float targetWidth = nyala ? 10f : 0f;

        if (outlinePisauAsli != null)
        {
            outlinePisauAsli.PulseWidth = nyala;
            outlinePisauAsli.OutlineWidth = targetWidth;
        }

        if (outlineBayanganSocket != null)
        {
            outlineBayanganSocket.PulseWidth = nyala;
            outlineBayanganSocket.OutlineWidth = targetWidth;
        }
        if (meshBayanganSocket != null)
        {
            meshBayanganSocket.SetActive(nyala);
        }
    }
}