
using UnityEngine;
using TMPro;
using ITISKIRUHERE;
using MikeNspired.XRIStarterKit;
using MikeNspired.XRIStarterKit.ChrisNolet;

public class CookingManager : MonoBehaviour
{
    public static CookingManager Instance { get; private set; }

    [Header("Cooking Steps")]
    public CookingStep[] steps;

    public TextMeshProUGUI text_;

    [SerializeField]
    private int currentStepIndex = 0;

    public int CurrentStepIndex => currentStepIndex;

    // =========================================================
    // CURRENT STEP
    // =========================================================

    public CookingStep CurrentStep
    {
        get
        {
            if (steps == null ||
                currentStepIndex < 0 ||
                currentStepIndex >= steps.Length)
            {
                return null;
            }

            return steps[currentStepIndex];
        }
    }

    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        Instance = this;
    }

    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        StartCooking();
    }

    // =========================================================
    // START COOKING
    // =========================================================

    public void StartCooking()
    {
        currentStepIndex = 0;

        if (steps == null)
            return;

        foreach (CookingStep step in steps)
        {
            if (step == null)
                continue;

            step.ResetStep();
            step.HideGhostObjects();
        }

        UpdateStepObjects();
        ShowCurrentStep();
    }

    // =========================================================
    // SET SIMULATION STEPS
    // =========================================================

    public void SetSimulationSteps(CookingStep[] newSteps)
    {
        steps = newSteps;
        StartCooking();
    }

    // =========================================================
    // COMPLETE CURRENT STEP
    // =========================================================

    public void CompleteCurrentStep()
    {
        if (CurrentStep == null)
            return;

        CookingStep completedStep = CurrentStep;

        completedStep.Complete();
        completedStep.HideGhostObjects();

        currentStepIndex++;

        UpdateStepObjects();
        ShowCurrentStep();
    }

    // =========================================================
    // SHOW CURRENT STEP
    // =========================================================

    
private void ShowCurrentStep()
{
    if (CurrentStep == null)
    {
        Debug.Log("=== SEMUA STEP SELESAI ===");

        if (CookingUIManager.Instance != null)
        {
            CookingUIManager.Instance.ShowCompletionPanel();
        }

        return;
    }

    CookingStep stepToShow = CurrentStep;

    stepToShow.ShowGhostObjects();

    if (text_ != null)
        text_.text = stepToShow.stepName;

    Debug.Log("STEP SEKARANG : " + stepToShow.stepName);
    Debug.Log("INSTRUKSI : " + stepToShow.instruction);

    // Periksa ulang status cooked saat step baru aktif.
    stepToShow.RecheckRequirements();

    // Pastikan step yang sama masih aktif sebelum lanjut.
    if (CurrentStep == stepToShow && stepToShow.IsCompleted)
    {
        CompleteCurrentStep();
    }
}


    // =========================================================
    // CHECK EVENT
    // =========================================================

    public void CheckEvent(
        CookingEventType eventType,
        GameObject target)
    {
        if (CurrentStep == null)
            return;

        CurrentStep.CheckRequirement(eventType, target);

        if (CurrentStep != null && CurrentStep.IsCompleted)
        {
            CompleteCurrentStep();
        }
    }

    // =========================================================
    // NEXT ACTIVE OBJECT
    // =========================================================

    public void NextActiveObject()
    {
        if (CurrentStep == null)
            return;

        CurrentStep.NextActiveObject();
    }

    // =========================================================
    // UPDATE ACTIVE OBJECTS
    // =========================================================

    public void UpdateActiveObjects()
    {
        UpdateStepObjects();
    }

    // =========================================================
    // UPDATE STEP OBJECTS
    // =========================================================

    private void UpdateStepObjects()
    {
        if (steps == null)
            return;

        // Matikan seluruh active objects pada semua step.
        foreach (CookingStep step in steps)
        {
            if (step == null)
                continue;

            if (step.activeObjects != null)
            {
                foreach (CookingStepObject obj in step.activeObjects)
                {
                    if (obj == null || obj.target == null)
                        continue;

                    SetStepObjectState(
                        obj.target,
                        false,
                        obj.outlineColor
                    );
                }
            }

            if (step.secondaryActiveObjects != null)
            {
                foreach (CookingStepObject obj in step.secondaryActiveObjects)
                {
                    if (obj == null || obj.target == null)
                        continue;

                    SetStepObjectState(
                        obj.target,
                        false,
                        obj.outlineColor
                    );
                }
            }
        }

        if (CurrentStep == null)
            return;

        // Active objects utama.
        if (CurrentStep.activeObjects != null)
        {
            if (!CurrentStep.sequentialActiveObjects)
            {
                foreach (CookingStepObject obj in CurrentStep.activeObjects)
                {
                    if (obj == null || obj.target == null)
                        continue;

                    SetStepObjectState(
                        obj.target,
                        true,
                        obj.outlineColor
                    );
                }
            }
            else
            {
                int index = CurrentStep.activeObjectIndex;

                if (index >= 0 &&
                    index < CurrentStep.activeObjects.Length)
                {
                    CookingStepObject obj =
                        CurrentStep.activeObjects[index];

                    if (obj != null && obj.target != null)
                    {
                        SetStepObjectState(
                            obj.target,
                            true,
                            obj.outlineColor
                        );
                    }
                }
            }
        }

        // Secondary active objects tetap aktif bersamaan.
        if (CurrentStep.secondaryActiveObjects != null)
        {
            foreach (CookingStepObject obj in CurrentStep.secondaryActiveObjects)
            {
                if (obj == null || obj.target == null)
                    continue;

                SetStepObjectState(
                    obj.target,
                    true,
                    obj.outlineColor
                );
            }
        }
    }

    // =========================================================
    // SET OBJECT STATE
    // =========================================================

    private void SetStepObjectState(
        GameObject target,
        bool state,
        Color outlineColor)
    {
        if (target == null)
            return;

        // Collider.
        Collider[] colliders =
            target.GetComponentsInChildren<Collider>(true);

        foreach (Collider col in colliders)
        {
            if (col == null)
                continue;

            col.enabled = state;
        }

        // Outline.
        Outline[] outlines =
            target.GetComponentsInChildren<Outline>(true);

        foreach (Outline outline in outlines)
        {
            if (outline == null)
                continue;

            if (state)
            {
                outline.OutlineColor = outlineColor;
                outline.enabled = true;
            }
            else
            {
                outline.enabled = false;
            }
        }
    }
}
