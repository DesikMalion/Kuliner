using UnityEngine;

public class Seasonable : MonoBehaviour
{
    [Header("Seasoning")]
    public bool isSeasoned;

    // =========================================================
    // SEASON
    // =========================================================

    public void Season()
    {
        if (isSeasoned)
            return;

        isSeasoned = true;

        Debug.Log(
            gameObject.name +
            " sudah dibumbui."
        );

        CookingManager.Instance.CheckEvent(
            CookingEventType.ObjectSeasoned,
            gameObject
        );
    }


    // =========================================================
    // RESET
    // =========================================================

    public void ResetSeasoning()
    {
        isSeasoned = false;
    }
}