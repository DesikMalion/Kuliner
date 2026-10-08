
using UnityEngine;

public class GloveTouch : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        BreadGrabCondition bread = 
            other.GetComponentInParent<BreadGrabCondition>();

        if (bread != null)
        {
            bread.GrabBread();
        }
    }
}
