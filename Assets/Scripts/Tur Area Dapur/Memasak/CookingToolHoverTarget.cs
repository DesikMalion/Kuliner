using ITISKIRUHERE;
using UnityEngine;
using UnityEngine.InputSystem; // WAJIB DITAMBAHKAN UNTUK MEMBACA TOMBOL
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

[RequireComponent(typeof(XRSimpleInteractable))]
public class CookingToolHoverTarget : MonoBehaviour
{
    [Header("Informasi Alat (Interaksi 1)")]
    [TextArea(1, 2)]
    [SerializeField] private string toolName;

    [TextArea(2, 4)]
    [SerializeField] private string toolFunction;

    [Header("Data Kuis (Interaksi 2)")]
    [TextArea(1, 2)]
    public string pertanyaanKuis = "Alat ini digunakan untuk metode memasak apa?";

    public string jawabanBenar;
    public string[] jawabanSalah;

    // --- PERUBAHAN DI SINI: MEMISAHKAN OFFSET ---
    [Header("Posisi Panel & Penanda")]
    [Tooltip("Tinggi/Posisi panel INFO (Interaksi 1) saat di-hover")]
    [SerializeField] private Vector3 hoverPanelOffset = new Vector3(0f, 0.5f, 0f);

    [Tooltip("Tinggi/Posisi panel KUIS (Interaksi 2) saat diklik. Isi nilai Y lebih besar agar tidak terpotong.")]
    [SerializeField] private Vector3 quizPanelOffset = new Vector3(0f, 0.8f, 0f);

    public AdvancedOutline alatOutline;

    [Header("Input Manual (Ganti Grip ke Trigger)")]
    public InputActionReference tombolTrigger;

    private XRSimpleInteractable simpleInteractable;
    private bool isSelesai = false;
    private bool sedangDiHover = false;

    private void Awake()
    {
        simpleInteractable = GetComponent<XRSimpleInteractable>();
    }

    private void OnEnable()
    {
        if (simpleInteractable == null) simpleInteractable = GetComponent<XRSimpleInteractable>();

        simpleInteractable.firstHoverEntered.AddListener(OnFirstHoverEntered);
        simpleInteractable.lastHoverExited.AddListener(OnLastHoverExited);
    }

    private void OnDisable()
    {
        if (simpleInteractable == null) return;

        simpleInteractable.firstHoverEntered.RemoveListener(OnFirstHoverEntered);
        simpleInteractable.lastHoverExited.RemoveListener(OnLastHoverExited);
    }

    private void OnFirstHoverEntered(HoverEnterEventArgs args)
    {
        sedangDiHover = true;

        if (ToolInfoUIManager.Instance == null || isSelesai) return;

        // Gunakan hoverPanelOffset
        ToolInfoUIManager.Instance.ShowInfo(transform, toolName, toolFunction, hoverPanelOffset);
    }

    private void OnLastHoverExited(HoverExitEventArgs args)
    {
        sedangDiHover = false;

        if (ToolInfoUIManager.Instance == null) return;
        ToolInfoUIManager.Instance.HideInfo(transform);
    }

    private void Update()
    {
        if (sedangDiHover && tombolTrigger != null && tombolTrigger.action.WasPressedThisFrame())
        {
            OnAlatDiklik();
        }
    }

    private void OnAlatDiklik()
    {
        if (isSelesai) return;

        if (ToolInfoUIManager.Instance != null && ToolInfoUIManager.Instance.isInteraksi1Aktif) return;

        if (CookingQuizUIManager.Instance != null)
        {
            // Gunakan quizPanelOffset
            CookingQuizUIManager.Instance.TampilkanKuis(this, transform, pertanyaanKuis, jawabanBenar, jawabanSalah, quizPanelOffset);
        }
    }

    public void TandaiSelesai()
    {
        isSelesai = true;

        if (alatOutline != null)
        {
            alatOutline.PulseWidth = true;
            alatOutline.OutlineWidth = 5f;
        }
    }
}