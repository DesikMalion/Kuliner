using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

[RequireComponent(typeof(UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable))]
public class BahanHoverEvent : MonoBehaviour
{
    private UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable interactable;
    private BahanData bahanData;
    private HoverUIManager uiManager;

    private void Awake()
    {
        // Mengambil referensi komponen secara otomatis
        interactable = GetComponent<UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable>();
        bahanData = GetComponent<BahanData>();

        // Mencari manager UI di dalam scene
        uiManager = FindObjectOfType<HoverUIManager>();
    }

    private void OnEnable()
    {
        // Mendaftarkan event hover
        interactable.hoverEntered.AddListener(OnHoverMasuk);
        interactable.hoverExited.AddListener(OnHoverKeluar);
    }

    private void OnDisable()
    {
        // Melepas pendaftaran event agar tidak error saat objek dihancurkan/non-aktif
        interactable.hoverEntered.RemoveListener(OnHoverMasuk);
        interactable.hoverExited.RemoveListener(OnHoverKeluar);
    }

    private void OnHoverMasuk(HoverEnterEventArgs args)
    {
        if (uiManager != null && bahanData != null)
        {
            uiManager.TampilkanInfo(bahanData, transform);
        }
    }

    private void OnHoverKeluar(HoverExitEventArgs args)
    {
        if (uiManager != null)
        {
            uiManager.SembunyikanInfo();
        }
    }
}