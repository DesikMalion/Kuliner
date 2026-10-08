using UnityEngine;

public class Flipable : MonoBehaviour
{
    public enum FlipAxis
    {
        X,
        Y,
        Z
    }

    [Header("Flip Settings")]
    public float flipDuration = 0.5f;

    [Header("Flip Axis")]
    public FlipAxis flipAxis = FlipAxis.Y;

    private bool isFlipping;


    // =========================================================
    // FLIP
    // =========================================================

    public void Flip()
    {
        if (isFlipping)
            return;

        // Cek apakah object punya Ingredient
        Ingredient ingredient =
            GetComponent<Ingredient>();

        if (ingredient != null)
        {
            // Kalau ingredient tidak membutuhkan flip
            if (!ingredient.requiresFlip)
            {
                Debug.Log(
                    ingredient.ingredientName +
                    " tidak membutuhkan flip."
                );

                return;
            }

            // Kalau sudah matang, jangan flip lagi
            if (ingredient.isCooked)
                return;

            // Kalau sudah pernah flip
            if (ingredient.isFlipped)
                return;
        }

        StartCoroutine(FlipRoutine());
    }


    // =========================================================
    // FLIP ANIMATION
    // =========================================================

    private System.Collections.IEnumerator FlipRoutine()
    {
        isFlipping = true;

        // =====================================================
        // ROTASI AWAL
        // =====================================================

        Quaternion startRotation =
            transform.localRotation;

        // =====================================================
        // TENTUKAN SUMBU FLIP
        // =====================================================

        Vector3 axis = Vector3.up;

        switch (flipAxis)
        {
            case FlipAxis.X:
                axis = Vector3.right;
                break;

            case FlipAxis.Y:
                axis = Vector3.up;
                break;

            case FlipAxis.Z:
                axis = Vector3.forward;
                break;
        }

        // =====================================================
        // ROTASI TARGET
        // =====================================================

        Quaternion targetRotation =
            startRotation *
            Quaternion.AngleAxis(
                180f,
                axis
            );

        float elapsed = 0f;

        while (elapsed < flipDuration)
        {
            elapsed += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed / flipDuration
                );

            t = Mathf.SmoothStep(
                0f,
                1f,
                t
            );

            transform.localRotation =
                Quaternion.Slerp(
                    startRotation,
                    targetRotation,
                    t
                );

            yield return null;
        }

        // Pastikan tepat di posisi akhir
        transform.localRotation =
            targetRotation;


        // =====================================================
        // INFORMASIKAN KE INGREDIENT
        // =====================================================

        Ingredient ingredient =
            GetComponent<Ingredient>();

        if (ingredient != null)
        {
            ingredient.OnFlipped();
        }


        // =====================================================
        // EVENT COOKING
        // =====================================================

        CookingManager.Instance.CheckEvent(
            CookingEventType.ObjectFlipped,
            gameObject
        );


        isFlipping = false;

        Debug.Log(
            gameObject.name +
            " berhasil di-flip menggunakan sumbu " +
            flipAxis
        );
    }
}