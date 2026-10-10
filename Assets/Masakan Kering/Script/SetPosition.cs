using UnityEngine;

public class SetPosition : MonoBehaviour
{
    public Transform target;
    public GameObject spline;
    public Transform goal;
   

   public void SetPosisitonTarget()
    {
        target.transform.position = goal.transform.position;
        target.transform.rotation = goal.transform.rotation;
        spline.gameObject.SetActive(false);
    }
}
