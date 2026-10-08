using UnityEngine;

public class SeasoningBowl : MonoBehaviour
{
    [Header("Seasoning")]
    public bool hasSeasoning = true;

    public void TakeSeasoning()
    {
        if (!hasSeasoning)
        {
            Debug.Log("Bumbu di mangkok sudah habis.");
            return;
        }

        Debug.Log(
            gameObject.name +
            " : bumbu diambil."
        );
    }
}