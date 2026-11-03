using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class FoodStorageGrabInteractable : XRGrabInteractable
{
    [Header("Referensi Bahan")]
    public FoodIngredient foodIngredient;

    [Header("Input Select / Grab")]
    public InputActionReference leftGrabAction;
    public InputActionReference rightGrabAction;

    private float nextFeedbackTime;

    protected override void Awake()
    {
        base.Awake();

        if (foodIngredient == null)
            foodIngredient = GetComponentInParent<FoodIngredient>();

        if (foodIngredient == null)
        {
            Debug.LogError(
                name + ": FoodIngredient belum ditemukan.",
                this
            );
        }
    }

    // Pengecekan izin saja, tanpa menulis notifikasi.
    public override bool IsSelectableBy(IXRSelectInteractor interactor)
    {
        if (!base.IsSelectableBy(interactor))
            return false;

        if (foodIngredient == null)
            return false;

        // Pertahankan pegangan setelah pengambilan dicatat.
        if (interactorsSelecting.Contains(interactor))
            return true;

        if (foodIngredient.isTaken)
            return false;

        // Tahap 1: bahan bebas dibawa ke penyimpanan.
        if (!foodIngredient.isStored)
            return true;

        // Tahap 2: izin mengambil ditentukan manager.
        FoodStorageManager manager = foodIngredient.storageManager;

        return manager != null &&
               manager.CanTakeIngredient(foodIngredient, false);
    }

    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);

        if (foodIngredient == null)
            return;

        // Grab awal untuk menyimpan tidak dihitung sebagai pengambilan.
        if (!foodIngredient.isStored || foodIngredient.isTaken)
            return;

        FoodStorageManager manager = foodIngredient.storageManager;

        if (manager != null)
            manager.TryTakeIngredient(foodIngredient);
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        if (foodIngredient != null && foodIngredient.storageManager != null)
            foodIngredient.storageManager.StartCoroutine(
                ResolveRelease(foodIngredient));
    }

    private static System.Collections.IEnumerator ResolveRelease(FoodIngredient item)
    {
        // Tunggu XR menyelesaikan detach dan kecepatan lempar.
        yield return null;
        if (item == null) yield break;
        var grab = item.GetComponentInChildren<FoodStorageGrabInteractable>();
        if (grab != null && grab.isSelected) yield break;
        if (item.storageManager != null)
            item.storageManager.FinishPlacement(item);
    }

    private void Update()
    {
        if (foodIngredient == null ||
            !foodIngredient.isStored ||
            foodIngredient.isTaken ||
            !isHovered ||
            isSelected)
        {
            return;
        }

        bool pressed =
            WasPressed(leftGrabAction) ||
            WasPressed(rightGrabAction);

        if (!pressed || Time.unscaledTime < nextFeedbackTime)
            return;

        FoodStorageManager manager = foodIngredient.storageManager;

        if (manager == null)
            return;

        // Tampilkan alasan penolakan saat tombol grab ditekan.
        if (!manager.CanTakeIngredient(foodIngredient, false))
        {
            nextFeedbackTime = Time.unscaledTime + 0.5f;
            manager.CanTakeIngredient(foodIngredient, true);
        }
    }

    private static bool WasPressed(InputActionReference reference)
    {
        return reference != null &&
               reference.action != null &&
               reference.action.WasPressedThisFrame();
    }
}