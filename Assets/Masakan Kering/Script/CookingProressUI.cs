
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class CookingProgressUI : MonoBehaviour
{
    public static CookingProgressUI Instance { get; private set; }

    [Header("Progress UI")]
    public GameObject progressPanel;
    public Image progressImage;

    [Header("Progress Text")]
    public TMP_Text titleText;
    public TMP_Text percentText;

    [Header("Complete UI")]
    public GameObject completePanel;
    public TMP_Text completeTitleText;
    public TMP_Text completeText;

    [Header("Complete Delay")]
    public float completeDisplayTime = 3f;

    private GameObject currentOwner;

    private Coroutine completeCoroutine;

    private void Awake()
    {
        Instance = this;

        if (progressPanel != null)
            progressPanel.SetActive(false);

        if (completePanel != null)
            completePanel.SetActive(false);
    }

    // =====================================================
    // SHOW PROGRESS
    // =====================================================

    public void Show(
        GameObject owner,
        string title)
    {
        // Batalkan timer complete sebelumnya
        if (completeCoroutine != null)
        {
            StopCoroutine(completeCoroutine);
            completeCoroutine = null;
        }

        currentOwner = owner;

        if (progressPanel != null)
            progressPanel.SetActive(true);

        if (completePanel != null)
            completePanel.SetActive(false);

        if (titleText != null)
            titleText.text = title;

        SetProgress(owner, 0f);
    }

    // =====================================================
    // UPDATE PROGRESS
    // =====================================================

    public void SetProgress(
        GameObject owner,
        float progress)
    {
        if (currentOwner != owner)
            return;

        progress =
            Mathf.Clamp01(progress);

        if (progressImage != null)
            progressImage.fillAmount = progress;

        if (percentText != null)
        {
            percentText.text =
                Mathf.RoundToInt(
                    progress * 100f
                ) + "%";
        }
    }

    // =====================================================
    // SHOW COMPLETE
    // =====================================================

    public void ShowComplete(
        GameObject owner,
        string title,
        string message)
    {
        if (currentOwner != owner)
            return;

        if (progressPanel != null)
            progressPanel.SetActive(false);

        if (completePanel != null)
            completePanel.SetActive(true);

        if (completeTitleText != null)
            completeTitleText.text = title;

        if (completeText != null)
            completeText.text = message;

        currentOwner = null;

        // Mulai timer 3 detik
        if (completeCoroutine != null)
        {
            StopCoroutine(completeCoroutine);
        }

        completeCoroutine =
            StartCoroutine(
                HideCompleteAfterDelay()
            );
    }

    // =====================================================
    // HIDE COMPLETE AFTER DELAY
    // =====================================================

    private IEnumerator HideCompleteAfterDelay()
    {
        yield return new WaitForSeconds(
            completeDisplayTime
        );

        if (completePanel != null)
            completePanel.SetActive(false);

        completeCoroutine = null;
    }

    // =====================================================
    // HIDE
    // =====================================================

    public void Hide()
    {
        if (completeCoroutine != null)
        {
            StopCoroutine(completeCoroutine);
            completeCoroutine = null;
        }

        currentOwner = null;

        if (progressPanel != null)
            progressPanel.SetActive(false);

        if (completePanel != null)
            completePanel.SetActive(false);
    }
}

