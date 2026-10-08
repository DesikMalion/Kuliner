using UnityEngine;

public class HandwashInteractable : MonoBehaviour
{
     public void WashHands()
    {
        CookingManager.Instance.CheckEvent(
            CookingEventType.WashHand,
            gameObject
        );
    }
}
