using UnityEngine;
using UnityEngine.Events;

public class WaterPourDetect : MonoBehaviour
{
    public PourDetector PourDetector;

    public UnityEvent OnPouringStop;

    public float scaleNow = 0f;

    [SerializeField] private float extinguishTime = 4f;
    [SerializeField] private float maxScale = 1f;

    private float pouringProgress = 0;
    private Vector3 initialScale = Vector3.zero;
    bool maxScaleChanged = false;

    public bool isPouring = false;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (maxScaleChanged) return;
        if (isPouring)
        {
            Pouring();
        }

    }

    public void SetPouring(bool isPour)
    {
        if (isPour)
        {
            if (PourDetector.HitObj != null) {
                if (PourDetector.HitObj.layer == gameObject.layer)
                {
                    isPouring = true;
                }
                else { 
                    isPouring = false ;
                    OnPouringStop.Invoke();
                }
            }
        }
        else { 
            isPouring=false;
            OnPouringStop.Invoke();
        }
   
    }


    void Pouring()
    {
         if (maxScaleChanged)
            return;

        pouringProgress += Time.deltaTime;

        float progress = Mathf.Clamp01(pouringProgress / extinguishTime);

        // Api mengecil
        float y = initialScale.y;
        y = (0f + progress);
        transform.parent.localScale = new Vector3(1, y, 1);

        scaleNow = y;

        if (y >= 1)
        {
            maxScaleChanged = true;
            transform.parent.localScale = Vector3.one;
            y = 1;
            scaleNow = y;
        }

    }

}
