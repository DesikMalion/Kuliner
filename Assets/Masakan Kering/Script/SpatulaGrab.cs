using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class SpatulaGrab : MonoBehaviour
{
    [Header("Hand Parent")]
    public Transform handParent;

    [Header("Take Ingredient Step")]
    public int takeIngredientStepIndex = 4;

    [Header("Ingredient Target")]
    public Ingredient targetIngredient;
    public Ingredient targetIngredient2;

    [Tooltip("Point tempat ingredient menempel.")]
    public Transform ingredientPoint;

    [Header("Settings")]
    [Tooltip("Jumlah maksimal ingredient yang bisa dibawa spatula.")]
    public int maxIngredient = 2;

    private XRGrabInteractable grabInteractable;
    private Rigidbody rb;

    // Jumlah ingredient yang sedang dibawa
    private int ingredientCount = 0;


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


        // =====================================================
        // MATIKAN PHYSICS SPATULA
        // =====================================================

        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }


        // =====================================================
        // MASUKKAN KE TANGAN
        // =====================================================

        transform.SetParent(handParent);

        transform.localPosition =
            Vector3.zero;

        transform.localRotation =
            Quaternion.identity;


        // =====================================================
        // XR TIDAK PERLU MENGONTROL SPATULA LAGI
        // =====================================================

        grabInteractable.enabled = false;


        BoxCollider box =
            GetComponent<BoxCollider>();

        if (box != null)
        {
            box.isTrigger = true;
        }


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
        // CEK COOKING MANAGER
        // =====================================================

        if (CookingManager.Instance == null)
            return;


        // =====================================================
        // CEK STEP
        // =====================================================

        if (CookingManager.Instance.CurrentStepIndex
            != takeIngredientStepIndex)
        {
            return;
        }


        // =====================================================
        // CEK JUMLAH INGREDIENT
        // =====================================================

        if (ingredientCount >= maxIngredient)
        {
            return;
        }


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

        bool isTarget =
            ingredient == targetIngredient ||
            ingredient == targetIngredient2;

        if (!isTarget)
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
        // =====================================================
        // CEK MAX
        // =====================================================

        if (ingredientCount >= maxIngredient)
            return;


        // =====================================================
        // CEK INGREDIENT POINT
        // =====================================================

        if (ingredientPoint == null)
        {
            Debug.LogWarning(
                "Ingredient Point belum diisi!"
            );

            return;
        }


        // =====================================================
        // TAMBAH JUMLAH
        // =====================================================

        ingredientCount++;


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
        // MATIKAN COLLIDER INGREDIENT
        // =====================================================

        Collider[] colliders =
            ingredient.GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
        {
            col.enabled = false;
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


        // =====================================================
        // DEBUG
        // =====================================================

        Debug.Log(
            "SPATULA MENGAMBIL : " +
            ingredient.ingredientName +
            " | Jumlah : " +
            ingredientCount +
            "/" +
            maxIngredient
        );
    }
}