using UnityEngine;

public class Seasonable : MonoBehaviour
{
    [Header("Seasoning")]
    public bool isSeasoned;

    [Header("Seasoned Color")]
    public Color seasonedColor = new Color(0.8f, 0.65f, 0.35f, 1f);

    private Renderer[] renderers;

    private Color[] originalColors;


    // =========================================================
    // AWAKE
    // =========================================================

    private void Awake()
    {
        renderers = GetComponentsInChildren<Renderer>();

        originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
        {
            if (renderers[i].material.HasProperty("_Color"))
            {
                originalColors[i] =
                    renderers[i].material.color;
            }
        }
    }


    // =========================================================
    // SEASON
    // =========================================================

    public void Season()
    {
        if (isSeasoned)
            return;

        isSeasoned = true;


        // =====================================================
        // GANTI WARNA
        // =====================================================

        SetSeasonedColor();


        Debug.Log(
            gameObject.name +
            " sudah dibumbui."
        );


        // =====================================================
        // CHECK EVENT
        // =====================================================

        if (CookingManager.Instance != null)
        {
            CookingManager.Instance.CheckEvent(
                CookingEventType.ObjectSeasoned,
                gameObject
            );
        }
    }


    // =========================================================
    // SET SEASONED COLOR
    // =========================================================

    private void SetSeasonedColor()
    {
        if (renderers == null)
            return;

        foreach (Renderer rend in renderers)
        {
            if (rend == null)
                continue;

            if (rend.material.HasProperty("_Color"))
            {
                rend.material.color =
                    seasonedColor;
            }
        }
    }


    // =========================================================
    // RESET
    // =========================================================

    public void ResetSeasoning()
    {
        isSeasoned = false;

        RestoreOriginalColor();
    }


    // =========================================================
    // RESTORE ORIGINAL COLOR
    // =========================================================

    private void RestoreOriginalColor()
    {
        if (renderers == null)
            return;

        for (int i = 0;
             i < renderers.Length;
             i++)
        {
            if (renderers[i] == null)
                continue;

            if (renderers[i].material.HasProperty("_Color"))
            {
                renderers[i].material.color =
                    originalColors[i];
            }
        }
    }
}