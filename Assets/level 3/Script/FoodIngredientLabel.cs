using System.Collections;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(FoodStorageGrabInteractable))]
public class FoodIngredientLabel : MonoBehaviour
{
    [Header("Informasi Tambahan")]
    [TextArea(2, 4)]
    public string description;

    [Header("Posisi Label")]
    [Tooltip("Offset posisi label dari pivot bahan dalam world axis.")]
    public Vector3 labelOffset = new Vector3(0f, 0.25f, 0f);

    [Header("Stabilitas Label")]
    [Tooltip("Jeda sebelum label hilang setelah hover keluar.")]
    [Min(0f)]
    public float hideDelay = 100;

    private FoodIngredient ingredient;
    private FoodStorageGrabInteractable interactable;

    private bool isHovering;
    private bool isHeld;

    private Coroutine hideCoroutine;
    /// <summary>
    /// Called when the script is loaded or a value is changed in the
    /// inspector (Called in the editor only).
    /// </summary>
    private void OnValidate()
    {
        hideDelay = 0.2f;
    }
    private void Awake()
    {
        ingredient = GetComponent<FoodIngredient>();
        interactable = GetComponent<FoodStorageGrabInteractable>();
    }

    private void OnEnable()
    {
        if (interactable == null)
            interactable = GetComponent<FoodStorageGrabInteractable>();

        if (interactable == null)
            return;

        interactable.hoverEntered.AddListener(OnHoverEntered);
        interactable.hoverExited.AddListener(OnHoverExited);
        interactable.selectEntered.AddListener(OnSelectEntered);
        interactable.selectExited.AddListener(OnSelectExited);
    }

    private void OnDisable()
    {
        if (interactable != null)
        {
            interactable.hoverEntered.RemoveListener(OnHoverEntered);
            interactable.hoverExited.RemoveListener(OnHoverExited);
            interactable.selectEntered.RemoveListener(OnSelectEntered);
            interactable.selectExited.RemoveListener(OnSelectExited);
        }

        CancelHideCoroutine();

        isHovering = false;
        isHeld = false;

        if (FoodIngredientLabelManager.Instance != null)
            FoodIngredientLabelManager.Instance.Hide(transform);
    }

    private void OnHoverEntered(HoverEnterEventArgs args)
    {
        isHovering = true;

        // Jika hover masuk lagi sebelum waktu habis,
        // batalkan proses menyembunyikan label.
        CancelHideCoroutine();

        RefreshLabel();
    }

    private void OnHoverExited(HoverExitEventArgs args)
    {
        isHovering = false;

        // Jangan langsung menyembunyikan label.
        // Beri toleransi agar label tidak berkedip.
        ScheduleHideIfNeeded();
    }

    private void OnSelectEntered(SelectEnterEventArgs args)
    {
        isHeld = true;

        CancelHideCoroutine();
        RefreshLabel();
    }

    private void OnSelectExited(SelectExitEventArgs args)
    {
        isHeld = false;

        // Kalau masih hover, label tetap tampil.
        if (isHovering)
        {
            CancelHideCoroutine();
            RefreshLabel();
        }
        else
        {
            ScheduleHideIfNeeded();
        }
    }

    private void RefreshLabel()
    {
        FoodIngredientLabelManager manager =
            FoodIngredientLabelManager.Instance;

        if (manager == null)
            return;

        if (isHovering || isHeld)
        {
            if (ingredient == null)
                ingredient = GetComponent<FoodIngredient>();

            manager.Show(
                ingredient,
                transform,
                description,
                labelOffset
            );
        }
    }

    private void ScheduleHideIfNeeded()
    {
        CancelHideCoroutine();

        if (!isHovering && !isHeld)
        {
            hideCoroutine = StartCoroutine(HideAfterDelay());
        }
    }

    private IEnumerator HideAfterDelay()
    {
        yield return new WaitForSeconds(hideDelay);

        // Periksa ulang status sebelum benar-benar menyembunyikan.
        if (!isHovering && !isHeld)
        {
            if (FoodIngredientLabelManager.Instance != null)
                FoodIngredientLabelManager.Instance.Hide(transform);
        }

        hideCoroutine = null;
    }

    private void CancelHideCoroutine()
    {
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
            hideCoroutine = null;
        }
    }
}