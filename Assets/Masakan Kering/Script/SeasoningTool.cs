using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class SeasoningTool : MonoBehaviour
{
    [Header("Brush State")]
    public bool hasSeasoning { get; private set; }

    [Header("XR Grab")]
    public XRGrabInteractable grabInteractable;
[Header("Return Position")]
public Transform returnPoint;
    [Header("Animation")]
    public CookingAnimationController animationController;

    private Seasonable currentSeasonable;


    // =========================================================
    // START
    // =========================================================

    private void Awake()
    {
        if (grabInteractable == null)
        {
            grabInteractable =
                GetComponentInParent<XRGrabInteractable>();
        }
    }


    // =========================================================
    // TRIGGER
    // =========================================================

    private void OnTriggerEnter(Collider other)
    {
        // =====================================================
        // MANGKOK BUMBU
        // =====================================================

        SeasoningBowl bowl =
            other.GetComponentInParent<SeasoningBowl>();

        if (bowl != null)
        {
            TakeSeasoning(bowl);
            return;
        }


        // =====================================================
        // INGREDIENT
        // =====================================================

        Seasonable seasonable =
            other.GetComponentInParent<Seasonable>();

        if (seasonable != null)
        {
            ApplySeasoning(seasonable);
        }
    }


    // =========================================================
    // AMBIL BUMBU
    // =========================================================

    private void TakeSeasoning(
        SeasoningBowl bowl)
    {
        if (!bowl.hasSeasoning)
        {
            Debug.Log(
                "Bumbu sudah habis."
            );

            return;
        }

        hasSeasoning = true;

        bowl.TakeSeasoning();

        Debug.Log(
            "Kuas mengambil bumbu."
        );
    }


    // =========================================================
    // MULAI OLES
    // =========================================================

    private void ApplySeasoning(
        Seasonable seasonable)
    {
        if (!hasSeasoning)
        {
            Debug.Log(
                "Kuas belum memiliki bumbu."
            );

            return;
        }

        if (seasonable.isSeasoned)
            return;

        if (animationController == null)
        {
            Debug.LogWarning(
                "CookingAnimationController belum dipasang."
            );

            return;
        }

        if (animationController.IsPlaying)
            return;


        currentSeasonable =
            seasonable;


        // =====================================================
        // LEPAS DARI TANGAN
        // =====================================================

        ReleaseFromInteractor();


        // =====================================================
        // SNAP KE POSISI ANIMASI
        // =====================================================

        SeasonPath path =
            seasonable.GetComponentInChildren<SeasonPath>();

        if (path != null &&
            path.startPoint != null)
        {
            transform.parent.position =
                path.startPoint.position;

            transform.parent.rotation =
                path.startPoint.rotation;
        }


        // =====================================================
        // MULAI ANIMASI
        // =====================================================

        animationController.PlayAnimation();

        Debug.Log(
            "Mulai animasi mengoles : " +
            seasonable.gameObject.name
        );
    }


    // =========================================================
    // RELEASE XR GRAB
    // =========================================================

    private void ReleaseFromInteractor()
    {
        if (grabInteractable == null)
            return;

        if (!grabInteractable.isSelected)
            return;


        IXRSelectInteractor interactor =
            grabInteractable.firstInteractorSelecting;

        if (interactor == null)
            return;


        // =====================================================
        // PAKSA XR GRAB RELEASE
        // =====================================================

        grabInteractable.interactionManager.SelectExit(
            interactor,
            grabInteractable
        );

        Debug.Log(
            "Kuas dilepas dari tangan."
        );
    }


    // =========================================================
    // DIPANGGIL SETELAH ANIMASI SELESAI
    // =========================================================

   public void FinishSeasoning()
{
    if (currentSeasonable == null)
        return;

    if (!hasSeasoning)
        return;

    currentSeasonable.Season();

    hasSeasoning = false;

    // Kembalikan kuas
    if (returnPoint != null)
    {
        transform.parent.position = returnPoint.position;
        transform.parent.rotation = returnPoint.rotation;
    }

    currentSeasonable = null;

    Debug.Log("Kuas selesai mengoles dan kembali.");
}
}