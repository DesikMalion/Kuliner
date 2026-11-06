using UnityEngine;
using UnityEngine.UI;

public class OpenControlMapping : MonoBehaviour
{
    [SerializeField] GameObject controlMappingPanel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Button tombolIkon = GetComponent<Button>();
        tombolIkon.onClick.AddListener(TriggerMenu);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void TriggerMenu()
    {
        if(controlMappingPanel != null)
            controlMappingPanel.SetActive(!controlMappingPanel.activeSelf);
    }
}
