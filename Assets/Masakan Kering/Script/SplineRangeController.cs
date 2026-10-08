using UnityEngine;

public class SplineFillController : MonoBehaviour
{
    [Header("Target")]
    public Renderer targetRenderer;

    [Header("Fill Amount")]
    [Range(0f, 1f)]
    public float fillAmount = 0f;

    private MaterialPropertyBlock propertyBlock;

    private static readonly int FillAmountID = Shader.PropertyToID("_Fill");

    private void Awake()
    {
        propertyBlock = new MaterialPropertyBlock();
        UpdateFill();
    }

    private void OnValidate()
    {
        if (propertyBlock == null)
            propertyBlock = new MaterialPropertyBlock();

        UpdateFill();
    }

    private void Update()
    {
        UpdateFill();
    }

    private void UpdateFill()
    {
        if (targetRenderer == null)
            return;

        targetRenderer.GetPropertyBlock(propertyBlock);

        propertyBlock.SetFloat(FillAmountID, fillAmount);

        targetRenderer.SetPropertyBlock(propertyBlock);
    }

    // =========================================================
    // SET FILL DARI SCRIPT LAIN
    // =========================================================

    public void SetFill(float value)
    {
        fillAmount = Mathf.Clamp01(value);
        UpdateFill();
    }

    // =========================================================
    // RESET
    // =========================================================

    public void ResetFill()
    {
        SetFill(0f);
    }

    // =========================================================
    // FULL
    // =========================================================

    public void FillFull()
    {
        SetFill(1f);
    }
}