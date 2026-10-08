using UnityEngine;

public class CookingPan : MonoBehaviour
{
    [Header("Oil")]
    public bool isOiled = false;

    [Header("Animation")]
    public CookingAnimationController animationController;
    public GameObject oil;

    // =========================================================
    // START
    // =========================================================

    private void Awake()
    {
        if (animationController == null)
        {
            animationController =
                GetComponent<CookingAnimationController>();
        }
    }


    // =========================================================
    // ADD OIL
    // =========================================================

    public void AddOil()
    {
        if (isOiled)
            return;

        // Wajan sudah diberi minyak
        isOiled = true;

        Debug.Log(
            gameObject.name +
            " : WAJAN SUDAH DIBERI MINYAK"
        );

        // =====================================================
        // PLAY ANIMATION
        // =====================================================

        if (animationController != null)
        {
            animationController.PlayAnimation();
oil.SetActive(true);
        }
        else
        {
            FinishOil();
        }
    }


    // =========================================================
    // FINISH OIL
    // =========================================================

    public void FinishOil()
    {
        Debug.Log(
            gameObject.name +
            " : PROSES MINYAK SELESAI"
        );


        // =====================================================
        // EVENT COOKING
        // =====================================================

        CookingEvent.Instance.Trigger(
            CookingEventType.ObjectPrepared,
            gameObject
        );
    }
}