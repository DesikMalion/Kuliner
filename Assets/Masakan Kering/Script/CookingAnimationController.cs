using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class CookingAnimationController : MonoBehaviour
{
    [Header("Animator")]
    public Animator animator;

    [Header("Animation")]
    public string animationTrigger = "Play";

    [Header("Animation Duration")]
    public float animationDuration = 1f;

    [Header("Finish")]
    [Tooltip("Dipanggil setelah animasi selesai.")]
    public UnityEvent OnAnimationFinished;

    private bool isPlaying;

    public bool IsPlaying
    {
        get
        {
            return isPlaying;
        }
    }


    // =========================================================
    // START
    // =========================================================

    private void Awake()
    {
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }
    }


    // =========================================================
    // PLAY ANIMATION
    // =========================================================

    public void PlayAnimation()
    {
        if (isPlaying)
            return;

        if (animator == null)
        {
            Debug.LogWarning(
                gameObject.name +
                " tidak memiliki Animator."
            );

            return;
        }

        animator.enabled = true;

        StartCoroutine(
            PlayAnimationRoutine()
        );
    }


    // =========================================================
    // ANIMATION ROUTINE
    // =========================================================

    private IEnumerator PlayAnimationRoutine()
    {
        isPlaying = true;

        Debug.Log(
            gameObject.name +
            " : ANIMASI DIMULAI"
        );


        // =====================================================
        // PLAY
        // =====================================================

        animator.ResetTrigger(
            animationTrigger
        );

        animator.SetTrigger(
            animationTrigger
        );


        // =====================================================
        // TUNGGU
        // =====================================================

        yield return new WaitForSeconds(
            animationDuration
        );


        // =====================================================
        // MATIKAN ANIMATOR
        // =====================================================

        animator.enabled = false;

        isPlaying = false;

        Debug.Log(
            gameObject.name +
            " : ANIMASI SELESAI"
        );


        // =====================================================
        // FINISH
        // =====================================================

        if (OnAnimationFinished != null)
        {
            OnAnimationFinished.Invoke();
        }
    }
}