
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class CookingStation : MonoBehaviour
{
    [Header("Main Slot - Slot Lama")]
    public bool isHot = true;
    public Transform slot_place;

    [Header("Additional Slots - Slot Tambahan")]
    public Transform[] additionalSlots;

    // Ingredient yang sedang berada di station.
    private readonly HashSet<Ingredient> cookingIngredients =
        new HashSet<Ingredient>();

    // Mencatat slot yang ditempati setiap ingredient.
    private readonly Dictionary<Ingredient, Transform> occupiedSlots =
        new Dictionary<Ingredient, Transform>();

    // Mencegah OnTriggerEnter memproses ingredient yang sama berulang kali.
    private readonly HashSet<Ingredient> processedIngredients =
        new HashSet<Ingredient>();

    [SerializeField]private Ingredient currentIngredient;

    // =========================================================
    // TRIGGER MASUK
    // =========================================================

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("TRIGGER MASUK: " + other.name);

        Ingredient ingredient = other.GetComponent<Ingredient>();

        // Coba cari Ingredient pada parent jika collider berada di child.
        if (ingredient == null)
            ingredient = other.GetComponentInParent<Ingredient>();

        if (ingredient == null)
            return;

        // Cegah collider child memproses ingredient yang sama berulang kali.
        if (processedIngredients.Contains(ingredient))
            return;

        Transform selectedSlot = null;

        // Pertahankan mekanisme lama:
        // ingredient baru ditempatkan ke slot ketika progress <= 0.
        if (ingredient.cookingProgress <= 0)
        {
            selectedSlot = GetFirstAvailableSlot();

            // Jangan menerima ingredient jika semua slot sudah terisi.
            if (selectedSlot == null)
            {
                Debug.LogWarning(
                    "Semua slot station penuh! Ingredient: "
                    + ingredient.ingredientName
                );
                return;
            }
        }

        processedIngredients.Add(ingredient);
        currentIngredient = ingredient;

        CookingManager.Instance.CheckEvent(
            CookingEventType.ObjectPlaced,
            ingredient.gameObject
        );

        if (ingredient.cookingProgress <= 0)
        {
            // Lepaskan ingredient dari tangan.
            ReleaseGrab(ingredient);

            // Snap ke posisi dan rotasi slot yang dipilih.
            ingredient.transform.SetPositionAndRotation(
                selectedSlot.position,
                selectedSlot.rotation
            );

            // Catat bahwa slot ini sekarang ditempati.
            occupiedSlots[ingredient] = selectedSlot;

            // Pertahankan perilaku slot lama.
            DisableGrab(ingredient);

            Debug.Log(
                ingredient.ingredientName
                + " masuk slot: " + selectedSlot.name
            );
        }

        // Mulai memasak jika station sedang panas.
        if (isHot)
        {
            StartCookingIngredient(ingredient);
        }
    }

    // =========================================================
    // MENCARI SLOT KOSONG
    // =========================================================

    private Transform GetFirstAvailableSlot()
    {
        // Prioritas pertama adalah slot lama.
        if (slot_place != null && !IsSlotOccupied(slot_place))
        {
            return slot_place;
        }

        // Jika slot lama penuh, periksa slot tambahan berurutan.
        if (additionalSlots != null)
        {
            for (int i = 0; i < additionalSlots.Length; i++)
            {
                Transform slot = additionalSlots[i];

                if (slot == null)
                    continue;

                if (!IsSlotOccupied(slot))
                    return slot;
            }
        }

        return null;
    }

    private bool IsSlotOccupied(Transform slot)
    {
        foreach (KeyValuePair<Ingredient, Transform> entry in occupiedSlots)
        {
            if (entry.Value == slot)
                return true;
        }

        return false;
    }

    // =========================================================
    // MEMULAI MEMASAK
    // =========================================================

    private void StartCookingIngredient(Ingredient ingredient)
    {
        if (ingredient == null)
            return;

        cookingIngredients.Add(ingredient);
        Debug.Log("HARUSNYA INI MULAI START COOKING "+ingredient.gameObject.name);
        ingredient.StartCooking();
    }

    public void StartCooking()
    {
        // // Mempertahankan fungsi publik yang digunakan script lain.
        // // Semua ingredient yang terdaftar akan mulai memasak.
        // List<Ingredient> ingredients = new List<Ingredient>(
        //     cookingIngredients
        // );
        // Debug.Log("INI KEBACA GAKKKASDOIHASIUHUIHEW");
        // foreach (Ingredient ingredient in ingredients)
        // {
        //     if (ingredient != null)
                currentIngredient.StartCooking();
       // }
    }

    // =========================================================
    // MELEPASKAN GRAB
    // =========================================================

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
    // MENONAKTIFKAN GRAB
    // =========================================================

    private void DisableGrab(Ingredient ingredient)
    {
        XRGrabInteractable grab =
            ingredient.GetComponent<XRGrabInteractable>();

        if (grab == null)
            return;

        grab.enabled = false;

        Debug.Log(
            "XRGrabInteractable DIMATIKAN: "
            + ingredient.ingredientName
        );
    }

    // =========================================================
    // TRIGGER KELUAR
    // =========================================================

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("TRIGGER KELUAR: " + other.name);

        Ingredient ingredient = other.GetComponent<Ingredient>();

        if (ingredient == null)
            ingredient = other.GetComponentInParent<Ingredient>();

        if (ingredient == null)
            return;

        // Hentikan proses memasak ingredient yang keluar.
        if (cookingIngredients.Contains(ingredient))
        {
            //ingredient.StopCooking();
            cookingIngredients.Remove(ingredient);
        }

        // Kosongkan slot yang sebelumnya ditempati.
        if (occupiedSlots.ContainsKey(ingredient))
        {
            Transform releasedSlot = occupiedSlots[ingredient];

            occupiedSlots.Remove(ingredient);

            Debug.Log(
                "Slot tersedia kembali: "
                + (releasedSlot != null ? releasedSlot.name : "Unknown")
            );
        }

        processedIngredients.Remove(ingredient);

        if (currentIngredient == ingredient)
        {
            //currentIngredient = null;
        }
    }
}
