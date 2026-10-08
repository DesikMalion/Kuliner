using UnityEngine;

public class Stirtool : MonoBehaviour
{
    [Header("Stir Settings")]

    [Tooltip("Minimal jarak gerakan agar dianggap sedang mengaduk.")]
    public float movementThreshold = 0.005f;

    [Tooltip("Seberapa besar gerakan tool mempengaruhi progress.")]
    public float stirMultiplier = 1f;


    [Header("Movement Detection")]

    [Tooltip("Object yang digunakan untuk mendeteksi pergerakan.")]
    public Transform stirObject;


    private Stirable currentIngredient;

    private Vector3 lastPosition;

    private bool isTouchingIngredient;


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        if (stirObject == null)
        {
            stirObject = transform;
        }

        lastPosition =
            stirObject.position;
    }


    // =========================================================
    // UPDATE
    // =========================================================

    private void Update()
    {
        DetectMovement();

        lastPosition =
            stirObject.position;
    }


    // =========================================================
    // DETECT MOVEMENT
    // =========================================================

    private void DetectMovement()
    {
        // Tidak sedang menyentuh ingredient
        if (!isTouchingIngredient)
            return;

        // Tidak ada ingredient
        if (currentIngredient == null)
            return;


        // =====================================================
        // HITUNG JARAK PERGERAKAN
        // =====================================================

        float movement =
            Vector3.Distance(
                stirObject.position,
                lastPosition
            );


        // =====================================================
        // TOOL DIAM
        // =====================================================

        if (movement < movementThreshold)
        {
            return;
        }


        // =====================================================
        // TOOL BERGERAK
        // =====================================================

        float stirAmount =
            movement * stirMultiplier;


        currentIngredient.AddStirProgress(
            stirAmount
        );


        Debug.Log(
            "STIR MOVEMENT : " +
            movement +
            " | PROGRESS : " +
            currentIngredient.stirProgress
        );
    }


    // =========================================================
    // TRIGGER ENTER
    // =========================================================

    private void OnTriggerEnter(Collider other)
    {
        Stirable stirable =
            other.GetComponentInParent<Stirable>();


        if (stirable == null)
            return;


        currentIngredient =
            stirable;

        isTouchingIngredient =
            true;


        // Reset posisi awal movement
        lastPosition =
            stirObject.position;


        Debug.Log(
            "StirTool menyentuh : " +
            stirable.gameObject.name
        );
    }


    // =========================================================
    // TRIGGER EXIT
    // =========================================================

    private void OnTriggerExit(Collider other)
    {
        Stirable stirable =
            other.GetComponentInParent<Stirable>();


        if (stirable == null)
            return;


        if (stirable != currentIngredient)
            return;


        currentIngredient = null;

        isTouchingIngredient =
            false;


        Debug.Log(
            "StirTool keluar dari ingredient."
        );
    }


    // =========================================================
    // RESET
    // =========================================================

    public void ResetTool()
    {
        currentIngredient = null;

        isTouchingIngredient =
            false;


        if (stirObject != null)
        {
            lastPosition =
                stirObject.position;
        }
    }
}