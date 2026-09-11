using UnityEngine;
using UnityEngine.InputSystem;

public class UIToggleManager : MonoBehaviour
{
    [Header("Panel menu")]
    public GameObject uiPanel;

    [Header("Pilih Tombol Controller")]
    public InputActionReference toggleAction;

    private void OnEnable()
    {
        toggleAction.action.Enable();
        toggleAction.action.performed += TogglePanel;
    }
    private void OnDisable()
    {
        toggleAction.action.performed -= TogglePanel;
        toggleAction.action.Disable();
    }
    private void TogglePanel(InputAction.CallbackContext context)
    {
        if (uiPanel != null)
        {
            uiPanel.SetActive(!uiPanel.activeSelf);
        }
    }
}
