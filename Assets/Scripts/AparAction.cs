using UnityEngine;
using UnityEngine.Events;

public class AparAction : MonoBehaviour
{
    public GameObject WaterObj;

    bool isGrabbed = false;
    public UnityEvent OnGrab;
    public UnityEvent OnRealease;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isGrabbed)
        {
            OnGrabAction();
        }
        else
        {
            OnReleaseAction();
        }
    }

    public void SetGrab(bool isGrab) { 
        
        isGrabbed = isGrab;

    }

    void OnGrabAction()
    {
        if(WaterObj.activeSelf)return;
        WaterObj.SetActive(true);
        OnGrab.Invoke();
    }

    void OnReleaseAction()
    {
        if (!WaterObj.activeSelf) return;
        WaterObj.SetActive(false);
        OnRealease.Invoke();
    }



}
