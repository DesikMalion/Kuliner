using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class FoodTakeArea : MonoBehaviour
{
    [Header("Manager")]
    public FoodStorageManager manager;

    [Header("Bahan yang Diizinkan")]
    public FoodIngredient[] allowedIngredients;

    [Header("Slot Tujuan")]
    public Transform[] slots;

    [Header("Material Feedback")]
    public Renderer[] areaRenderers;
    public Material correctMaterial;
    public Material wrongMaterial;

    private readonly Dictionary<Renderer, Material[]> originalMaterials =
        new Dictionary<Renderer, Material[]>();

    private readonly Dictionary<Transform, FoodIngredient> occupied =
        new Dictionary<Transform, FoodIngredient>();

    private readonly Dictionary<FoodIngredient, int> overlaps =
        new Dictionary<FoodIngredient, int>();

    private void Awake()
    {
        if (areaRenderers == null || areaRenderers.Length == 0)
            areaRenderers = GetComponentsInChildren<Renderer>(true);

        // Simpan seluruh slot material pada setiap renderer.
        foreach (Renderer rend in areaRenderers)
        {
            if (rend != null && !originalMaterials.ContainsKey(rend))
                originalMaterials.Add(rend, rend.sharedMaterials);
        }
    }

    private void Reset()
    {
        GetComponent<BoxCollider>().isTrigger = true;
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
        if (other.GetComponentInParent<FoodIngredient>() != null)
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
        if (IsOccupiedBy(ingredient) || ingredient.isTaken)
            continue;

        adaBahan = true;

        bool sesuai =
            manager != null &&
            manager.CanPlaceInTakeArea(ingredient, this);

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

    public bool IsOccupiedBy(FoodIngredient ingredient)
    {
        foreach (FoodIngredient item in occupied.Values)
        {
            if (item == ingredient)
                return true;
        }

        return false;
    }

    public bool Contains(Vector3 position)
    {
        BoxCollider box = GetComponent<BoxCollider>();

        return box != null &&
            box.enabled &&
            gameObject.activeInHierarchy &&
            (box.ClosestPoint(position) - position).sqrMagnitude
                < 0.000001f;
    }

    public bool Contains(FoodIngredient ingredient)
    {
        BoxCollider box = GetComponent<BoxCollider>();

        if (ingredient == null || box == null ||
            !box.enabled || !isActiveAndEnabled)
            return false;

        if (Contains(ingredient.transform.position))
            return true;

        foreach (Collider part in
            ingredient.GetComponentsInChildren<Collider>())
        {
            if (!part.enabled || part.isTrigger ||
                !part.gameObject.activeInHierarchy)
                continue;

            if (Physics.ComputePenetration(
                box,
                box.transform.position,
                box.transform.rotation,
                part,
                part.transform.position,
                part.transform.rotation,
                out Vector3 direction,
                out float distance))
            {
                return true;
            }
        }

        return false;
    }

    public bool Accepts(FoodIngredient ingredient)
    {
        return ingredient != null &&
            (allowedIngredients == null ||
             allowedIngredients.Length == 0 ||
             System.Array.IndexOf(
                 allowedIngredients, ingredient) >= 0);
    }

    public bool TryGetSlot(out Transform slot)
    {
        if (slots != null)
        {
            foreach (Transform candidate in slots)
            {
                if (candidate != null &&
                    (!occupied.TryGetValue(
                        candidate, out FoodIngredient item) ||
                     item == null || !item.isTaken))
                {
                    slot = candidate;
                    return true;
                }
            }
        }

        slot = null;
        return false;
    }

    public void Occupy(Transform slot, FoodIngredient ingredient)
    {
        occupied[slot] = ingredient;
        UpdateAreaMaterial();
    }

    public void ClearSlots()
    {
        occupied.Clear();
        overlaps.Clear();
        RestoreOriginalMaterials();
    }

    private void OnDisable()
    {
        overlaps.Clear();
        RestoreOriginalMaterials();
    }
}