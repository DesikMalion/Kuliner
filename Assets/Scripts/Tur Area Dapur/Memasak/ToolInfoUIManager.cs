using TMPro;
using UnityEngine;

public class ToolInfoUIManager : MonoBehaviour
{
    public static ToolInfoUIManager Instance { get; private set; }

    [Header("Panel UI")]
    [SerializeField] private GameObject infoPanel;
    [SerializeField] private TextMeshProUGUI toolNameText;
    [SerializeField] private TextMeshProUGUI toolFunctionText;

    [Header("Billboard")]
    [SerializeField] private bool facePlayer = true;
    [SerializeField] private Transform playerCamera;

    [HideInInspector] public bool isInteraksi1Aktif = true;

    private Transform currentTarget;
    private Vector3 currentOffset;
    private int activeHoverCount;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }

        if (playerCamera == null && Camera.main != null)
        {
            playerCamera = Camera.main.transform;
        }
    }

    private void LateUpdate()
    {
        if (infoPanel == null || !infoPanel.activeSelf || currentTarget == null)
            return;

        infoPanel.transform.position = currentTarget.TransformPoint(currentOffset);

        if (facePlayer && playerCamera != null)
        {
            Vector3 direction = infoPanel.transform.position - playerCamera.position;

            if (direction.sqrMagnitude > 0.001f)
            {
                infoPanel.transform.rotation = Quaternion.LookRotation(direction);
            }
        }
    }

    public void ShowInfo(Transform target, string toolName, string toolFunction, Vector3 panelOffset)
    {
        // JIKA SAKLAR MATI (Tombol Next sudah diklik), JANGAN MUNCULKAN PANEL
        if (!isInteraksi1Aktif) return;

        currentTarget = target;
        currentOffset = panelOffset;
        activeHoverCount++;

        toolNameText.text = toolName;
        toolFunctionText.text = toolFunction;

        infoPanel.transform.position = target.TransformPoint(panelOffset);
        infoPanel.SetActive(true);
    }

    public void HideInfo(Transform target)
    {
        activeHoverCount = Mathf.Max(0, activeHoverCount - 1);

        if (currentTarget != target)
            return;

        if (activeHoverCount > 0)
            return;

        currentTarget = null;
        infoPanel.SetActive(false);
    }

    public void ForceHide()
    {
        activeHoverCount = 0;
        currentTarget = null;

        if (infoPanel != null)
        {
            infoPanel.SetActive(false);
        }
    }
}