using System.Collections.Generic;
using UnityEngine;

public class FoodStorageArea : MonoBehaviour
{
    public enum StorageAreaType
    {
        Chilling,
        Freezing,
        GudangKering
    }

    [Header("Tipe Area")]
    public StorageAreaType areaType;

    [Header("Manager")]
    public FoodStorageManager manager;

    [Header("Slot Penyimpanan")]
    public Transform[] slots;

    [Header("Material Feedback")]
    public Renderer[] areaRenderers;
    public Material correctMaterial;
    public Material wrongMaterial;

    private FoodIngredient[] occupiedSlots;

    private readonly Dictionary<Renderer, Material[]> originalMaterials =
        new Dictionary<Renderer, Material[]>();

    private readonly Dictionary<FoodIngredient, int> overlaps =
        new Dictionary<FoodIngredient, int>();

    private void Awake()
    {
        occupiedSlots = new FoodIngredient[
            slots != null ? slots.Length : 0
        ];

        if (areaRenderers == null || areaRenderers.Length == 0)
            areaRenderers = GetComponentsInChildren<Renderer>(true);

        // Simpan seluruh slot material pada setiap renderer.
        foreach (Renderer rend in areaRenderers)
        {
            if (rend != null && !originalMaterials.ContainsKey(rend))
                originalMaterials.Add(rend, rend.sharedMaterials);
        }
    }

   private void OnTriggerEnter(Collider other)
{
    FoodIngredient ingredient =
        other.GetComponentInParent<FoodIngredient>();

    if (ingredient == null)
        return;

    if (overlaps.ContainsKey(ingredient))
        overlaps[ingredient]++;
    else
        overlaps.Add(ingredient, 1);

    UpdateAreaMaterial();
}

private void OnTriggerStay(Collider other)
{
    FoodIngredient ingredient =
        other.GetComponentInParent<FoodIngredient>();

    if (ingredient == null)
        return;

    UpdateAreaMaterial();

    if (ingredient.isStored || ingredient.isTaken)
        return;

    FoodStorageGrabInteractable grab =
        ingredient.GetComponentInChildren<
            FoodStorageGrabInteractable>();

    // Selama masih dipegang, hanya tampilkan feedback.
    if (grab != null && grab.isSelected)
        return;

    // Setelah dilepas, baru coba masukkan ke slot.
    TryStoreIngredient(ingredient);
}

private void TryStoreIngredient(FoodIngredient ingredient)
{
    if (manager == null ||
        ingredient == null ||
        ingredient.isStored ||
        ingredient.isTaken)
    {
        return;
    }

    int slotIndex = FindEmptySlot();

    if (slotIndex == -1)
    {
        manager.ReturnIngredientToStart(ingredient);
        manager.ShowAreaFeedback(
            "Slot " + areaType + " penuh!"
        );
        return;
    }

    bool success = manager.RegisterStored(
        ingredient,
        areaType,
        this,
        slotIndex
    );

    if (!success)
    {
        manager.ReturnIngredientToStart(ingredient);
        return;
    }

    occupiedSlots[slotIndex] = ingredient;

    Transform slot = slots[slotIndex];
    Rigidbody rb = ingredient.GetComponent<Rigidbody>();

    if (rb != null)
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        rb.position = slot.position;
        rb.rotation = slot.rotation;
    }

    ingredient.transform.SetPositionAndRotation(
        slot.position,
        slot.rotation
    );

    manager.RefreshTakeAvailability();
    UpdateAreaMaterial();
}

    private void OnTriggerExit(Collider other)
    {
        FoodIngredient ingredient =
            other.GetComponentInParent<FoodIngredient>();

        if (ingredient == null || !overlaps.ContainsKey(ingredient))
            return;

        overlaps[ingredient]--;

        if (overlaps[ingredient] <= 0)
            overlaps.Remove(ingredient);

        UpdateAreaMaterial();
    }

    private void UpdateAreaMaterial()
{
    bool adaBahan = false;
    bool semuaSesuai = true;

    foreach (FoodIngredient ingredient in overlaps.Keys)
    {
        if (ingredient == null)
            continue;

        // Sudah masuk slot: tidak memengaruhi feedback.
        if (ingredient.isStored || ingredient.isTaken)
            continue;

        adaBahan = true;

        bool sesuai =
            ingredient.condition !=
                FoodIngredient.FoodCondition.Rusak &&
            (
                (areaType == StorageAreaType.Chilling &&
                 ingredient.requiredStorage ==
                    FoodIngredient.StorageType.Chilling) ||

                (areaType == StorageAreaType.Freezing &&
                 ingredient.requiredStorage ==
                    FoodIngredient.StorageType.Freezing) ||

                (areaType == StorageAreaType.GudangKering &&
                 ingredient.requiredStorage ==
                    FoodIngredient.StorageType.GudangKering)
            );

        if (!sesuai)
        {
            semuaSesuai = false;
            break;
        }
    }

    if (!adaBahan)
    {
        RestoreOriginalMaterials();
        return;
    }

    ApplyFeedbackMaterial(
        semuaSesuai ? correctMaterial : wrongMaterial
    );
}

    private void ApplyFeedbackMaterial(Material material)
    {
        if (material == null)
        {
            RestoreOriginalMaterials();
            return;
        }

        foreach (var entry in originalMaterials)
        {
            Renderer rend = entry.Key;

            if (rend == null)
                continue;

            Material[] materials = new Material[entry.Value.Length];

            for (int i = 0; i < materials.Length; i++)
                materials[i] = material;

            rend.sharedMaterials = materials;
        }
    }

    private void RestoreOriginalMaterials()
    {
        foreach (var entry in originalMaterials)
        {
            if (entry.Key != null)
                entry.Key.sharedMaterials = entry.Value;
        }
    }

    private int FindEmptySlot()
    {
        for (int i = 0; i < occupiedSlots.Length; i++)
        {
            if (slots[i] != null && occupiedSlots[i] == null)
                return i;
        }

        return -1;
    }

    public void ReleaseSlot(FoodIngredient ingredient)
    {
        for (int i = 0; i < occupiedSlots.Length; i++)
        {
            if (occupiedSlots[i] == ingredient)
                occupiedSlots[i] = null;
        }
    }

    public void ClearSlotOccupancy()
    {
        for (int i = 0; i < occupiedSlots.Length; i++)
            occupiedSlots[i] = null;

        overlaps.Clear();
        RestoreOriginalMaterials();
    }

    private void OnDisable()
    {
        overlaps.Clear();
        RestoreOriginalMaterials();
    }
}