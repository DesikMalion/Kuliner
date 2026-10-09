using UnityEngine;

public class ObjectPiringPencucian : MonoBehaviour
{
    public bool isBersih = false;
    public bool isDicuci = false;
    public bool isSnapped = false;
    public GameObject[] KotoranPiring;
    int piringBersih = 0;

    private float extinguishTime = 1f;
    private float maxScale = 1f;

    private float extinguishProgress;
    private float extinguishProgressNow;
    private Vector3 initialScale = Vector3.one;

    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetPiring()
    {
        isBersih = false;
        isSnapped = false;
        isDicuci = false;
        piringBersih = 0;
        for (int i = 0; i < KotoranPiring.Length; i++)
        {
            KotoranPiring[i].SetActive(false);
        }
        KotoranPiring[0].SetActive(true);
        extinguishProgress = 0f;
        extinguishProgressNow = 0f;
    }

    public void CuciPiring()
    {
        if (isBersih)
        {
            return;
        }

        extinguishProgress += Time.deltaTime;

        extinguishProgressNow = Mathf.Clamp01(extinguishProgress / extinguishTime);

        if (extinguishProgressNow == 1f)
        {
            extinguishProgressNow = 0f;
            extinguishProgress = 0f;
            
            for (int i = 0; i < KotoranPiring.Length; i++)
            {
                KotoranPiring[i].SetActive(false);
            }
            KotoranPiring[piringBersih].SetActive(true);

            if (piringBersih >= KotoranPiring.Length-1)
            {
                if(isSnapped)
                isBersih = true;
            }
            else {

                
                piringBersih++;
            }

                
        }

        // Api mengecil
        //fireObject.transform.parent.localScale = initialScale * (1f - progress);
    }
}
