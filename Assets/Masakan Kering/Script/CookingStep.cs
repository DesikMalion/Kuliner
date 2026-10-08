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

public class CookingStep : MonoBehaviour
{
    [Header("Step Information")]
    public string stepName;

    [TextArea(2, 5)]
    public string instruction;

    [Header("Requirements")]
    public List<CookingRequirement> requirements;

    [Header("Active Objects")]
    public CookingStepObject[] activeObjects;

    [Header("Active Object Order")]
    public bool sequentialActiveObjects;

    [HideInInspector]
    public int activeObjectIndex = 0;

    public bool IsCompleted { get; private set; }


    public void Complete()
    {
        if (IsCompleted)
            return;

        IsCompleted = true;

        Debug.Log(
            "STEP SELESAI : " +
            stepName
        );
    }


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