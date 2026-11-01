using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.Interaction.Toolkit.Locomotion.Turning;

public class VRThumbstickScroll : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerMoveHandler
{
    [Header("Komponen UI")]
    public ScrollRect scrollRect;

    [Header("Pengaturan")]
    public float scrollSpeed = 2f;
    public float deadzone = 0.1f;
    public float hoverGrace = 0.3f;
    public bool debugLog = false;

    // Dicari otomatis tiap scene, tidak perlu di-drag
    readonly List<Behaviour> turnProviders = new List<Behaviour>();

    InputAction leftStick, rightStick;
    float lastHoverTime = -999f;
    bool isHovered;
    bool lastProviderState = true;

    void Awake()
    {
        if (scrollRect == null) scrollRect = GetComponent<ScrollRect>();

        leftStick = new InputAction("LeftStick", InputActionType.Value, expectedControlType: "Vector2");
        leftStick.AddBinding("<XRController>{LeftHand}/thumbstick");
        leftStick.AddBinding("<XRController>{LeftHand}/primary2DAxis");
        leftStick.AddBinding("<Gamepad>/leftStick");

        rightStick = new InputAction("RightStick", InputActionType.Value, expectedControlType: "Vector2");
        rightStick.AddBinding("<XRController>{RightHand}/thumbstick");
        rightStick.AddBinding("<XRController>{RightHand}/primary2DAxis");
        rightStick.AddBinding("<Gamepad>/rightStick");
    }

    void OnEnable()
    {
        leftStick.Enable();
        rightStick.Enable();
        SceneManager.sceneLoaded += OnSceneLoaded;
        FindTurnProviders();
    }

    void OnDisable()
    {
        leftStick.Disable();
        rightStick.Disable();
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SetTurn(true); // kembalikan turn saat panel ditutup
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(FindDelayed());
    }

    IEnumerator FindDelayed()
    {
        yield return null;
        yield return null; // tunggu XR Origin scene baru siap
        FindTurnProviders();
    }

    void FindTurnProviders()
    {
        turnProviders.Clear();

        // SnapTurnProvider & ContinuousTurnProvider = turunan dari base class turn
        foreach (var p in FindObjectsByType<SnapTurnProviderBase>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            turnProviders.Add(p);
        foreach (var p in FindObjectsByType<ContinuousTurnProviderBase>(FindObjectsInactive.Exclude, FindObjectsSortMode.None))
            turnProviders.Add(p);

        lastProviderState = true;
    }

    public void OnPointerEnter(PointerEventData e) { isHovered = true; lastHoverTime = Time.time; }
    public void OnPointerMove(PointerEventData e) { isHovered = true; lastHoverTime = Time.time; }
    public void OnPointerExit(PointerEventData e) { isHovered = false; lastHoverTime = Time.time; }

    void SetTurn(bool enabled)
    {
        foreach (var p in turnProviders)
            if (p != null && p.enabled != enabled) p.enabled = enabled;
    }

    void Update()
    {
        if (scrollRect == null) return;

        bool hoverOk = isHovered || (Time.time - lastHoverTime < hoverGrace);

        // hanya sentuh provider saat statusnya berubah (hemat, tidak tiap frame)
        if (hoverOk == lastProviderState)
        {
            SetTurn(!hoverOk);
            lastProviderState = !hoverOk;
        }

        float l = leftStick.ReadValue<Vector2>().x;
        float r = rightStick.ReadValue<Vector2>().x;
        float input = Mathf.Abs(l) > Mathf.Abs(r) ? l : r;

        if (debugLog && Mathf.Abs(input) > deadzone)
            Debug.Log($"[VRScroll] input={input:F2} hover={hoverOk} turnProviders={turnProviders.Count}");

        if (!hoverOk ||