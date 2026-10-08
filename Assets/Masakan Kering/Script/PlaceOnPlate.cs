using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class PlaceOnPlate : MonoBehaviour
{
     public PlateSlot targetSlot;

    private bool isPlaced;
    public bool isCake;

    public GameObject matang;
    private void OnTriggerEnter(Collider other)
    {
        if (isPlaced)
            return;

        PlateSlot slot =
            other.GetComponent<PlateSlot>();

        if (slot == null)
            return;

        if (slot != targetSlot)
            return;

        Ingredient ingredient =
            GetComponent<Ingredient>();

        if (ingredient == null)
            return;

        if (!ingredient.isCooked)
        {
            Debug.Log("Ikan belum matang.");
            return;
        }

        isPlaced = true;
 ReleaseGrab(ingredient);

            // =====================================================
            // SNAP KE TEMPAT GRILL
            // =====================================================
            ingredient.transform.SetParent(slot.transform);
            ingredient.transform.SetPositionAndRotation(
                slot.place_slot.position,
                slot.place_slot.rotation
            );

        if (matang != null)
        {
            ingredient.gameObject.SetActive(false);
            matang.gameObject.SetActive(true);
        }
        if (isCake)
        {
            ingredient.transform.GetChild(0).GetComponent<MeshRenderer>().enabled = false;
        }
        Debug.Log("Ikan diletakkan di piring.");

        CookingManager.Instance.CheckEvent(
            CookingEventType.ObjectPlacedOnPlate,
            gameObject
        );
    }
    private void ReleaseGrab(Ingredient ingredient)
{
    XRGrabInteractable grab =
        ingredient.GetComponent<XRGrabInteractable>();

    if (grab == null)
        return;

    if (grab.isSelected)
    {
        IXRSelectInteractor interactor =
            grab.firstInteractorSelecting;

        if (interactor != null)
        {
            grab.interactionManager.SelectExit(
                interactor,
                grab
            );
        }
    }
}
}
