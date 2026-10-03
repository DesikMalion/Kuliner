using UnityEngine;
using System.Collections;

public class SingleDoorController : MonoBehaviour
{
    [System.Serializable]
    public class DoorLeaf
    {
        public Transform doorTransform;   // daun pintu (rotasi Y, pivot di engsel)
        public Transform handleTransform; // handle pintu (rotasi Z)

        [Header("Batas Rotasi")]
        public float handleMaxX;
        public float doorMaxY;

        [HideInInspector] public Quaternion doorInitialRot;
        [HideInInspector] public Quaternion handleInitialRot;
        [HideInInspector] public bool isOpen = false;
        [HideInInspector] public bool isAnimating = false;
    }

    [Header("Daun Pintu Kiri")]
    public DoorLeaf leftDoor = new DoorLeaf { handleMaxX = 15f, doorMaxY = 85f };
    [Header("Kecepatan Animasi")]
    [SerializeField] private float handleAnimDuration = 0.2f;
    [SerializeField] private float doorAnimDuration = 1.0f;

    private void Awake()
    {
        CacheInitialRotation(leftDoor);
    }

    private void CacheInitialRotation(DoorLeaf leaf)
    {
        if (leaf.doorTransform != null)
            leaf.doorInitialRot = leaf.doorTransform.localRotation;

        if (leaf.handleTransform != null)
            leaf.handleInitialRot = leaf.handleTransform.localRotation;
    }

    // ------------------ PUBLIC METHOD UNTUK DIHUBUNGKAN KE EVENT VR ------------------

    public void TriggerLeftDoor()
    {
        if (leftDoor.isAnimating) return;
        StartCoroutine(HandleDoorSequence(leftDoor));
    }

    // -----------------------------------------------------------------------------

    private IEnumerator HandleDoorSequence(DoorLeaf leaf)
    {
        leaf.isAnimating = true;

        // 1. Tekan handle menuju batas rotasi Z
        yield return StartCoroutine(RotateLocal(
            leaf.handleTransform,
            leaf.handleInitialRot,
            Quaternion.Euler(leaf.handleMaxX, 0f, 0f),
            handleAnimDuration));

        // 2. Buka / tutup daun pintu (rotasi Y)
        Quaternion targetDoorRot = leaf.isOpen
            ? leaf.doorInitialRot
            : Quaternion.Euler(0f, leaf.doorMaxY, 0f);

        yield return StartCoroutine(RotateLocal(
            leaf.doorTransform,
            leaf.doorTransform.localRotation,
            targetDoorRot,
            doorAnimDuration));

        leaf.isOpen = !leaf.isOpen;

        // 3. Lepas handle, kembali ke posisi awal
        yield return StartCoroutine(RotateLocal(
            leaf.handleTransform,
            leaf.handleTransform.localRotation,
            leaf.handleInitialRot,
            handleAnimDuration));

        leaf.isAnimating = false;
    }

    private IEnumerator RotateLocal(Transform target, Quaternion from, Quaternion to, float duration)
    {
        if (target == null) yield break;

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);
            target.localRotation = Quaternion.Slerp(from, to, t);
            yield return null;
        }
        target.localRotation = to;
    }

}
