using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class CookingUIManager : MonoBehaviour
{
    public static CookingUIManager Instance { get; private set; }

    public CookingTaskUI sc_cooking_taskUI;


    // =========================================================
    // PANEL
    // =========================================================

    [Header("Panel")]
    public GameObject simulationListPanel;
    public GameObject simulationDescriptionPanel;
    public GameObject completionPanel;


    // =========================================================
    // LIST UI
    // =========================================================

    [Header("Simulation List")]
    public Transform simulationListContainer;
    public GameObject simulationButtonPrefab;


    // =========================================================
    // DESCRIPTION UI
    // =========================================================

    [Header("Description")]
    public TMP_Text titleText;
    public TMP_Text descriptionText;

    public Button startButton;
    public Button backButton;


    // =========================================================
    // PROGRESS CANVAS
    // =========================================================

    [Header("Progress Canvas")]
    public RectTransform progressCanvas;


    // =========================================================
    // SIMULATION DATA
    // =========================================================

    [Header("Simulation Data")]
    public SimulationData[] simulations;


    // =========================================================
    // COMPLETION
    // =========================================================

    [Header("Completion")]
    public Button completionBackButton;


    // =========================================================
    // SELECTED SIMULATION
    // =========================================================

    private SimulationData selectedSimulation;


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        if (Instance != null &&
            Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }


    // =========================================================
    // START
    // =========================================================

    private void Start()
    {
        ShowSimulationList();

        GenerateSimulationList();


        // =====================================================
        // START BUTTON
        // =====================================================

        if (startButton != null)
        {
            startButton.onClick.AddListener(
                StartSelectedSimulation
            );
        }


        // =====================================================
        // BACK BUTTON
        // =====================================================

        if (backButton != null)
        {
            backButton.onClick.AddListener(
                ShowSimulationList
            );
        }


        // =====================================================
        // COMPLETION BACK BUTTON
        // =====================================================

        if (completionBackButton != null)
        {
            completionBackButton.onClick.AddListener(
                ShowSimulationList
            );
        }


        // =====================================================
        // HIDE COMPLETION PANEL
        // =====================================================

        if (completionPanel != null)
        {
            completionPanel.SetActive(false);
        }
    }


    // =========================================================
    // GENERATE SIMULATION LIST
    // =========================================================

    private void GenerateSimulationList()
    {
        if (simulationListContainer == null)
        {
            Debug.LogError(
                "Simulation List Container belum dipasang."
            );

            return;
        }


        if (simulationButtonPrefab == null)
        {
            Debug.LogError(
                "Simulation Button Prefab belum dipasang."
            );

            return;
        }


        // =====================================================
        // HAPUS BUTTON LAMA
        // =====================================================

        foreach (Transform child in simulationListContainer)
        {
            Destroy(child.gameObject);
        }


        // =====================================================
        // BUAT BUTTON
        // =====================================================

        for (int i = 0;
             i < simulations.Length;
             i++)
        {
            SimulationData data =
                simulations[i];


            if (data == null)
                continue;


            GameObject buttonObject =
                Instantiate(
                    simulationButtonPrefab,
                    simulationListContainer
                );


            // =================================================
            // SET TEXT BUTTON
            // =================================================

            TMP_Text text =
                buttonObject.GetComponentInChildren<TMP_Text>();


            if (text != null)
            {
                text.text =
                    data.simulationName;
            }


            // =================================================
            // BUTTON
            // =================================================

            Button button =
                buttonObject.GetComponent<Button>();


            if (button != null)
            {
                SimulationData selectedData =
                    data;


                button.onClick.AddListener(
                    () =>
                    {
                        OpenDescription(
                            selectedData
                        );
                    }
                );
            }
        }
    }


    // =========================================================
    // OPEN DESCRIPTION
    // =========================================================

    private void OpenDescription(
        SimulationData data)
    {
        if (data == null)
            return;


        selectedSimulation =
            data;


        // =====================================================
        // SET TITLE
        // =====================================================

        if (titleText != null)
        {
            titleText.text =
                data.simulationName;
        }


        // =====================================================
        // SET DESCRIPTION
        // =====================================================

        if (descriptionText != null)
        {
            descriptionText.text =
                data.description;
        }


        // =====================================================
        // HIDE LIST
        // =====================================================

        if (simulationListPanel != null)
        {
            simulationListPanel.SetActive(false);
        }


        // =====================================================
        // SHOW DESCRIPTION
        // =====================================================

        if (simulationDescriptionPanel != null)
        {
            simulationDescriptionPanel.SetActive(true);
        }
    }


    // =========================================================
    // SHOW SIMULATION LIST
    // =========================================================

    public void ShowSimulationList()
    {
        selectedSimulation = null;


        // =====================================================
        // SHOW LIST
        // =====================================================

        if (simulationListPanel != null)
        {
            simulationListPanel.SetActive(true);
        }


        // =====================================================
        // HIDE DESCRIPTION
        // =====================================================

        if (simulationDescriptionPanel != null)
        {
            simulationDescriptionPanel.SetActive(false);
        }


        // =====================================================
        // HIDE COMPLETION
        // =====================================================

        if (completionPanel != null)
        {
            completionPanel.SetActive(false);
        }
    }


    // =========================================================
    // START SIMULATION
    // =========================================================

    private void StartSelectedSimulation()
    {
        if (selectedSimulation == null)
        {
            Debug.LogWarning(
                "Belum memilih simulasi."
            );

            return;
        }


        Debug.Log(
            "MEMULAI SIMULASI : " +
            selectedSimulation.simulationName
        );


        // =====================================================
        // HIDE SEMUA OBJECT SIMULASI
        // =====================================================

        HideAllSimulationObjects();


        // =====================================================
        // PINDAHKAN PROGRESS CANVAS
        // =====================================================

        MoveProgressCanvas(
            selectedSimulation.progressTransform
        );


        // =====================================================
        // TUTUP UI PEMBUKA
        // =====================================================

        if (simulationListPanel != null)
        {
            simulationListPanel.SetActive(false);
        }


        if (simulationDescriptionPanel != null)
        {
            simulationDescriptionPanel.SetActive(false);
        }


        // =====================================================
        // MULAI SIMULASI TERPILIH
        // =====================================================

        selectedSimulation.StartSimulation();
    }


    // =========================================================
    // MOVE PROGRESS CANVAS
    // =========================================================

    private void MoveProgressCanvas(
        RectTransform targetTransform)
    {
        if (progressCanvas == null)
        {
            Debug.LogWarning(
                "Progress Canvas belum dipasang."
            );

            return;
        }


        if (targetTransform == null)
        {
            Debug.LogWarning(
                "Progress Transform untuk simulasi " +
                selectedSimulation.simulationName +
                " belum dipasang."
            );

            return;
        }


        // =====================================================
        // POSITION
        // =====================================================

        progressCanvas.position =
            targetTransform.position;


        // =====================================================
        // ROTATION
        // =====================================================

        progressCanvas.rotation =
            targetTransform.rotation;


        // =====================================================
        // SCALE
        // =====================================================

        progressCanvas.localScale =
            targetTransform.localScale;


        Debug.Log(
            "Progress Canvas dipindahkan ke : " +
            targetTransform.name
        );
    }


    // =========================================================
    // SHOW COMPLETION PANEL
    // =========================================================

    public void ShowCompletionPanel()
    {
        Debug.Log(
            "=== SIMULASI SELESAI ==="
        );


        // =====================================================
        // HIDE LIST
        // =====================================================

        if (simulationListPanel != null)
        {
            simulationListPanel.SetActive(false);
        }


        // =====================================================
        // HIDE DESCRIPTION
        // =====================================================

        if (simulationDescriptionPanel != null)
        {
            simulationDescriptionPanel.SetActive(false);
        }


        // =====================================================
        // SHOW COMPLETION
        // =====================================================

        if (completionPanel != null)
        {
            completionPanel.SetActive(true);
        }
    }


    // =========================================================
    // HIDE ALL SIMULATION OBJECTS
    // =========================================================

    public void HideAllSimulationObjects()
    {
        if (simulations == null)
            return;


        foreach (SimulationData simulation in simulations)
        {
            if (simulation == null)
                continue;


            if (simulation.requiredObjects == null)
                continue;


            foreach (GameObject obj in simulation.requiredObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
        }
    }
}



// =============================================================
// SIMULATION DATA
// =============================================================

[System.Serializable]
public class SimulationData
{
    // =========================================================
    // SIMULATION
    // =========================================================

    [Header("Simulation")]
    public string simulationName;


    [TextArea(4, 10)]
    public string description;


    // =========================================================
    // REQUIRED OBJECTS
    // =========================================================

    [Header("Required Objects")]
    public GameObject[] requiredObjects;


    // =========================================================
    // COOKING STEPS
    // =========================================================

    [Header("Cooking Steps")]
    public CookingStep[] cookingSteps;


    // =========================================================
    // PROGRESS UI POSITION
    // =========================================================

    [Header("Progress UI Position")]
    public RectTransform progressTransform;


    // =========================================================
    // START SIMULATION
    // =========================================================

    public void StartSimulation()
    {
        Debug.Log(
            "Simulation Started : " +
            simulationName
        );


        // =====================================================
        // AKTIFKAN OBJECT SIMULASI
        // =====================================================

        if (requiredObjects != null)
        {
            foreach (GameObject obj in requiredObjects)
            {
                if (obj != null)
                {
                    obj.SetActive(true);
                }
            }
        }


        // =====================================================
        // SET COOKING STEPS
        // =====================================================

        if (CookingManager.Instance != null)
        {
            CookingManager.Instance.SetSimulationSteps(
                cookingSteps
            );
        }


        // =====================================================
        // RESET TASK UI
        // =====================================================

        if (CookingUIManager.Instance != null &&
            CookingUIManager.Instance.sc_cooking_taskUI != null)
        {
            CookingUIManager.Instance.sc_cooking_taskUI.enabled =
                true;


            CookingUIManager.Instance.sc_cooking_taskUI.ResetTaskUI();
        }
    }
}