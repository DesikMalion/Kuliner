using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PourDetector : MonoBehaviour
{
    public int pourThreshold = 45;
    public Transform origin = null;
    public GameObject streamPrefab = null;

    public bool isPouring = false;
    private Stream currentStream = null;

    public UnityEvent PourObjEvent;
    public UnityEvent PourObjEndEvent;

    public GameObject HitObj;

    private void Update()
    {
        bool pourCheck = CalculatePourAngle() < pourThreshold;

        if (isPouring != pourCheck)
        {
            isPouring = pourCheck;

            if (isPouring)
            {
                StartPour();
                
            }
            else
            {
                EndPour();
            }
        }

        if (currentStream != null) {
            if (currentStream.HitObj != null) {
                if (HitObj == null)
                {
                    HitObj = currentStream.HitObj;
                    PourObjEvent.Invoke();
                }
                else if (HitObj != currentStream.HitObj) {
                    
                    HitObj = currentStream.HitObj;
                    PourObjEvent.Invoke();
                }
            
            }
        }
    }

    private void StartPour()
    {
        // Empty
        Debug.Log("Start Pouring");

        if (currentStream == null)
        currentStream = CreateStream();
        currentStream.Begin();
        currentStream.transform.eulerAngles = Vector3.zero;

        //PourObjEvent.Invoke();
    }

    private void EndPour()
    {
        // Empty
        Debug.Log("End Pouring");
        currentStream.End();
        PourObjEndEvent.Invoke();
        //currentStream = null;
    }

    private float CalculatePourAngle()
    {
        return transform.forward.y * Mathf.Rad2Deg;
    }

    private Stream CreateStream()
    {
        GameObject streamObject = Instantiate(
            streamPrefab,
            origin.position,
            Quaternion.identity,
            transform
        );

        return streamObject.GetComponent<Stream>();
    }
}