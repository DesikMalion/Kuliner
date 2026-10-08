using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class CookingStation : MonoBehaviour
{
    public bool isHot = true;
    public Transform slot_place;
    private Ingredient currentIngredient;


    // =========================================================
    // OBJECT MASUK STATION
    // =========================================================

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(
            "TRIGGER MASUK: " +
            other.name
        );

        Ingredient ingredient =
            other.GetComponent<Ingredient>();

        if (ingredient == null)
            return;


        currentIngredient = ingredient;


        // =====================================================
        // GENERIC EVENT
        // =====================================================

        CookingManager.Instance.CheckEvent(
            CookingEventType.ObjectPlaced,
            ingredient.gameObject
        );
        if (ingredient.cookingProgress <=0)
        {
             ReleaseGrab(ingredient);

            // =====================================================
            // SNAP KE TEMPAT GRILL
            // =====================================================

            ingredient.transform.SetPositionAndRotation(
                slot_place.position,
                slot_place.rotation
            );
        }

        // =====================================================
        // MULAI MEMASAK
        // =====================================================

        if (isHot)
        {
   


            ingredient.StartCooking();
        }
    }
public void StartCooking()
    {
        currentIngredient.StartCooking();
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
            ingredient.SetRigIngredient();
        }
    }
}
    // =========================================================
    // OBJECT KELUAR STATION
    // =========================================================

    private void OnTriggerExit(Collider other)
    {
        Debug.Log(
            "TRIGGER KELUAR: " +
            other.name
        );

        Ingredient ingredient =
            other.GetComponent<Ingredient>();

        if (ingredient == null)
            return;


        if (ingredient == currentIngredient)
        {
            ingredient.StopCooking();

            currentIngredient = null;
        }
    }
}