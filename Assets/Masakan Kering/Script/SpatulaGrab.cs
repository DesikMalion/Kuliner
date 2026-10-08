using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SpatulaGrab : MonoBehaviour
{
    [Header("Hand Parent")]
    public Transform handParent;

    [Header("Take Ingredient Step")]
    public int takeIngredientStepIndex = 4;

    [Header("Ingredient")]
    public Ingredient targetIngredient;

    [Tooltip("Point tempat ingredient menempel.")]
    public Transform ingredientPoint;

    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;

    private bool hasIngredient = false;


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        grabInteractable =
            GetComponent<XRGrabInteractable>();

        rb = GetComponent<Rigidbody>();
    }


    // =========================================================
    // ENABLE
    // =========================================================

    private void OnEnable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.AddListener(OnGrab);
        }
    }


    // =========================================================
    // DISABLE
    // =========================================================

    private void OnDisable()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrab);
        }
    }


    // =========================================================
    // GRAB SPATULA
    // =========================================================

    private void OnGrab(SelectEnterEventArgs args)
    {
        if (handParent == null)
        {
            Debug.LogWarning(
                "Hand Parent belum diisi pada Spatula!"
            );

            return;
        }


        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        transform.SetParent(handParent);

        transform.localPosition =
            Vector3.zero;

        transform.localRotation =
            Quaternion.identity;


        // Setelah masuk tangan,
        // XR tidak perlu mengontrol spatula lagi
        grabInteractable.enabled = false;

        this.GetComponent<BoxCollider>().isTrigger = true;

        Debug.Log(
            "Spatula masuk ke tangan!"
        );
    }


    // =========================================================
    // TRIGGER INGREDIENT
    // =========================================================

    private void OnTriggerEnter(Collider other)
    {
        // =====================================================
        // CEK STEP
        // =====================================================

        if (CookingManager.Instance == null)
            return;

        if (CookingManager.Instance.CurrentStepIndex
            != takeIngredientStepIndex)
        {
            return;
        }


        // =====================================================
        // KALAU SUDAH ADA INGREDIENT
        // =====================================================

        if (hasIngredient)
            return;


        // =====================================================
        // CARI INGREDIENT
        // =====================================================

        Ingredient ingredient =
            other.GetComponentInParent<Ingredient>();

        if (ingredient == null)
            return;


        // =====================================================
        // CEK TARGET
        // =====================================================

        if (targetIngredient != null &&
            ingredient != targetIngredient)
        {
            return;
        }


        // =====================================================
        // AMBIL INGREDIENT
        // =====================================================

        TakeIngredient(ingredient);
    }


    // =========================================================
    // TAKE INGREDIENT
    // =========================================================

    private void TakeIngredient(
        Ingredient ingredient)
    {
        if (hasIngredient)
            return;

        if (ingredientPoint == null)
        {
            Debug.LogWarning(
                "Ingredient Point belum diisi!"
            );

            return;
        }


        hasIngredient = true;


        // =====================================================
        // MASUKKAN INGREDIENT KE SPATULA
        // =====================================================

        ingredient.transform.SetParent(
            ingredientPoint
        );

        ingredient.transform.localPosition =
            Vector3.zero;

        ingredient.transform.localRotation =
            Quaternion.identity;


        // =====================================================
        // MATIKAN PHYSICS INGREDIENT
        // =====================================================

        Rigidbody ingredientRb =
            ingredient.GetComponent<Rigidbody>();

        if (ingredientRb != null)
        {
            ingredientRb.isKinematic = true;
            ingredientRb.useGravity = false;
        }


        // =====================================================
        // EVENT
        // =====================================================

        if (CookingManager.Instance != null)
        {
            CookingManager.Instance.CheckEvent(
                CookingEventType.ObjectPickedUp,
                ingredient.gameObject
            );
        }


        Debug.Log(
            "SPATULA MENGAMBIL : " +
            ingredient.ingredientName
        );
    }
}