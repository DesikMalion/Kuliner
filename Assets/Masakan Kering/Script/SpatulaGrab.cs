
using System.Collections.Generic;
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

    // Jumlah ingredient yang sedang dibawa.
    private int ingredientCount = 0;

    // Ingredient yang sudah diambil agar tidak diambil dua kali.
    private readonly HashSet<Ingredient> takenIngredients =
        new HashSet<Ingredient>();

    // Ingredient harus keluar lalu masuk kembali sebelum diambil.
    private readonly HashSet<Ingredient> exitedIngredients =
        new HashSet<Ingredient>();

    // Menghindari trigger berulang dari beberapa collider
    // milik ingredient yang sama.
    private readonly HashSet<Ingredient> ingredientsInside =
        new HashSet<Ingredient>();

    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        grabInteractable = GetComponent<XRGrabInteractable>();
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
                "Hand Parent belum diisi pada Spatula!",
                this
            );

            return;
        }

        // Matikan physics spatula.
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = false;
        }

        // Masukkan spatula ke tangan.
        transform.SetParent(handParent);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        // XR tidak perlu mengontrol spatula lagi.
        if (grabInteractable != null)
        {
            grabInteractable.enabled = false;
        }

        BoxCollider box = GetComponent<BoxCollider>();

        if (box != null)
        {
            box.isTrigger = true;
        }

        Debug.Log("Spatula masuk ke tangan!");
    }

    // =========================================================
    // TRIGGER INGREDIENT MASUK
    // =========================================================

    private void OnTriggerEnter(Collider other)
    {
        Ingredient ingredient =
            other.GetComponentInParent<Ingredient>();

        if (ingredient == null)
            return;

        // Jika ingredient ini sudah diambil, abaikan.
        if (takenIngredients.Contains(ingredient))
            return;

        // Tunggu step pengambilan yang benar.
        if (CookingManager.Instance == null)
            return;

        if (CookingManager.Instance.CurrentStepIndex
            != takeIngredientStepIndex)
        {
            return;
        }

        // Pastikan ingredient termasuk target.
        bool isTarget =
            ingredient == targetIngredient ||
            ingredient == targetIngredient2;

        if (!isTarget)
            return;

        // Catat ingredient yang sedang berada di area trigger.
        ingredientsInside.Add(ingredient);

        // Harus sudah keluar sebelumnya, baru boleh diambil.
        if (!exitedIngredients.Contains(ingredient))
        {
            Debug.Log(
                ingredient.ingredientName +
                " terdeteksi. Keluar dari collider lalu masuk lagi untuk mengambil."
            );

            return;
        }

        // Cek jumlah maksimum.
        if (ingredientCount >= maxIngredient)
            return;

        // Ingredient sudah keluar dan masuk kembali.
        exitedIngredients.Remove(ingredient);

        TakeIngredient(ingredient);
    }

    // =========================================================
    // TRIGGER INGREDIENT KELUAR
    // =========================================================

    private void OnTriggerExit(Collider other)
    {
        Ingredient ingredient =
            other.GetComponentInParent<Ingredient>();

        if (ingredient == null)
            return;

        if (takenIngredients.Contains(ingredient))
            return;

        // Hanya proses ingredient yang memang menjadi target.
        bool isTarget =
            ingredient == targetIngredient ||
            ingredient == targetIngredient2;

        if (!isTarget)
            return;

        // Tandai bahwa ingredient sudah keluar dari collider.
        exitedIngredients.Add(ingredient);
        ingredientsInside.Remove(ingredient);

        Debug.Log(
            ingredient.ingredientName +
            " keluar dari spatula. Masuk kembali untuk mengambil."
        );
    }

    // =========================================================
    // TAKE INGREDIENT
    // =========================================================

    private void TakeIngredient(Ingredient ingredient)
    {
        if (ingredient == null)
            return;

        if (ingredientCount >= maxIngredient)
            return;

        if (takenIngredients.Contains(ingredient))
            return;

        if (ingredientPoint == null)
        {
            Debug.LogWarning(
                "Ingredient Point belum diisi!",
                this
            );

            return;
        }

        // Catat agar tidak terambil dua kali.
        takenIngredients.Add(ingredient);

        // Tambah jumlah ingredient.
        ingredientCount++;

        // Pindahkan ingredient ke spatula.
        ingredient.transform.SetParent(ingredientPoint);
        ingredient.transform.localPosition = Vector3.zero;
        ingredient.transform.localRotation = Quaternion.identity;

        // Matikan physics ingredient.
        Rigidbody ingredientRb =
            ingredient.GetComponent<Rigidbody>();

        if (ingredientRb != null)
        {
            ingredientRb.isKinematic = true;
            ingredientRb.useGravity = false;
        }

        // Matikan collider agar tidak memicu trigger berulang.
        Collider[] colliders =
            ingredient.GetComponentsInChildren<Collider>();

        foreach (Collider col in colliders)
        {
            col.enabled = false;
        }

        // Kirim event ke sistem task.
        if (CookingManager.Instance != null)
        {
            CookingManager.Instance.CheckEvent(
                CookingEventType.ObjectPickedUp,
                ingredient.gameObject
            );
        }

        Debug.Log(
            "SPATULA MENGAMBIL: " +
            ingredient.ingredientName +
            " | Jumlah: " +
            ingredientCount +
            "/" +
            maxIngredient
        );
    }
}