using UnityEngine;
using System.Collections;

public class OvenDoor : MonoBehaviour
{
    [Header("Door")]
    public Transform door;

    [Header("Rotation Y")]
    public float closedAngle = 0f;
    public float openAngle = -90f;

    [Header("Animation")]
    public float duration = 0.5f;

    private bool isOpen;
    private bool isMoving;

    private void Awake()
    {
        if (door == null)
            door = transform;
    }

    public void Open()
    {
        if (isMoving || isOpen)
            return;
CookingManager.Instance.CheckEvent(
            CookingEventType.OvenDoorOpened,
            gameObject
        );
        StartCoroutine(
            RotateDoor(openAngle, true)
        );
    }

    public void Close()
    {
        if (isMoving || !isOpen)
            return;
CookingManager.Instance.CheckEvent(
            CookingEventType.OvenDoorClosed,
            gameObject
        );
        StartCoroutine(
            RotateDoor(closedAngle, false)
        );
    }

    public void Toggle()
    {
        if (isMoving)
            return;

        if (isOpen)
            Close();
        else
            Open();
    }

    private IEnumerator RotateDoor(
        float targetY,
        bool opening)
    {
        isMoving = true;

        // Ambil posisi Y saat ini
        float startY =
            door.localEulerAngles.y;

        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;

            float t =
                Mathf.Clamp01(
                    elapsed / duration
                );

            t = Mathf.SmoothStep(
                0f,
                1f,
                t
            );

            // HANYA Y yang berubah
            float currentY =
                Mathf.LerpAngle(
                    startY,
                    targetY,
                    t
                );

            Vector3 euler =
                door.localEulerAngles;

            euler.y = currentY;

            door.localEulerAngles =
                euler;

            yield return null;
        }

        // Pastikan posisi akhir tepat
        Vector3 finalEuler =
            door.localEulerAngles;

        finalEuler.y = targetY;

        door.localEulerAngles =
            finalEuler;

        isOpen = opening;
        isMoving = false;
    }
}