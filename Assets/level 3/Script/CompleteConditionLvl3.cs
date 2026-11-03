using UnityEngine;
using UnityEngine.UI;

public class CompleteConditionLvl3 : MonoBehaviour
{
    public Button backIdentification;
    public Button backStorage;
    public Button button_oke;
    public bool isIdentificationDone;
    public bool isStorageDone;

    [Header("Panel Info")]
    public GameObject panel;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        isIdentificationDone = false;
        isStorageDone = false;

        backIdentification.onClick.AddListener(() => SettingBoolIdentification(true));
        backStorage.onClick.AddListener(() => SettingBoolStorage(true));
        button_oke.onClick.AddListener(ResetCondition);
    }

    // Update is called once per frame
    void Update()
    {
        if (isIdentificationDone && isStorageDone)
        {
            panel.SetActive(true);
        }
        else
        {

            panel.SetActive(false);
        }
    }


    public void SettingBoolIdentification(bool condition)
    {
        isIdentificationDone = condition;
    }

    public void SettingBoolStorage(bool condition)
    {
        isStorageDone = condition;

    }

    public void ResetCondition()
    {
        isIdentificationDone = false;
        isStorageDone = false;
    }
}
