using UnityEngine;
using UnityEngine.Events;

public class AparAction : MonoBehaviour
{
    public GameObject WaterObj;
    public bool isPlayerInRange = false;
    bool isGrabbed = false;
    public UnityEvent OnGrab;
    public UnityEvent OnRealease;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (isPlayerInRange && isGrabbed)
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

    //buat fungsi jika boxCollider terkena player, boxCollider ini ada di objek lain
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("PlayerHand"))
        {
            // Lakukan aksi yang diinginkan ketika boxCollider terkena player
            //Debug.Log("Player has entered the box collider!");
            isPlayerInRange = true;
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("PlayerHand"))
        {
            // Lakukan aksi yang diinginkan ketika player keluar dari boxCollider
            //Debug.Log("Player has exited the box collider!");
            isPlayerInRange = false;
        }
    }


}
