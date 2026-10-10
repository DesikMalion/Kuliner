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


        // =====================================================
        // RESET SEMUA STEP
        // =====================================================

        foreach (CookingStep step in steps)
        {
            if (step != null)
            {
                step.ResetStep();

                // Pastikan semua ghost mati
                step.HideGhostObjects();
            }
        }


        // =====================================================
        // UPDATE ACTIVE OBJECT
        // =====================================================

        UpdateStepObjects();


        // =====================================================
        // TAMPILKAN STEP PERTAMA
        // =====================================================

        ShowCurrentStep();
    }


    // =========================================================
    // SET SIMULATION STEPS
    // =========================================================

    public void SetSimulationSteps(
        CookingStep[] newSteps)
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


        // =====================================================
        // SIMPAN STEP LAMA
        // =====================================================

        CookingStep completedStep =
            CurrentStep;


        // =====================================================
        // COMPLETE
        // =====================================================

        completedStep.Complete();


        // =====================================================
        // MATIKAN GHOST STEP LAMA
        // =====================================================

        completedStep.HideGhostObjects();


        // =====================================================
        // NEXT STEP
        // =====================================================

        currentStepIndex++;


        // =====================================================
        // UPDATE ACTIVE OBJECT
        // =====================================================

        UpdateStepObjects();


        // =====================================================
        // TAMPILKAN STEP BARU
        // =====================================================

        ShowCurrentStep();
    }


    // =========================================================
    // SHOW CURRENT STEP
    // =========================================================

    private void ShowCurrentStep()
    {
        if (CurrentStep == null)
        {
            Debug.Log(
                "=== SEMUA STEP SELESAI ==="
            );


            if (CookingUIManager.Instance != null)
            {
                CookingUIManager.Instance
                    .ShowCompletionPanel();
            }

            return;
        }


        // =====================================================
        // TAMPILKAN GHOST CURRENT STEP
        // =====================================================

        CurrentStep.ShowGhostObjects();


        // =====================================================
        // TEXT STEP
        // =====================================================

        if (text_ != null)
        {
            text_.text =
                CurrentStep.stepName;
        }


        Debug.Log(
            "STEP SEKARANG : " +
            CurrentStep.stepName
        );


        Debug.Log(
            "INSTRUKSI : " +
            CurrentStep.instruction
        );
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


        // =====================================================
        // REQUIREMENT BERJALAN SENDIRI
        // =====================================================

        CurrentStep.CheckRequirement(
            eventType,
            target
        );


        // =====================================================
        // STEP SELESAI
        // =====================================================

        if (CurrentStep.IsCompleted)
        {
            CompleteCurrentStep();
        }
    }


    // =========================================================
    // NEXT ACTIVE OBJECT
    // =========================================================
    //
    // Dipanggil ketika object aktif saat ini sudah selesai.
    //
    // Tidak berhubungan dengan requirement.
    //
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


        // =====================================================
        // MATIKAN SEMUA OBJECT
        // =====================================================

        foreach (CookingStep step in steps)
        {
            if (step == null ||
                step.activeObjects == null)
                continue;


            foreach (CookingStepObject stepObject
                     in step.activeObjects)
            {
                if (stepObject == null ||
                    stepObject.target == null)
                    continue;


                SetStepObjectState(
                    stepObject.target,
                    false,
                    stepObject.outlineColor
                );
            }
        }


        // =====================================================
        // CURRENT STEP
        // =====================================================

        if (CurrentStep == null ||
            CurrentStep.activeObjects == null)
            return;


        // =====================================================
        // TIDAK BERURUTAN
        // =====================================================

        if (!CurrentStep.sequentialActiveObjects)
        {
            foreach (CookingStepObject stepObject
                     in CurrentStep.activeObjects)
            {
                if (stepObject == null ||
                    stepObject.target == null)
                    continue;


                SetStepObjectState(
                    stepObject.target,
                    true,
                    stepObject.outlineColor
                );
            }

            return;
        }


        // =====================================================
        // BERURUTAN
        // =====================================================

        int index =
            CurrentStep.activeObjectIndex;


        if (index < 0 ||
            index >= CurrentStep.activeObjects.Length)
            return;


        CookingStepObject activeObject =
            CurrentStep.activeObjects[index];


        if (activeObject == null ||
            activeObject.target == null)
            return;


        SetStepObjectState(
            activeObject.target,
            true,
            activeObject.outlineColor
        );
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


        // =====================================================
        // COLLIDER
        // =====================================================

        Collider[] colliders =
            target.GetComponentsInChildren<Collider>(true);


        foreach (Collider col in colliders)
        {
            if (col == null)
                continue;

            col.enabled = state;
        }


        // =====================================================
        // OUTLINE
        // =====================================================

        Outline[] outlines =
            target.GetComponentsInChildren<Outline>(true);


        foreach (Outline outline in outlines)
        {
            if (outline == null)
                continue;


            if (state)
            {
                outline.OutlineColor =
                    outlineColor;

                outline.enabled = true;
            }
            else
            {
                outline.enabled = false;
            }
        }
    }
}