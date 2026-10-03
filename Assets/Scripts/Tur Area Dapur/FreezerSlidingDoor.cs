using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

[RequireComponent(typeof(XRSimpleInteractable))]
public class FreezerSlidingDoor : MonoBehaviour
{
    public enum SumbuGeser { X, Y, Z }

    [Header("Pengaturan Rel Pintu")]
    public SumbuGeser sumbu = SumbuGeser.Z;

    [Tooltip("Kalau true: batas dihitung relatif dari posisi awal pintu. Kalau false: posisi lokal absolut.")]
    public bool relatifDariPosisiAwal = true;

    public float batasMin = 0f;
    public float batasMax = 1f;

    private XRSimpleInteractable interactable;
    private IXRSelectInteractor interactor;
    private Vector3 posisiAwalLokal;
    private float offsetGenggam;

    void Awake()
    {
        interactable = GetComponent<XRSimpleInteractable>();

        var rb = GetComponent<Rigidbody>();
        if (rb != null) rb.isKinematic = true;
    }

    void OnEnable()
    {
        interactable.selectEntered.AddListener(OnGrab);
        interactable.selectExited.AddListener(OnRelease);
    }

    void OnDisable()
    {
        interactable.selectEntered.RemoveListener(OnGrab);
        interactable.selectExited.RemoveListener(OnRelease);
    }

    void Start()
    {
        posisiAwalLokal = transform.localPosition;
    }

    int Idx => (int)sumbu;

    Vector3 KeLokalParent(Vector3 worldPos)
    {
        return transform.parent != null ? transform.parent.InverseTransformPoint(worldPos) : worldPos;
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        interactor = args.interactorObject;
        Vector3 tanganLokal = KeLokalParent(interactor.GetAttachTransform(interactable).position);
        offsetGenggam = transform.localPosition[Idx] - tanganLokal[Idx];
    }

    void OnRelease(SelectExitEventArgs args)
    {
        interactor = null;
    }

    void Update()
    {
        if (interactor == null) return;

        Vector3 tanganLokal = KeLokalParent(interactor.GetAttachTransform(interactable).position);
        float target = tanganLokal[Idx] + offsetGenggam;

        float min = relatifDariPosisiAwal ? posisiAwalLokal[Idx] + batasMin : batasMin;
        float max = relatifDariPosisiAwal ? posisiAwalLokal[Idx] + batasMax : batasMax;
        target = Mathf.Clamp(target, Mathf.Min(min, max), Mathf.Max(min, max));

        Vector3 pos = posisiAwalLokal;   // sumbu lain dikunci
        pos[Idx] = target;
        transform.localPosition = pos;
    }
}