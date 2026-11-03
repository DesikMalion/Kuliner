
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRBaseInteractable))]
public class FoodCorrectHoverHologram : MonoBehaviour
{
    [Header("Referensi")]
    public FoodIngredient foodIngredient;

    [Tooltip("Mesh duplikat untuk efek hologram.")]
    public GameObject hologramVisual;

    [Header("Material Hologram")]
    public Material greenHologramMaterial;
    public Material redHologramMaterial;

    private XRBaseInteractable interactable;
    private Renderer[] hologramRenderers;
    private bool isHovering;

    private void Awake()
    {
        interactable = GetComponent<XRBaseInteractable>();

        if (foodIngredient == null)
            foodIngredient = GetComponent<FoodIngredient>();

        if (hologramVisual != null)
        {
            hologramRenderers =
                hologramVisual.GetComponentsInChildren<Renderer>(true);

            hologramVisual.SetActive(false);
        }
    }

    private void OnEnable()
    {
        if (interactable == null)
            interactable = GetComponent<XRBaseInteractable>();

        if (interactable != null)
        {
            interactable.hoverEntered.AddListener(OnHoverEntered);
            interactable.hoverExited.AddListener(OnHoverExited);
        }
    }

    private void OnDisable()
    {
        if (interactable != null)
        {
            interactable.hoverEntered.RemoveListener(OnHoverEntered);
            interactable.hoverExited.RemoveListener(OnHoverExited);
        }

        isHovering = false;

        if (hologramVisual != null)
            hologramVisual.SetActive(false);
    }

    private void Update()
    {
        UpdateHologram();
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        isHovering = true;
        UpdateHologram();
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        isHovering = false;
        UpdateHologram();
    }

    private void UpdateHologram()
    {
        if (hologramVisual == null || foodIngredient == null)
            return;

        bool shouldShow = isHovering;

        if (hologramVisual.activeSelf != shouldShow)
            hologramVisual.SetActive(shouldShow);

        if (!shouldShow)
            return;

        Material selectedMaterial =
            foodIngredient.isCorrectlySorted
            ? greenHologramMaterial
            : redHologramMaterial;

        if (selectedMaterial == null || hologramRenderers == null)
            return;

        foreach (Renderer rend in hologramRenderers)
        {
            if (rend == null)
                continue;

            // Gunakan material yang sama untuk semua slot hologram.
            Material[] materials = rend.sharedMaterials;

            for (int i = 0; i < materials.Length; i++)
                materials[i] = selectedMaterial;

            rend.sharedMaterials = materials;
        }
    }
}
