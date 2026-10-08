using UnityEngine;

public class BreadGrabCondition : MonoBehaviour
{
    [Header("Grab Point")]
    public Transform breadGrabPoint;

    private bool isGrabbed = false;

    private void OnEnable()
    {
        isGrabbed = false;
    }

    public void GrabBread()
    {
        if (isGrabbed)
            return;

        if (breadGrabPoint == null)
        {
            Debug.LogWarning("Bread Grab Point belum diisi!");
            return;
        }

        isGrabbed = true;

        // Pindahkan roti ke point
        transform.SetParent(breadGrabPoint.transform);
        transform.localPosition = new Vector3(0,0,0);
        transform.localRotation =  Quaternion.identity;

        Debug.Log("Roti berhasil diambil dengan sarung tangan!");
    }
}