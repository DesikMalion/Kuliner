using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Teleportation;

public class HandTeleportProvider : MonoBehaviour
{
    [Header("References")]
    public XRRayInteractor rayInteractor;
    public TeleportationProvider teleportationProvider;

    // Function in the "Gesture Ended" event of a closed hand.
    public void TryTeleport()
    {
        if (rayInteractor == null || teleportationProvider == null)
        {
            Debug.LogError("Missing references to HandTeleportProvider!");
            gameObject.SetActive(false);
            return;
        }

        // 1. We check exactly what the ray is hitting right now.
        if (rayInteractor.TryGetCurrent3DRaycastHit(out RaycastHit hit))
        {
            // 2. We look for the teleportation component in the targeted object (or its parents).
            BaseTeleportationInteractable teleportObject =
                hit.collider.GetComponentInParent<BaseTeleportationInteractable>();

            if (teleportObject != null)
            {
                // 3. If we find a suitable floor, we prepare the teleport.
                TeleportRequest request = new TeleportRequest();

                // If it's an anchor (fixed point), we use the anchor's position.
                if (teleportObject is TeleportationAnchor anchor)
                {
                    request.destinationPosition =
                        anchor.teleportAnchorTransform.position;

                    request.destinationRotation =
                        anchor.teleportAnchorTransform.rotation;
                }
                // If it's an area (rental floor), we use the exact point where the ray touches.
                else
                {
                    request.destinationPosition = hit.point;
                    request.destinationRotation =
                        teleportationProvider.transform.rotation;
                }

                // 4. TRIGGER THE TELEPORT
                teleportationProvider.QueueTeleportRequest(request);
            }
        }

        // 5. Turn off the line.
        gameObject.SetActive(false);
    }
}