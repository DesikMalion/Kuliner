using UnityEngine;

public class PrepareObject : MonoBehaviour
{
    [Header("Prepare")]
    public string objectName;

    [Tooltip("Workstation/slot tempat object ini harus diletakkan.")]
    public PrepareSlot targetSlot;

    private bool isPrepared;

    public bool IsPrepared => isPrepared;

    private void OnTriggerEnter(Collider other)
    {
        PrepareSlot slot = other.GetComponent<PrepareSlot>();

        if (slot == null)
            return;

        // Bukan slot untuk object ini
        if (slot != targetSlot)
            return;

        Prepare();
    }

    private void OnTriggerExit(Collider other)
    {
        PrepareSlot slot = other.GetComponent<PrepareSlot>();

        if (slot == null)
            return;

        if (slot != targetSlot)
            return;

        // Kalau object diambil lagi,
        // status prepare dibatalkan.
        isPrepared = false;
    }

    private void Prepare()
    {
        if (isPrepared)
            return;

        isPrepared = true;

        Debug.Log(objectName + " sudah dipersiapkan.");

        CookingManager.Instance.CheckEvent(
            CookingEventType.ObjectPrepared,
            gameObject
        );
    }
}
