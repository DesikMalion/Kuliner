
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(BoxCollider))]
public class FoodSortingArea : MonoBehaviour
{
    public enum SortingAreaType
    {
        Nabati,
        Hewani,
        BahanRusak
    }

    [Header("Tipe Area")]
    public SortingAreaType areaType;

    [Header("Manager")]
    public FoodIdentificationManager manager;

    [Header("Material Feedback")]
    public Renderer[] areaRenderers;
    public Material correctMaterial;
    public Material wrongMaterial;

    private readonly Dictionary<Renderer, Material[]> originalMaterials =
        new Dictionary<Renderer, Material[]>();

    private readonly Dictionary<Collider, FoodIngredient> touching =
        new Dictionary<Collider, FoodIngredient>();

    private readonly List<Collider> removedColliders =
        new List<Collider>();

    // 0 = Default, 1 = Benar, 2 = Salah.
    private int currentState = -1;

    private void Awake()
    {
        if (areaRenderers == null || areaRenderers.Length == 0)
            areaRenderers = GetComponentsInChildren<Renderer>(true);

        foreach (Renderer rend in areaRenderers)
        {
            if (rend != null && !originalMaterials.ContainsKey(rend))
            {
                originalMaterials.Add(rend, rend.sharedMaterials);
            }
        }

        SetVisualState(0);
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

        touching[other] = ingredient;

        if (manager != null)
        {
            manager.CheckIngredient(ingredient, areaType);
        }
        else
        {
            Debug.LogWarning(
                "FoodIdentificationManager belum dihubungkan!",
                this
            );
        }

        RefreshVisual();
    }

    private void OnTriggerStay(Collider other)
    {
        FoodIngredient ingredient =
            other.GetComponentInParent<FoodIngredient>();

        if (ingredient != null)
            touching[other] = ingredient;
    }

    private void OnTriggerExit(Collider other)
    {
        if (touching.Remove(other))
            RefreshVisual();
    }

    private void LateUpdate()
    {
        removedColliders.Clear();

        foreach (var entry in touching)
        {
            if (entry.Key == null ||
                !entry.Key.enabled ||
                !entry.Key.gameObject.activeInHierarchy ||
                entry.Value == null)
            {
                removedColliders.Add(entry.Key);
            }
        }

        foreach (Collider col in removedColliders)
            touching.Remove(col);

        // Periksa status grab dan release setiap frame.
        RefreshVisual();
    }

    private bool IsGrabbed(FoodIngredient ingredient)
    {
        UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable grab =
            ingredient.GetComponentInParent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable>();

        return grab != null && grab.isSelected;
    }

    private bool IsCorrectArea(FoodIngredient ingredient)
    {
        if (ingredient.condition ==
            FoodIngredient.FoodCondition.Rusak)
        {
            return areaType == SortingAreaType.BahanRusak;
        }

        switch (ingredient.category)
        {
            case FoodIngredient.FoodCategory.Nabati:
                return areaType == SortingAreaType.Nabati;

            case FoodIngredient.FoodCategory.Hewani:
                return areaType == SortingAreaType.Hewani;

            default:
                return false;
        }
    }

    private void RefreshVisual()
    {
        bool hasGrabbedIngredient = false;

        foreach (FoodIngredient ingredient in touching.Values)
        {
            if (ingredient == null)
                continue;

            // Area hanya memberikan feedback saat bahan di-grab.
            if (!IsGrabbed(ingredient))
                continue;

            hasGrabbedIngredient = true;

            if (!IsCorrectArea(ingredient))
            {
                SetVisualState(2);
                return;
            }
        }

        if (hasGrabbedIngredient)
            SetVisualState(1);
        else
            SetVisualState(0);
    }

    private void SetVisualState(int state)
    {
        if (currentState == state)
            return;

        currentState = state;

        Material feedbackMaterial = null;

        if (state == 1)
            feedbackMaterial = correctMaterial;
        else if (state == 2)
            feedbackMaterial = wrongMaterial;

        foreach (var entry in originalMaterials)
        {
            Renderer rend = entry.Key;

            if (rend == null)
                continue;

            if (state == 0 || feedbackMaterial == null)
            {
                rend.sharedMaterials = entry.Value;
                continue;
            }

            Material[] materials =
                new Material[entry.Value.Length];

            for (int i = 0; i < materials.Length; i++)
                materials[i] = feedbackMaterial;

            rend.sharedMaterials = materials;
        }
    }

    private void OnDisable()
    {
        touching.Clear();
        SetVisualState(0);
    }
}
