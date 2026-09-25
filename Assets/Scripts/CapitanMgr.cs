using UnityEngine;
using UnityEngine.Events;

public class CapitanMgr : MonoBehaviour
{

    public bool mencapit = false;
    public Animator AnimatorCapitan;
    GameObject objDicapit;
    public UnityEvent OnTriggerSocket;

    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetMencapit(bool isCapit) {

        if (mencapit != isCapit)
        {
            mencapit = isCapit;

            if (isCapit)
            {
                AnimatorCapitan.Play("CapitanTutup");
            }
            else
            {
                AnimatorCapitan.Play("CapitanBuka");

            }
        }
    
    }


    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Target" && !objDicapit && !mencapit) {
            objDicapit = other.transform.parent.gameObject;
            other.enabled = false;
            objDicapit.transform.parent = transform;
            objDicapit.transform.localEulerAngles = Vector3.zero;
            objDicapit.transform.localPosition = Vector3.zero;

            SetMencapit(true);
        }
        
        if (other.gameObject.tag == "Finish" && objDicapit) {
            objDicapit.transform.parent = other.transform.parent;
            objDicapit.transform.localEulerAngles = Vector3.zero;
            objDicapit.transform.localPosition = Vector3.zero;
            objDicapit = null;
            SetMencapit(false);

            OnTriggerSocket.Invoke();
}
    }
}
