
using UnityEngine;
using System.Collections.Generic;

// =========================================================
// ACTIVE OBJECT DATA
// =========================================================

[System.Serializable]
public class CookingStepObject
{
    [Header("Object")]
    public GameObject target;

    [Header("Outline Color")]
    public Color outlineColor = Color.white;
}

// =========================================================
// GHOST OBJECT DATA
// =========================================================

[System.Serializable]
public class CookingGhostObject
{
    [Header("Ghost Object")]
    public GameObject ghost;

    [Header("Show Ghost")]
    public bool showGhost = true;
}

// =========================================================
// COOKING STEP
// =========================================================

public class CookingStep : MonoBehaviour
{
    [Header("Step Information")]
    public string stepName;

    [TextArea(2, 5)]
    public string instruction;

    // =====================================================
    // REQUIREMENTS
    // =====================================================

    [Header("Requirements")]
    public List<CookingRequirement> requirements;

    // =====================================================
    // ACTIVE OBJECTS UTAMA
    // =====================================================

    [Header("Active Objects")]
    public CookingStepObject[] activeObjects;

    [Header("Active Object Order")]
    public bool sequentialActiveObjects = false;

    [HideInInspector]
    public int activeObjectIndex = 0;

    // =====================================================
    // SECONDARY ACTIVE OBJECTS
    // =====================================================

    [Header("Secondary Active Objects")]
    [Tooltip("Objek tambahan yang tidak mengikuti urutan sequential.")]
    public CookingStepObject[] secondaryActiveObjects;

    // =====================================================
    // GHOST OBJECTS
    // =====================================================

    [Header("Ghost Objects")]
    public CookingGhostObject[] ghostObjects;

    // =====================================================
    // STATUS
    // =====================================================

    public bool IsCompleted { get; private set; }

    // =====================================================
    // COMPLETE
    // =====================================================

    public void Complete()
    {
        if (IsCompleted)
            return;

        IsCompleted = true;

        HideGhostObjects();

        Debug.Log("STEP SELESAI : " + stepName);
    }

    // =====================================================
    // CHECK REQUIREMENT DARI EVENT
    // =====================================================

   
public bool CheckRequirement(
    CookingEventType eventType,
    GameObject target)
{
    if (IsCompleted || requirements == null)
        return false;

    foreach (CookingRequirement requirement in requirements)
    {
        if (requirement == null || requirement.isCompleted)
            continue;

        if (requirement.requiredEvent != eventType)
            continue;

        if (requirement.requiredTarget != null &&
            requirement.requiredTarget != target)
            continue;

        // Requirement cooked harus memeriksa kondisi ikan terlebih dahulu.
        if (requirement.requireCooked)
        {
            Ingredient ingredient = FindIngredient(
                requirement.requiredTarget != null
                    ? requirement.requiredTarget
                    : target
            );

            if (ingredient == null || !ingredient.isCooked)
                continue;
        }

        requirement.isCompleted = true;

        Debug.Log(
            "Requirement selesai: " +
            (target != null ? target.name : "Target tidak ditentukan") +
            " | Step: " + stepName
        );

        CheckAllRequirements();
        return true;
    }

    return false;
}

    // =====================================================
    // RECHECK REQUIREMENTS
    // Dipanggil kembali saat step mulai aktif
    // =====================================================

   

private Ingredient FindIngredient(GameObject target)
{
    if (target == null)
        return null;

    Ingredient ingredient = target.GetComponent<Ingredient>();

    if (ingredient == null)
        ingredient = target.GetComponentInChildren<Ingredient>(true);

    if (ingredient == null)
        ingredient = target.GetComponentInParent<Ingredient>();

    return ingredient;
}

public void RecheckRequirements()
{
    if (IsCompleted || requirements == null)
        return;

    foreach (CookingRequirement requirement in requirements)
    {
        if (requirement == null || requirement.isCompleted)
            continue;

        if (!requirement.requireCooked)
            continue;

        if (requirement.requiredTarget == null)
            continue;

        Ingredient ingredient = FindIngredient(
            requirement.requiredTarget
        );

        if (ingredient != null && ingredient.isCooked)
        {
            requirement.isCompleted = true;

            Debug.Log(
                "Requirement cooked terdeteksi: " +
                requirement.requiredTarget.name +
                " | Step: " + stepName
            );
        }
    }

    CheckAllRequirements();
}



    // =====================================================
    // CHECK ALL REQUIREMENTS
    // =====================================================

    private void CheckAllRequirements()
    {
        if (requirements == null)
            return;

        foreach (CookingRequirement requirement in requirements)
        {
            if (requirement == null)
                continue;

            if (!requirement.isCompleted)
                return;
        }

        Complete();
    }

    // =====================================================
    // NEXT ACTIVE OBJECT
    // =====================================================

    public void NextActiveObject()
    {
        if (!sequentialActiveObjects)
            return;

        if (activeObjects == null ||
            activeObjects.Length == 0)
            return;

        if (activeObjectIndex >= activeObjects.Length - 1)
            return;

        activeObjectIndex++;

        Debug.Log(
            "Active Object berikutnya : " +
            activeObjectIndex
        );

        if (CookingManager.Instance != null)
        {
            CookingManager.Instance.UpdateActiveObjects();
        }
    }

    // =====================================================
    // RESET ACTIVE OBJECTS
    // =====================================================

    public void ResetActiveObjects()
    {
        activeObjectIndex = 0;
    }

    // =====================================================
    // SHOW GHOST
    // =====================================================

    public void ShowGhostObjects()
    {
        if (ghostObjects == null)
            return;

        foreach (CookingGhostObject ghostData in ghostObjects)
        {
            if (ghostData == null ||
                ghostData.ghost == null ||
                !ghostData.showGhost)
                continue;

            ghostData.ghost.SetActive(true);
        }
    }

    // =====================================================
    // HIDE GHOST
    // =====================================================

    public void HideGhostObjects()
    {
        if (ghostObjects == null)
            return;

        foreach (CookingGhostObject ghostData in ghostObjects)
        {
            if (ghostData == null || ghostData.ghost == null)
                continue;

            ghostData.ghost.SetActive(false);
        }
    }

    // =====================================================
    // RESET STEP
    // =====================================================

    public void ResetStep()
    {
        IsCompleted = false;

        if (requirements != null)
        {
            foreach (CookingRequirement requirement in requirements)
            {
                if (requirement == null)
                    continue;

                requirement.isCompleted = false;
            }
        }

        ResetActiveObjects();

        HideGhostObjects();
    }
}

// =========================================================
// COOKING REQUIREMENT
// =========================================================


[System.Serializable]
public class CookingRequirement
{
    public CookingEventType requiredEvent;
    public GameObject requiredTarget;

    [Header("Recheck Condition")]
    [Tooltip("Centang jika requirement ini harus mengecek status cooked.")]
    public bool requireCooked;

    [HideInInspector]
    public bool isCompleted;
}

