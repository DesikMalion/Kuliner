
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class Spatula : CookingTool
{
    // =========================================================
    // CURRENT INGREDIENT
    // =========================================================

    [SerializeField]private Ingredient currentIngredient;

    [SerializeField]private bool isHoldingIngredient = false;


    // =========================================================
    // UI
    // =========================================================

    [Header("Action UI")]
    public GameObject actionUI;

    [Header("UI Text")]
    public TMPro.TMP_Text actionText;


    // =========================================================
    // STEP INDEX SETTINGS
    // =========================================================

    [Header("Auto Action By Step Index")]

    [Tooltip("Nomor step untuk flip atau rotate. 0 = nonaktif.")]
    public int flipRotateStepIndex = 0;

    [Tooltip("Nomor step untuk mengambil ingredient. 0 = nonaktif.")]
    public int grabStepIndex = 0;

    [Tooltip("Nomor step untuk melepas ingredient. 0 = nonaktif.")]
    public int releaseStepIndex = 0;

    private int lastActionStepIndex = -1;


    // =========================================================
    // HOLD POINT
    // =========================================================

    [Header("Hold Point")]
    [Tooltip("Posisi ingredient ketika sedang dipegang spatula.")]
    public Transform holdPoint;


    // =========================================================
    // ROTATE SETTINGS
    // =========================================================

    [Header("Rotate")]

    [Tooltip("Sudut rotasi setiap kali ingredient diputar.")]
    public float rotateAngle = 90f;

    [Tooltip("Durasi animasi rotasi.")]
    public float rotateDuration = 0.3f;

    private bool isRotating;


    // =========================================================
    // INTERNAL HOLD DATA
    // =========================================================

    private Transform originalParent;

    private Rigidbody currentRigidbody;

    private XRGrabInteractable currentGrabInteractable;

    private bool originalIsKinematic;

    private bool originalUseGravity;


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
{
    if (CookingManager.Instance == null)
        return;

    if (CookingManager.Instance.CurrentStep == null)
        return;


    // =====================================================
    // CURRENT STEP
    // =====================================================

    int activeStepIndex =
        CookingManager.Instance.CurrentStepIndex;


    // =====================================================
    // FLIP / ROTATE
    // =====================================================

    // ROTATE BOLEH DILAKUKAN BERKALI-KALI
    // DALAM STEP YANG SAMA.
    if (flipRotateStepIndex > 0 &&
        activeStepIndex == flipRotateStepIndex)
    {
        if (currentIngredient == null ||
            isHoldingIngredient ||
            isRotating)
        {
            return;
        }

        // JANGAN gunakan lastActionStepIndex
        // karena rotate membutuhkan beberapa kali aksi.

        Use();

        return;
    }


    // =====================================================
    // GRAB INGREDIENT
    // =====================================================

    if (grabStepIndex > 0 &&
        activeStepIndex == grabStepIndex)
    {
        if (currentIngredient == null ||
            isHoldingIngredient)
        {
            return;
        }

        // Grab hanya boleh dilakukan sekali
        if (lastActionStepIndex == activeStepIndex)
            return;

        lastActionStepIndex = activeStepIndex;

        GrabIngredient();

        return;
    }


    // =====================================================
    // RELEASE INGREDIENT
    // =====================================================

    if (releaseStepIndex > 0 &&
        activeStepIndex == releaseStepIndex)
    {
        if (!isHoldingIngredient ||
            currentIngredient == null)
        {
            return;
        }

        // Release hanya boleh dilakukan sekali
        if (lastActionStepIndex == activeStepIndex)
            return;

        lastActionStepIndex = activeStepIndex;

        ReleaseIngredient();

        return;
    }
}


    // =========================================================
    // TRIGGER MASUK
    // =========================================================

    private void OnTriggerEnter(Collider other)
    {
         Debug.Log(
        "SPATULA TRIGGER KENA: " +
        other.name
    );
        Ingredient ingredient =
            other.GetComponentInParent<Ingredient>();

        if (ingredient == null)
            return;

        // Jangan mengganti target ketika sedang memegang ingredient.
        if (isHoldingIngredient)
            return;

        currentIngredient = ingredient;

        Debug.Log(
            "Spatula menyentuh: " +
            ingredient.ingredientName
        );

        ShowActionUI();
    }


    // =========================================================
    // TRIGGER KELUAR
    // =========================================================

    private void OnTriggerExit(Collider other)
    {
        Ingredient ingredient =
            other.GetComponentInParent<Ingredient>();

        if (ingredient == null)
            return;

        // Pertahankan target selama ingredient dipegang.
        if (isHoldingIngredient)
            return;

        if (currentIngredient == ingredient)
        {
            currentIngredient = null;

            HideActionUI();

            Debug.Log(
                "Spatula keluar dari: " +
                ingredient.ingredientName
            );
        }
    }


    // =========================================================
    // FLIP / ROTATE
    // =========================================================

    public void Use()
    {
        if (currentIngredient == null)
        {
            Debug.Log("Tidak ada ingredient.");
            return;
        }


        // =====================================================
        // ROTATE
        // =====================================================

        if (currentIngredient.requiresRotate)
        {
            RotateIngredient();
            return;
        }


        // =====================================================
        // FLIP
        // =====================================================

        if (currentIngredient.requiresFlip)
        {
            Flipable flippable =
                currentIngredient.GetComponent<Flipable>();

            if (flippable == null)
            {
                Debug.Log(
                    currentIngredient.ingredientName +
                    " tidak memiliki Flipable."
                );

                return;
            }

            flippable.Flip();

            Debug.Log(
                "STEP ACTION → FLIP: " +
                currentIngredient.ingredientName
            );

            return;
        }


        // =====================================================
        // TIDAK MEMERLUKAN AKSI
        // =====================================================

        Debug.Log(
            currentIngredient.ingredientName +
            " tidak membutuhkan flip atau rotate."
        );
    }


    // =========================================================
    // ROTATE INGREDIENT
    // =========================================================

    private void RotateIngredient()
    {
        if (currentIngredient == null)
            return;

        if (isRotating)
            return;

        if (currentIngredient.isCooked)
            return;

        StartCoroutine(RotateRoutine());
    }


    // =========================================================
    // ROTATE ROUTINE
    // =========================================================

    private System.Collections.IEnumerator RotateRoutine()
    {
        isRotating = true;

        Quaternion startRotation =
            currentIngredient.transform.rotation;

        Quaternion targetRotation =
            startRotation *
            Quaternion.Euler(
                0f,
                rotateAngle,
                0f
            );

        float elapsed = 0f;

        while (elapsed < rotateDuration)
        {
            elapsed += Time.deltaTime;

            float t = Mathf.Clamp01(
                elapsed / rotateDuration
            );

            t = Mathf.SmoothStep(0f, 1f, t);

            if (currentIngredient == null)
                break;

            currentIngredient.transform.rotation =
                Quaternion.Slerp(
                    startRotation,
                    targetRotation,
                    t
                );

            yield return null;
        }

        if (currentIngredient != null)
        {
            currentIngredient.transform.rotation =
                targetRotation;

            currentIngredient.OnRotated();

            Debug.Log(
                "STEP ACTION → ROTATE: " +
                currentIngredient.ingredientName
            );
        }

        isRotating = false;
    }


    // =========================================================
    // GRAB INGREDIENT
    // =========================================================

    private void GrabIngredient()
    {
        if (currentIngredient == null)
            return;

        if (isHoldingIngredient)
            return;

        if (holdPoint == null)
        {
            Debug.LogWarning(
                "Hold Point belum dipasang di Spatula."
            );

            return;
        }


        // =====================================================
        // SIMPAN DATA INGREDIENT
        // =====================================================

        originalParent =
            currentIngredient.transform.parent;

        currentRigidbody =
            currentIngredient.GetComponent<Rigidbody>();

        currentGrabInteractable =
            currentIngredient.GetComponent<XRGrabInteractable>();


        // =====================================================
        // SIMPAN STATE RIGIDBODY
        // =====================================================

        if (currentRigidbody != null)
        {
            originalIsKinematic =
                currentRigidbody.isKinematic;

            originalUseGravity =
                currentRigidbody.useGravity;

            currentRigidbody.isKinematic = true;
            currentRigidbody.useGravity = false;
        }


        // =====================================================
        // LEPAS XR GRAB JIKA SEDANG DIPEGANG CONTROLLER
        // =====================================================

        if (currentGrabInteractable != null)
        {
            if (currentGrabInteractable.isSelected)
            {
                IXRSelectInteractor interactor =
                    currentGrabInteractable.firstInteractorSelecting;

                if (interactor != null)
                {
                    currentGrabInteractable.interactionManager
                        .SelectExit(
                            interactor,
                            currentGrabInteractable
                        );
                }
            }

            currentGrabInteractable.enabled = false;
        }


        // =====================================================
        // PINDAHKAN KE HOLD POINT
        // =====================================================

        currentIngredient.transform.SetParent(holdPoint);

        currentIngredient.transform.SetPositionAndRotation(
            holdPoint.position,
            holdPoint.rotation
        );
  CookingManager.Instance.CheckEvent(
        CookingEventType.ObjectPickedUp,
        currentIngredient.gameObject
    );

        isHoldingIngredient = true;

        UpdateHoldingUI();

        Debug.Log(
            "STEP ACTION → GRAB: " +
            currentIngredient.ingredientName
        );
    }


    // =========================================================
    // RELEASE INGREDIENT
    // =========================================================

    private void ReleaseIngredient()
    {
        if (currentIngredient == null)
            return;


        Ingredient releasedIngredient = currentIngredient;

        Debug.Log(
            "STEP ACTION → RELEASE: " +
            releasedIngredient.ingredientName
        );


        // =====================================================
        // KEMBALIKAN PARENT
        // =====================================================

        releasedIngredient.transform.SetParent(originalParent);


        // =====================================================
        // KEMBALIKAN RIGIDBODY
        // =====================================================

        if (currentRigidbody != null)
        {
            currentRigidbody.isKinematic =
                originalIsKinematic;

            currentRigidbody.useGravity =
                originalUseGravity;
        }


        // =====================================================
        // AKTIFKAN XR GRAB KEMBALI
        // =====================================================

        if (currentGrabInteractable != null)
        {
            currentGrabInteractable.enabled = true;
        }


        // =====================================================
        // RESET DATA HOLD
        // =====================================================

        currentRigidbody = null;
        currentGrabInteractable = null;
        originalParent = null;

        isHoldingIngredient = false;
        isRotating = false;

        currentIngredient = null;

        HideActionUI();
    }


    // =========================================================
    // UI NORMAL
    // =========================================================

    private void ShowActionUI()
    {
        if (actionUI != null)
            actionUI.SetActive(true);

        if (actionText != null)
        {
            actionText.text =
                "Balik / Putar\n" +
                "Ambil";
        }
    }


    // =========================================================
    // UI SAAT MEMBAWA
    // =========================================================

    private void UpdateHoldingUI()
    {
        if (actionUI != null)
            actionUI.SetActive(true);

        if (actionText != null)
        {
            actionText.text =
                "Balik / Putar\n" +
                "Lepas";
        }
    }


    // =========================================================
    // HIDE UI
    // =========================================================

    private void HideActionUI()
    {
        if (actionUI != null)
            actionUI.SetActive(false);
    }
}
