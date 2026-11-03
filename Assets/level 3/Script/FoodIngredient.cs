
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class FoodIngredient : MonoBehaviour
{
    public enum FoodCategory
    {
        Nabati,
        Hewani
    }

    public enum FoodCondition
    {
        Segar,
        Rusak
    }

    public enum StorageType
    {
        Chilling,
        Freezing,
        GudangKering
    }

    [Header("Informasi Bahan")]
    public string ingredientName;

    public FoodCategory category;

    public FoodCondition condition = FoodCondition.Segar;

    [Header("Status Identifikasi")]
    public bool isCorrectlySorted;

    [Header("Informasi Penyimpanan")]
    public StorageType requiredStorage;

    [Tooltip("Hari simulasi saat bahan kedaluwarsa.")]
    public int expiryDay = 7;

    [Header("Status Stok")]
    public bool isStored;
    public bool isTaken;

    [HideInInspector]
    public int arrivalOrder;

    [Header("Manager Penyimpanan")]
    public FoodStorageManager storageManager;

    private XRGrabInteractable grabInteractable;

    private void OnEnable()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();

        if (grabInteractable != null)
            grabInteractable.selectEntered.AddListener(OnGrabbed);
    }

    private void OnDisable()
    {
        if (grabInteractable != null)
            grabInteractable.selectEntered.RemoveListener(OnGrabbed);
    }

    private void OnGrabbed(SelectEnterEventArgs args)
    {
        if (storageManager != null)
            storageManager.TryTakeIngredient(this);
    }

    // Untuk simulasi pertama
    public void MarkAsSorted()
    {
        isCorrectlySorted = true;
    }

    // Reset status identifikasi dan penyimpanan
    public void ResetIngredient()
    {
        isCorrectlySorted = false;

        isStored = false;
        isTaken = false;
        arrivalOrder = 0;
    }
}
