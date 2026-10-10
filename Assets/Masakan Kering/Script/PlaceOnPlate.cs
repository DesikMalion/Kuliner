
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class PlaceOnPlate : MonoBehaviour
{
    [Header("Legacy Slot - Tetap Dipakai")]
    public PlateSlot targetSlot;

    [Header("Auto Slot - Untuk Banyak Tempe")]
    [Tooltip("Isi jika objek menggunakan sistem slot otomatis.")]
    public PlateSlotManager slotManager;

    [Header("Existing Settings")]
    public GameObject tools_used;
    public bool isPlaced;
    public bool isCake;
    public GameObject matang;

    private void OnTriggerEnter(Collider other)
    {
        if (isPlaced)
            return;

        PlateSlot slot = other.GetComponent<PlateSlot>();

        if (slot == null)
            return;

        // =====================================================
        // VALIDASI SLOT
        // =====================================================

        if (slotManager != null)
        {
            // Sistem otomatis: hanya slot kosong berikutnya.
            PlateSlot nextSlot = slotManager.GetNextAvailableSlot();

            if (slot != nextSlot)
                return;
        }
        else
        {
            // Sistem lama: targetSlot tetap digunakan.
            if (slot != targetSlot)
                return;
        }

        // =====================================================
        // CEK INGREDIENT
        // =====================================================

        Ingredient ingredient = GetComponent<Ingredient>();

        if (ingredient == null)
            return;

        if (!ingredient.isCooked)
        {
            Debug.Log(gameObject.name + " belum matang.");
            return;
        }

        // =====================================================
        // KUNCI SLOT OTOMATIS
        // =====================================================

        if (slotManager != null)
        {
            if (!slotManager.TryOccupySlot(slot))
                return;
        }

        // =====================================================
        // PLACE OBJECT
        // =====================================================

        isPlaced = true;

        ReleaseGrab(ingredient);

        ingredient.transform.SetParent(slot.transform);

        ingredient.transform.SetPositionAndRotation(
            slot.place_slot.position,
            slot.place_slot.rotation
        );

        // =====================================================
        // HIDE TOOLS
        // =====================================================

        if (tools_used != null)
        {
            tools_used.SetActive(false);
        }

        // =====================================================
        // SHOW COOKED VISUAL
        // =====================================================

        if (matang != null)
        {
            ingredient.gameObject.SetActive(false);
            matang.SetActive(true);
        }

        // =====================================================
        // CAKE VISUAL
        // =====================================================

        if (isCake && ingredient.transform.childCount > 0)
        {
            MeshRenderer meshRenderer =
                ingredient.transform.GetChild(0)
                .GetComponent<MeshRenderer>();

            if (meshRenderer != null)
                meshRenderer.enabled = false;
        }

        Debug.Log(
            gameObject.name + " diletakkan di slot " + slot.name
        );

        // =====================================================
        // COOKING EVENT
        // =====================================================

        if (CookingManager.Instance != null)
        {
            CookingManager.Instance.CheckEvent(
                CookingEventType.ObjectPlacedOnPlate,
                gameObject
            );
        }
    }

    // =====================================================
    // RELEASE XR GRAB
    // =====================================================

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
