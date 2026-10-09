using UnityEngine;
using System.Collections.Generic;


[System.Serializable]
public class CookingStepObject
{
    [Header("Object")]
    public GameObject target;

    [Header("Outline Color")]
    public Color outlineColor = Color.white;
}


// =========================================================
// GHOST OBJECT
// =========================================================

[System.Serializable]
public class CookingGhostObject
{
    [Header("Ghost Object")]
    public GameObject ghost;

    [Header("Show Ghost")]
    public bool showGhost = true;
}


public class CookingStep : MonoBehaviour
{
    [Header("Step Information")]
    public string stepName;

    [TextArea(2, 5)]
    public string instruction;


    // =========================================================
    // REQUIREMENTS
    // =========================================================

    [Header("Requirements")]
    public List<CookingRequirement> requirements;


    // =========================================================
    // ACTIVE OBJECTS
    // =========================================================

    [Header("Active Objects")]
    public CookingStepObject[] activeObjects;

    [Header("Active Object Order")]
    public bool sequentialActiveObjects;

    [HideInInspector]
    public int activeObjectIndex = 0;


    // =========================================================
    // GHOST OBJECTS
    // =========================================================

    [Header("Ghost Objects")]
    public CookingGhostObject[] ghostObjects;


    // =========================================================
    // STATUS
    // =========================================================

    public bool IsCompleted { get; private set; }


    // =========================================================
    // COMPLETE
    // =========================================================

    public void Complete()
    {
        if (IsCompleted)
            return;

        IsCompleted = true;

        // Matikan ghost ketika step selesai
        HideGhostObjects();

        Debug.Log(
            "STEP SELESAI : " +
            stepName
        );
    }


    // =========================================================
    // CHECK REQUIREMENT
    // =========================================================

    public bool CheckRequirement(
        CookingEventType eventType,
        GameObject target)
    {
        foreach (CookingRequirement requirement in requirements)
        {
            if (requirement.requiredEvent != eventType)
                continue;

            if (requirement.requiredTarget != null &&
                requirement.requiredTarget != target)
                continue;

            if (requirement.isCompleted)
                return false;

            requirement.isCompleted = true;

            Debug.Log(
                "Requirement selesai : " +
                target.name
            );

            CheckAllRequirements();

            return true;
        }

        return false;
    }


    // =========================================================
    // CHECK ALL REQUIREMENTS
    // =========================================================

    private void CheckAllRequirements()
    {
        foreach (CookingRequirement requirement in requirements)
        {
            if (!requirement.isCompleted)
                return;
        }

        Complete();
    }


    // =========================================================
    // ACTIVE OBJECT
    // =========================================================

    public void NextActiveObject()
    {
        if (!sequentialActiveObjects)
            return;

        if (activeObjects == null ||
            activeObjects.Length == 0)
            return;

        if (activeObjectIndex >=
            activeObjects.Length - 1)
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


    public void ResetActiveObjects()
    {
        activeObjectIndex = 0;
    }


    // =========================================================
    // SHOW GHOST
    // =========================================================

    public void ShowGhostObjects()
    {
        if (ghostObjects == null)
            return;

        foreach (CookingGhostObject ghostData in ghostObjects)
        {
            if (ghostData == null)
                continue;

            if (ghostData.ghost == null)
                continue;

            if (!ghostData.showGhost)
                continue;

            ghostData.ghost.SetActive(true);
        }
    }


    // =========================================================
    // HIDE GHOST
    // =========================================================

    public void HideGhostObjects()
    {
        if (ghostObjects == null)
            return;

        foreach (CookingGhostObject ghostData in ghostObjects)
        {
            if (ghostData == null)
                continue;

            if (ghostData.ghost == null)
                continue;

            ghostData.ghost.SetActive(false);
        }
    }


    // =========================================================
    // RESET STEP
    // =========================================================

    public void ResetStep()
    {
        IsCompleted = false;

        foreach (CookingRequirement requirement in requirements)
        {
            requirement.isCompleted = false;
        }

        ResetActiveObjects();

        // Ghost kembali disembunyikan.
        HideGhostObjects();
    }
}


[System.Serializable]
public class CookingRequirement
{
    public CookingEventType requiredEvent;
    public GameObject requiredTarget;

    [HideInInspector]
    public bool isCompleted;
}