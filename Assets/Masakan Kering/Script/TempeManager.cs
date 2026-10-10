using UnityEngine;

public class TempeManager : MonoBehaviour
{
    public PlaceOnPlate[] tempe_place;
    public bool allPlaced;
    public GameObject tools;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        allPlaced = true;
        for (int i = 0; i < tempe_place.Length; i++)
        {
            if (!tempe_place[i].isPlaced)
            {
                allPlaced = false;
            }
        }

        if (allPlaced)
        {
            tools.SetActive(false);
        }
    }
}
