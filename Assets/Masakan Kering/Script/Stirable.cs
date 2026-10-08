using UnityEngine;

public class Stirable : MonoBehaviour
{
    [Header("Stir Progress")]
    [Range(0f, 1f)]
    public float stirProgress = 0f;

    [Header("Stir Settings")]
    public float requiredDistance = 1f;


    // =========================================================
    // ADD STIR PROGRESS
    // =========================================================

    public void AddStirProgress(float distance)
    {
        if (distance <= 0f)
            return;

        if (stirProgress >= 1f)
            return;


        // =====================================================
        // TAMBAH PROGRESS
        // =====================================================

        stirProgress +=
            distance / requiredDistance;

        stirProgress =
            Mathf.Clamp01(stirProgress);


        // =====================================================
        // CEK SELESAI STIR
        // =====================================================

        if (stirProgress >= 1f)
        {
            Ingredient ingredient =
                GetComponent<Ingredient>();

            if (ingredient != null)
            {
                ingredient.OnStirred();
            }
        }
    }


    // =========================================================
    // RESET
    // =========================================================

    public void ResetStir()
    {
        stirProgress = 0f;
    }
}