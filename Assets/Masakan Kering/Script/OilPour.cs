using UnityEngine;

public class OilPour : MonoBehaviour
{
    [Header("Oil Point")]
    public Transform oilPoint;

    [Header("Grab")]
    public UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grabInteractable;

    private bool hasPoured = false;

    // =====================================================
    // POSISI AWAL
    // =====================================================

    private Vector3 startPosition;
    private Quaternion startRotation;
    private Transform oilTransform;


    private void Awake()
    {
        
        if (grabInteractable == null)
        {
            grabInteractable =
                GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();
        }

        // Karena yang dipindahkan adalah transform.parent
        oilTransform = transform.parent;

        if (oilTransform != null)
        {
            startPosition = oilTransform.position;
            startRotation = oilTransform.rotation;
        }
        else
        {
            startPosition = transform.position;
            startRotation = transform.rotation;
        }
    }


    private void OnTriggerEnter(Collider other)
    {
        if (hasPoured)
            return;

        CookingPan pan = other.GetComponent<CookingPan>();

        if (pan == null)
            return;

        hasPoured = true;


        // =====================================================
        // LEPAS DARI GRAB VR
        // =====================================================

        if (grabInteractable != null &&
            grabInteractable.isSelected)
        {
            var interactors =
                grabInteractable.interactorsSelecting;

            if (interactors.Count > 0)
            {
                grabInteractable.interactionManager.SelectExit(
                    interactors[0],
                    grabInteractable
                );
            }
        }


        // =====================================================
        // PINDAHKAN KE OIL POINT
        // =====================================================

        if (oilPoint != null)
        {
            if (oilTransform != null)
            {
                oilTransform.position = oilPoint.position;
                oilTransform.rotation = oilPoint.rotation;
            }
            else
            {
                transform.position = oilPoint.position;
                transform.rotation = oilPoint.rotation;
            }
        }


        // =====================================================
        // WAJAN DIBERI MINYAK
        // =====================================================

        pan.AddOil();
    }


    // =====================================================
    // RESET OIL POUR
    // =====================================================

    public void ResetOil()
    {
        hasPoured = false;

        // Kembalikan posisi dan rotasi awal
        if (oilTransform != null)
        {
            oilTransform.position = startPosition;
            oilTransform.rotation = startRotation;
        }
        else
        {
            transform.position = startPosition;
            transform.rotation = startRotation;
        }

        // Pastikan tidak sedang di-grab
        if (grabInteractable != null &&
            grabInteractable.isSelected)
        {
            var interactors =
                grabInteractable.interactorsSelecting;

            if (interactors.Count > 0)
            {
                grabInteractable.interactionManager.SelectExit(
                    interactors[0],
                    grabInteractable
                );
            }
        }
    }
}