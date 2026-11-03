
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class FoodStorageManager : MonoBehaviour
{
    public enum RotationMode
    {
        FIFO,
        FEFO
    }

    [Header("Aturan Pengambilan")]
    public RotationMode rotationMode = RotationMode.FIFO;

    [Header("Daftar Bahan Simulasi 2")]
    public FoodIngredient[] ingredients;

    [Header("UI")]
    public TMP_Text feedbackText;
    public TMP_Text progressText;
public GameObject kembaliButton;
    private FoodIngredient pendingIngredient;
    private readonly Dictionary<FoodIngredient, Pose> storedPoses =
        new Dictionary<FoodIngredient, Pose>();

    private int correctCount;
    private int nextArrivalOrder = 1;
    private int totalIngredients;

    private readonly Dictionary<FoodIngredient, Vector3> startPositions =
        new Dictionary<FoodIngredient, Vector3>();

    private readonly Dictionary<FoodIngredient, Quaternion> startRotations =
        new Dictionary<FoodIngredient, Quaternion>();

    private readonly Dictionary<FoodIngredient, FoodStorageArea> storedAreas =
        new Dictionary<FoodIngredient, FoodStorageArea>();

    private void Awake()
    {
        CacheStartPositions();
    }

    private void Start()
    {
        ResetSimulation();
    }

    private void CacheStartPositions()
    {
        startPositions.Clear();
        startRotations.Clear();

        if (ingredients == null)
            return;

        foreach (FoodIngredient ingredient in ingredients)
        {
            if (ingredient == null)
                continue;

            startPositions[ingredient] = ingredient.transform.position;
            startRotations[ingredient] = ingredient.transform.rotation;
        }
    }

    // =====================================================
    // PENYIMPANAN
    // =====================================================

    public bool RegisterStored(
        FoodIngredient ingredient,
        FoodStorageArea.StorageAreaType targetArea,
        FoodStorageArea sourceArea,
        int slotIndex)
    {
        if (ingredient == null ||
            ingredient.isStored ||
            ingredient.isTaken)
            return false;

        if (ingredient.condition == FoodIngredient.FoodCondition.Rusak)
        {
            ShowAreaFeedback(
                ingredient.ingredientName +
                " rusak dan tidak boleh masuk stok layak pakai."
            );

            return false;
        }

        if (!IsCorrectStorage(ingredient, targetArea))
        {
            ShowAreaFeedback(
                "Area penyimpanan salah untuk " +
                ingredient.ingredientName +
                ". Seharusnya: " +
                ingredient.requiredStorage + "."
            );

            return false;
        }

        // Pastikan bahan terdaftar dalam daftar simulasi.
        if (ingredients == null ||
            System.Array.IndexOf(ingredients, ingredient) < 0)
        {
            ShowAreaFeedback(
                ingredient.ingredientName +
                " tidak terdaftar di FoodStorageManager."
            );

            return false;
        }

        if (sourceArea == null || sourceArea.slots == null ||
            slotIndex < 0 || slotIndex >= sourceArea.slots.Length ||
            sourceArea.slots[slotIndex] == null)
            return false;
        Transform slot = sourceArea.slots[slotIndex];
        storedPoses[ingredient] = new Pose(slot.position, slot.rotation);
        ingredient.arrivalOrder = nextArrivalOrder++;
        ingredient.isStored = true;
        ingredient.isTaken = false;

        if (sourceArea != null)
            storedAreas[ingredient] = sourceArea;

        ShowAreaFeedback(
            ingredient.ingredientName +
            " berhasil disimpan. Urutan masuk: " +
            ingredient.arrivalOrder
        );

        return true;
    }

    public bool IsCorrectStorage(
        FoodIngredient ingredient,
        FoodStorageArea.StorageAreaType targetArea)
    {
        switch (targetArea)
        {
            case FoodStorageArea.StorageAreaType.Chilling:
                return ingredient.requiredStorage ==
                    FoodIngredient.StorageType.Chilling;

            case FoodStorageArea.StorageAreaType.Freezing:
                return ingredient.requiredStorage ==
                    FoodIngredient.StorageType.Freezing;

            case FoodStorageArea.StorageAreaType.GudangKering:
                return ingredient.requiredStorage ==
                    FoodIngredient.StorageType.GudangKering;
        }

        return false;
    }

    // =====================================================
    // CEK SEMUA BAHAN SUDAH DISIMPAN
    // =====================================================

    public bool AllIngredientsStored()
    {
        if (ingredients == null || ingredients.Length == 0)
            return false;

        foreach (FoodIngredient item in ingredients)
        {
            if (item == null)
                continue;

            if (item.condition == FoodIngredient.FoodCondition.Rusak)
                continue;

            if (!item.isStored && !item.isTaken)
                return false;
        }

        return true;
    }

    // =====================================================
    // AKTIFKAN HANYA BAHAN YANG BOLEH DIAMBIL
    // =====================================================

    


public void RefreshTakeAvailability()
{
    if (ingredients == null)
        return;

    foreach (FoodIngredient item in ingredients)
    {
        if (item == null)
            continue;

        // Tetap aktif agar hover dan percobaan grab bisa dideteksi.
        // Izin mengambil diperiksa oleh IsSelectableBy().
        SetGrabEnabled(item, true);
    }
}



    private void SetGrabEnabled(
        FoodIngredient ingredient,
        bool enabledState)
    {
        FoodStorageGrabInteractable grab =
            ingredient.GetComponent<FoodStorageGrabInteractable>();

        if (grab == null)
        {
            grab = ingredient.GetComponentInChildren
                <FoodStorageGrabInteractable>();
        }

        if (grab != null && grab.enabled != enabledState)
            grab.enabled = enabledState;
    }

    // =====================================================
    // VALIDASI FIFO / FEFO
    // =====================================================

    public bool CanTakeIngredient(
    FoodIngredient ingredient,
    bool showFeedback = true)
{
    if (ingredient == null ||
        !ingredient.isStored ||
        ingredient.isTaken)
    {
        return false;
    }

    if (!AllIngredientsStored())
    {
        if (showFeedback)
        {
            ShowFeedback(
                "Simpan semua bahan layak pakai terlebih dahulu!"
            );
        }

        return false;
    }

    if (pendingIngredient != null && pendingIngredient != ingredient)
    {
        if (showFeedback)
            ShowFeedback("Letakkan bahan yang sedang dibawa terlebih dahulu.");
        return false;
    }

    FoodIngredient expected =
        GetNextIngredientToTake(ingredient.requiredStorage);

    if (expected == null)
        return false;

    if (ingredient != expected)
    {
        if (showFeedback)
        {
            string rule = rotationMode == RotationMode.FIFO
                ? "FIFO: ambil bahan yang masuk lebih dahulu."
                : "FEFO: ambil bahan dengan kedaluwarsa paling dekat.";

            ShowFeedback(
                "Urutan pengambilan salah!\n" +
                rule +
                "\nBahan berikutnya: " +
                expected.ingredientName
            );
        }

        return false;
    }

    return true;
}

    private FoodIngredient GetNextIngredientToTake(
        FoodIngredient.StorageType storageType)
    {
        FoodIngredient expected = null;

        if (ingredients == null)
            return null;

        foreach (FoodIngredient item in ingredients)
        {
            if (item == null ||
                !item.isStored ||
                item.isTaken ||
                item.requiredStorage != storageType)
                continue;

            if (expected == null || ComesBefore(item, expected))
                expected = item;
        }

        return expected;
    }

    private bool ComesBefore(
        FoodIngredient a,
        FoodIngredient b)
    {
        if (rotationMode == RotationMode.FEFO &&
            a.expiryDay != b.expiryDay)
        {
            return a.expiryDay < b.expiryDay;
        }

        return a.arrivalOrder < b.arrivalOrder;
    }

    // =====================================================
    // CATAT BAHAN BENAR-BENAR DIAMBIL
    // =====================================================

    public void TryTakeIngredient(FoodIngredient ingredient)
    {
        if (ingredient == null ||
            ingredient.isTaken ||
            !CanTakeIngredient(ingredient))
            return;

        if (pendingIngredient == ingredient)
            return;
        pendingIngredient = ingredient;
        ShowFeedback("Bawa " + ingredient.ingredientName +
            " ke area pengambilan yang sesuai, lalu lepaskan.");
    }

    // Dipanggil setelah XR selesai melepas objek.
    public void FinishPlacement(FoodIngredient ingredient)
    {
        if (pendingIngredient != ingredient || ingredient == null)
            return;

        Physics.SyncTransforms();
        string rejection = "Bahan belum menyentuh collider FoodTakeArea yang aktif.";
        FoodTakeArea matched = null;
        Transform destination = null;
        foreach (FoodTakeArea zone in FindObjectsByType<FoodTakeArea>(
            FindObjectsSortMode.None))
        {
            if (!zone.Contains(ingredient))
                continue;
            if (zone.manager != this)
            {
                rejection = "Manager pada FoodTakeArea kosong atau berbeda.";
                continue;
            }
            if (!zone.Accepts(ingredient))
            {
                rejection = "Allowed Ingredients membatasi bahan ini. Kosongkan daftar untuk menerima semua.";
                continue;
            }
            if (!zone.TryGetSlot(out destination))
            {
                rejection = "Slots tujuan belum diisi, berisi None, atau semuanya penuh.";
                continue;
            }
            matched = zone;
            break;
        }

        if (matched == null)
        {
            pendingIngredient = null;
            if (storedPoses.TryGetValue(ingredient, out Pose pose))
                MoveIngredient(ingredient, pose.position, pose.rotation);
            ShowFeedback(rejection + "\n" +
                ingredient.ingredientName + " kembali ke slot penyimpanannya.");
            RefreshTakeAvailability();
            return;
        }

        matched.Occupy(destination, ingredient);
        MoveIngredient(ingredient, destination.position, destination.rotation);
        if (storedAreas.TryGetValue(ingredient, out FoodStorageArea area) &&
            area != null)
            area.ReleaseSlot(ingredient);
        storedAreas.Remove(ingredient);
        ingredient.isStored = false;
        ingredient.isTaken = true;
        pendingIngredient = null;
        correctCount++;
        UpdateProgress();
        RefreshTakeAvailability();
        if (totalIngredients > 0 && correctCount >= totalIngredients)
{
    ShowFeedback("Simulasi penyimpanan selesai!");

    if (kembaliButton != null)
        kembaliButton.SetActive(true);
}
else
{
    ShowFeedback(
        "Benar! " + ingredient.ingredientName +
        " sudah diletakkan sesuai " + rotationMode + "."
    );
}
    }

    private static void MoveIngredient(FoodIngredient ingredient,
        Vector3 position, Quaternion rotation)
    {
        Rigidbody rb = ingredient.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.position = position;
            rb.rotation = rotation;
        }
        ingredient.transform.SetPositionAndRotation(position, rotation);
    }

    // =====================================================
    // KEMBALIKAN BAHAN KE POSISI AWAL
    // =====================================================

    public void ReturnIngredientToStart(FoodIngredient ingredient)
    {
        if (ingredient == null)
            return;

        if (storedAreas.TryGetValue(
            ingredient, out FoodStorageArea area))
        {
            if (area != null)
                area.ReleaseSlot(ingredient);

            storedAreas.Remove(ingredient);
        }

        // Lepaskan bahan dari tangan jika sedang dipegang.
        FoodStorageGrabInteractable grab =
            ingredient.GetComponent<FoodStorageGrabInteractable>();

        if (grab == null)
        {
            grab = ingredient.GetComponentInChildren
                <FoodStorageGrabInteractable>();
        }

        if (grab != null && grab.isSelected &&
            grab.interactionManager != null)
        {
            var interactor = grab.firstInteractorSelecting;

            if (interactor != null)
            {
                grab.interactionManager.SelectExit(
                    interactor,
                    grab
                );
            }
        }

        ingredient.isStored = false;
        ingredient.isTaken = false;
        ingredient.arrivalOrder = 0;

        if (!startPositions.TryGetValue(
            ingredient, out Vector3 startPosition))
        {
            Debug.LogWarning(
                "Posisi awal tidak ditemukan untuk " +
                ingredient.ingredientName
            );

            return;
        }

        SetGrabEnabled(ingredient, true);

        Rigidbody rb = ingredient.GetComponent<Rigidbody>();

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.position = startPosition;
            rb.rotation = startRotations[ingredient];
        }
        else
        {
            ingredient.transform.SetPositionAndRotation(
                startPosition,
                startRotations[ingredient]
            );
        }

        ShowAreaFeedback(
            ingredient.ingredientName +
            " dikembalikan ke posisi awal."
        );

        RefreshTakeAvailability();
    }

    // =====================================================
    // RESET
    // =====================================================

    public void ResetSimulation()
    {
         if (kembaliButton != null)
        kembaliButton.SetActive(false);
        pendingIngredient = null;
        storedPoses.Clear();
        foreach (FoodTakeArea zone in FindObjectsByType<FoodTakeArea>(
            FindObjectsSortMode.None))
            if (zone.manager == this) zone.ClearSlots();
        correctCount = 0;
        nextArrivalOrder = 1;
        totalIngredients = 0;

        FoodStorageArea[] areas =
            FindObjectsByType<FoodStorageArea>(
                FindObjectsSortMode.None
            );

        foreach (FoodStorageArea area in areas)
        {
            if (area != null)
                area.ClearSlotOccupancy();
        }

        storedAreas.Clear();

        if (ingredients != null)
        {
            foreach (FoodIngredient ingredient in ingredients)
            {
                if (ingredient == null)
                    continue;

                // Pastikan grab aktif lagi setelah reset.
                SetGrabEnabled(ingredient, true);

                ingredient.ResetIngredient();
                ingredient.isStored = false;
                ingredient.isTaken = false;
                ingredient.arrivalOrder = 0;
SetGrabEnabled(ingredient, true);
                if (ingredient.condition !=
                    FoodIngredient.FoodCondition.Rusak)
                {
                    totalIngredients++;
                }

                if (startPositions.TryGetValue(
                    ingredient, out Vector3 startPosition))
                {
                    ingredient.transform.SetPositionAndRotation(
                        startPosition,
                        startRotations[ingredient]
                    );
                }

                Rigidbody rb = ingredient.GetComponent<Rigidbody>();

                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }
        }

        UpdateProgress();

        ShowFeedback(
            "Simpan semua bahan layak pakai, lalu ambil sesuai " +
            rotationMode + "."
        );
    }

    // =====================================================
    // UI
    // =====================================================

    private void UpdateProgress()
    {
        if (progressText != null)
        {
            progressText.text =
                "Bahan ditempatkan: " +
                correctCount + "/" + totalIngredients;
        }
    }

    private void ShowFeedback(string message)
    {
        if (feedbackText != null)
            feedbackText.text = message;

        Debug.Log(message);
    }

    public void ShowAreaFeedback(string message)
    {
        ShowFeedback(message);
    }

    public bool CanStoreIngredientInArea(
    FoodIngredient ingredient,
    FoodStorageArea.StorageAreaType targetArea)
{
    if (ingredient == null)
        return false;

    if (ingredient.condition == FoodIngredient.FoodCondition.Rusak)
        return false;

    if (ingredients == null ||
        System.Array.IndexOf(ingredients, ingredient) < 0)
        return false;

    return IsCorrectStorage(ingredient, targetArea);
}

public bool CanPlaceInTakeArea(
    FoodIngredient ingredient,
    FoodTakeArea zone)
{
    if (ingredient == null || zone == null)
        return false;

    if (zone.manager != this)
        return false;

    if (pendingIngredient != ingredient)
        return false;

    if (!zone.Accepts(ingredient))
        return false;

    if (!CanTakeIngredient(ingredient, false))
        return false;

    if (!zone.TryGetSlot(out Transform slot))
        return false;

    return true;
}
}
