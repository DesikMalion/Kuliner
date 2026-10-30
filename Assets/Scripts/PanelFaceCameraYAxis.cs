using UnityEngine;

public class PanelFaceCameraYAxis : MonoBehaviour
{
    private Transform mainCamera;
    private bool isGrabbed = false;

    [Tooltip("Centang jika panel UI terlihat terbalik (mirror) saat menghadap kamera")]
    public bool flipDirection = true;

    void Start()
    {
        if (Camera.main != null)
        {
            mainCamera = Camera.main.transform;
        }
    }

    // Panggil fungsi ini dari event Select Entered di XR Grab Interactable
    public void OnGrabbed()
    {
        isGrabbed = true;
    }

    // Panggil fungsi ini dari event Select Exited di XR Grab Interactable
    public void OnReleased()
    {
        isGrabbed = false;
    }

    void LateUpdate()
    {
        if (isGrabbed && mainCamera != null)
        {
            // Hitung jarak dan arah dari panel ke kamera
            Vector3 directionToCamera = mainCamera.position - transform.position;

            // Kunci sumbu X dan Z dengan membuat nilai arah Y menjadi 0 (tetap tegak lurus)
            directionToCamera.y = 0;

            if (directionToCamera != Vector3.zero)
            {
                // UI Canvas Unity biasanya menghadap ke arah sebaliknya dari sumbu Z positif
                if (flipDirection)
                {
                    directionToCamera = -directionToCamera;
                }

                // Terapkan rotasi akhir
                transform.rotation = Quaternion.LookRotation(directionToCamera);
            }
        }
    }
}