using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CookingTaskUI : MonoBehaviour
{
    [Header("UI")]
    [Tooltip("Parent tempat semua checklist dibuat.")]
    public Transform taskContainer;

    [Tooltip("Prefab satu item checklist.")]
    public GameObject taskPrefab;

    [Header("Checklist Text")]
    public string uncheckedSymbol = "☐";
    public string checkedSymbol = "☑";

    [Header("Completed Color")]
    public Color completedColor = Color.gray;

    private List<TMP_Text> taskTexts =
        new List<TMP_Text>();

    private CookingManager cookingManager;


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        cookingManager =
            CookingManager.Instance;

        if (cookingManager == null)
        {
            Debug.LogError(
                "CookingTaskUI : CookingManager tidak ditemukan!"
            );

            return;
        }

        if (taskContainer != null)
        {
            taskContainer.gameObject.SetActive(true);
        }

        GenerateTasks();
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        if (cookingManager == null)
        {
            cookingManager =
                CookingManager.Instance;

            if (cookingManager == null)
                return;
        }

        UpdateChecklist();
    }


    // =========================================================
    // GENERATE TASK
    // =========================================================

    private void GenerateTasks()
    {
        // Pastikan CookingManager terbaru
        cookingManager =
            CookingManager.Instance;

        if (cookingManager == null)
        {
            Debug.LogError(
                "CookingTaskUI : CookingManager tidak ditemukan!"
            );

            return;
        }


        // =====================================================
        // CEK CONTAINER
        // =====================================================

        if (taskContainer == null)
        {
            Debug.LogError(
                "CookingTaskUI : Task Container belum dipasang."
            );

            return;
        }


        // =====================================================
        // CEK PREFAB
        // =====================================================

        if (taskPrefab == null)
        {
            Debug.LogError(
                "CookingTaskUI : Task Prefab belum dipasang."
            );

            return;
        }


        // =====================================================
        // HAPUS TASK LAMA
        // =====================================================

        foreach (Transform child in taskContainer)
        {
            Destroy(child.gameObject);
        }

        taskTexts.Clear();


        // =====================================================
        // AMBIL STEPS
        // =====================================================

        CookingStep[] steps =
            cookingManager.steps;

        if (steps == null ||
            steps.Length == 0)
        {
            Debug.LogWarning(
                "CookingTaskUI : CookingManager tidak memiliki step."
            );

            return;
        }


        // =====================================================
        // BUAT TASK BARU
        // =====================================================

        for (int i = 0; i < steps.Length; i++)
        {
            CookingStep step =
                steps[i];

            if (step == null)
                continue;


            GameObject taskObject =
                Instantiate(
                    taskPrefab,
                    taskContainer
                );


            TMP_Text text =
                taskObject.GetComponentInChildren<TMP_Text>();


            if (text == null)
            {
                Debug.LogError(
                    "CookingTaskUI : Task Prefab tidak memiliki TMP_Text."
                );

                Destroy(taskObject);

                continue;
            }


            taskTexts.Add(text);


            // =================================================
            // TEXT AWAL
            // =================================================

            text.text =
                
                " " +
                (i + 1) +
                ". " +
                step.stepName;

            text.color = Color.white;
        }
    }


    // =========================================================
    // UPDATE CHECKLIST
    // =========================================================

    private void UpdateChecklist()
    {
        if (cookingManager == null)
            return;


        CookingStep[] steps =
            cookingManager.steps;

        if (steps == null)
            return;


        int textIndex = 0;


        for (int i = 0; i < steps.Length; i++)
        {
            CookingStep step =
                steps[i];

            if (step == null)
                continue;


            if (textIndex >= taskTexts.Count)
                break;


            TMP_Text text =
                taskTexts[textIndex];


            if (text == null)
                continue;


            // =================================================
            // SELESAI
            // =================================================

            if (step.IsCompleted)
            {
                text.text =
                    
                    " <s>" +
                    (i + 1) +
                    ". " +
                    step.stepName +
                    "</s>";

                text.color =
                    completedColor;
            }


            // =================================================
            // BELUM SELESAI
            // =================================================

            else
            {
                text.text =
                    
                    " " +
                    (i + 1) +
                    ". " +
                    step.stepName;

                text.color =
                    Color.white;
            }


            textIndex++;
        }
    }


    // =========================================================
    // RESET TASK UI
    // =========================================================

    public void ResetTaskUI()
    {
        Debug.Log(
            "CookingTaskUI : Reset Task UI"
        );


        // Pastikan manager terbaru
        cookingManager =
            CookingManager.Instance;

        if (cookingManager == null)
        {
            Debug.LogError(
                "CookingTaskUI : CookingManager tidak ditemukan!"
            );

            return;
        }


        // Buat ulang checklist
        GenerateTasks();


        // Update status checklist
        UpdateChecklist();
    }
}