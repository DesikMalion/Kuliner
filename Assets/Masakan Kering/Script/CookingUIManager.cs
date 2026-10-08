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


    // =========================================================
    // LIST UI
    // =========================================================

    [Header("Simulation List")]
    public Transform simulationListContainer;
    public GameObject simulationButtonPrefab;
public GameObject completionPanel;

    // =========================================================
    // DESCRIPTION UI
    // =========================================================

    [Header("Description")]
    public TMP_Text titleText;
    public TMP_Text descriptionText;

    public Button startButton;
    public Button backButton;


    // =========================================================
    // SIMULATION DATA
    // =========================================================

    [Header("Simulation Data")]
    public SimulationData[] simulations;


    private SimulationData selectedSimulation;

[Header("Completion")]
public Button completionBackButton;
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


    if (startButton != null)
    {
        startButton.onClick.AddListener(
            StartSelectedSimulation
        );
    }


    if (backButton != null)
    {
        backButton.onClick.AddListener(
            ShowSimulationList
        );
    }


    if (completionBackButton != null)
    {
        completionBackButton.onClick.AddListener(
            ShowSimulationList
        );
    }


    if (completionPanel != null)
    {
        completionPanel.SetActive(false);
    }
}


    // =========================================================
    // GENERATE LIST
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


        // Hapus button lama
        foreach (
            Transform child
            in simulationListContainer
        )
        {
            Destroy(child.gameObject);
        }


        // Buat button
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


            TMP_Text text =
                buttonObject.GetComponentInChildren<TMP_Text>();


            if (text != null)
            {
                text.text =
                    data.simulationName;
            }


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


        if (titleText != null)
        {
            titleText.text =
                data.simulationName;
        }


        if (descriptionText != null)
        {
            descriptionText.text =
                data.description;
        }


        if (simulationListPanel != null)
        {
            simulationListPanel.SetActive(false);
        }


        if (simulationDescriptionPanel != null)
        {
            simulationDescriptionPanel.SetActive(true);
        }
    }


    // =========================================================
    // SHOW LIST
    // =========================================================

  public void ShowSimulationList()
{
    selectedSimulation = null;


    if (simulationListPanel != null)
    {
        simulationListPanel.SetActive(true);
    }


    if (simulationDescriptionPanel != null)
    {
        simulationDescriptionPanel.SetActive(false);
    }


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

    // =================================================
    // HIDE SEMUA OBJECT SIMULASI
    // =================================================

    HideAllSimulationObjects();


    // =================================================
    // TUTUP UI PEMBUKA
    // =================================================

    if (simulationListPanel != null)
    {
        simulationListPanel.SetActive(false);
    }

    if (simulationDescriptionPanel != null)
    {
        simulationDescriptionPanel.SetActive(false);
    }


    // =================================================
    // MULAI SIMULASI TERPILIH
    // =================================================

    selectedSimulation.StartSimulation();
}

    public void ShowCompletionPanel()
{
    Debug.Log("=== SIMULASI SELESAI ===");


    // Tutup panel lain
    if (simulationListPanel != null)
    {
        simulationListPanel.SetActive(false);
    }

    if (simulationDescriptionPanel != null)
    {
        simulationDescriptionPanel.SetActive(false);
    }


    // Tampilkan panel selesai
    if (completionPanel != null)
    {
        completionPanel.SetActive(true);
    }
}

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
    [Header("Simulation")]
    public string simulationName;

    [TextArea(4, 10)]
    public string description;


    // =====================================================
    // REQUIRED OBJECTS
    // =====================================================

    [Header("Required Objects")]
    public GameObject[] requiredObjects;


    // =====================================================
    // COOKING STEPS
    // =====================================================

    [Header("Cooking Steps")]
    public CookingStep[] cookingSteps;


    // =====================================================
    // START SIMULATION
    // =====================================================

    public void StartSimulation()
    {
        Debug.Log(
            "Simulation Started : " +
            simulationName
        );


        // =================================================
        // AKTIFKAN OBJECT SIMULASI
        // =================================================

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


        // =================================================
        // SET COOKING STEPS
        // =================================================

        if (CookingManager.Instance != null)
        {
            CookingManager.Instance.SetSimulationSteps(
                cookingSteps
            );
        }


        // =================================================
        // RESET TASK UI
        // =================================================

        if (CookingUIManager.Instance != null &&
            CookingUIManager.Instance.sc_cooking_taskUI != null)
        {
            CookingUIManager.Instance.sc_cooking_taskUI.enabled = true;

            CookingUIManager.Instance.sc_cooking_taskUI.ResetTaskUI();
        }
    }
}