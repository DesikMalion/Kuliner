
using UnityEngine;

public class CookingGlove : MonoBehaviour
{
    [Header("Glove Settings")]
    public GameObject gloveOnHand;
    public GameObject gloveOnTable;

    public bool IsWearingGlove { get; private set; }

    public void OnSelectGlove()
    {
        WearGlove();
    }

    public void WearGlove()
    {
        if (IsWearingGlove)
            return;

        IsWearingGlove = true;

        if (gloveOnTable != null)
            gloveOnTable.SetActive(false);

        if (gloveOnHand != null)
            gloveOnHand.SetActive(true);

        Debug.Log("Sarung tangan sudah dipakai!");

        CookingEvent.Instance.Trigger(
            CookingEventType.UseGlove,
            gameObject
        );
    }
}
