using ITISKIRUHERE;
using UnityEngine;

public class UniformSelectionMgr : MonoBehaviour
{
    public GameObject NarasiAwal;
    public GameObject NarasiFinal;
    public GameObject DefaultHandL;
    public GameObject DefaultHandR;
    public UniformNames[] UniformNames;
    public UniformObject[] UniformObjectsOnAvatar;
    public UniformObject[] UniformObjectsSelect;

    bool canSelectUniform = false;

    void Start()
    {
        NarasiAwal.SetActive(true);
        NarasiFinal.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HideNarasiAwal() { 
        NarasiAwal.SetActive(false);
        canSelectUniform = true;
    }

    public void SetUniform(string name) { //ex: gloveR1
        if (!canSelectUniform) return;
        string subname = "";
        
        for (int i = 0; i < UniformNames.Length; i++)
        {
            if (name.ToLower().Contains(UniformNames[i].Name.ToLower()))
            {
                //UniformNames[i].isSelected = true;
                subname = UniformNames[i].Name; //ex: gloveR
                break;
            }
        }

        if (subname == "gloveR") {
            DefaultHandR.SetActive(false);
        }else if (subname == "gloveL") {
            DefaultHandL.SetActive(false);
        }

        for (int i = 0; i < UniformObjectsSelect.Length; i++)
        {
            if (UniformObjectsSelect[i].Name.Contains(subname))
            {
                UniformObjectsSelect[i].Obj.SetActive(true);
                OutlineDisabler(UniformObjectsSelect[i].Obj);


            }

            if (UniformObjectsSelect[i].Name.ToLower() == name.ToLower())
            {
                OutlineDisabler(UniformObjectsSelect[i].Obj);
                UniformObjectsSelect[i].Obj.SetActive(false);
                //break;

                for (int j = 0; j < UniformNames.Length; j++)
                {
                    if (UniformNames[j].Name == subname)
                    {
                        if (UniformObjectsSelect[i].isTrue)
                        {
                            UniformNames[j].isSelected = true;

                        }
                        else
                        {

                            UniformNames[j].isSelected = false;

                        }
                    }

                }
            }
        }
        for (int i = 0; i < UniformObjectsOnAvatar.Length; i++)
        {
            if (UniformObjectsOnAvatar[i].Name.Contains(subname))
            {
                UniformObjectsOnAvatar[i].Obj.SetActive(false);
            }

            if (UniformObjectsOnAvatar[i].Name.ToLower() == name.ToLower())
            {
                UniformObjectsOnAvatar[i].Obj.SetActive(true);
                //break;
            }
        }

        bool isAllSelected = true;

        for (int i = 0; i < UniformNames.Length; i++)
        {
            if (!UniformNames[i].isSelected)
            {
                isAllSelected = false;
                break;
            }
        }

        if (isAllSelected)
        {
            NarasiFinal.SetActive(true);
        }

    }

    bool isLoadscene = false;
    public void LoadScene(string name) { 
        if (isLoadscene) return;
        isLoadscene = true;
        UnityEngine.SceneManagement.SceneManager.LoadScene(name);

    }

    public void OutlineEnabler(GameObject obj)
    {

        AdvancedOutline advancedOutline = obj.GetComponent<AdvancedOutline>();

        if (advancedOutline == null)
        {
            advancedOutline = obj.GetComponentInChildren<AdvancedOutline>();
        }
        if (advancedOutline == null)
        {
            //Debug.LogError("AdvancedOutline component not found on the object or its children.");
            return;
        }

            advancedOutline.PulseWidth = true;
            advancedOutline.OutlineMode = AdvancedOutline.Mode.OutlineVisible;
            advancedOutline.OutlineWidth = 10;


    }

    public void OutlineDisabler(GameObject obj)
    {

        AdvancedOutline advancedOutline = obj.GetComponent<AdvancedOutline>();

        if (advancedOutline == null)
        {
            advancedOutline = obj.GetComponentInChildren<AdvancedOutline>();
        }
        if (advancedOutline == null)
        {
            //Debug.LogError("AdvancedOutline component not found on the object or its children.");
            return;
        }

        advancedOutline.PulseWidth = false;
        advancedOutline.OutlineMode = AdvancedOutline.Mode.OutlineHidden;
        advancedOutline.OutlineWidth = 0;


    }

}


//class untuk menampung GameObject dan nama objeknya
[System.Serializable]
public class UniformObject
{
    public GameObject Obj;
    public string Name;
    public bool isTrue;
}

[System.Serializable]
public class UniformNames
{
    public string Name;
    public bool isSelected;
    

}