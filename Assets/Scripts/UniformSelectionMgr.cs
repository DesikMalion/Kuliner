using UnityEngine;

public class UniformSelectionMgr : MonoBehaviour
{
    public GameObject NarasiAwal;
    public GameObject NarasiFinal;
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
        for (int i = 0; i < UniformNames.Length; i++)
        {
            if (name.ToLower().Contains(UniformNames[i].Name.ToLower()))
            {
                UniformNames[i].isSelected = true;
                break;
            }
        }
        for (int i = 0; i < UniformObjectsSelect.Length; i++)
        {
            if (UniformObjectsSelect[i].Name.ToLower() == name.ToLower())
            {
                UniformObjectsSelect[i].Obj.SetActive(false);
                break;
            }
        }
        for (int i = 0; i < UniformObjectsOnAvatar.Length; i++)
        {
            if (UniformObjectsOnAvatar[i].Name.ToLower() == name.ToLower())
            {
                UniformObjectsOnAvatar[i].Obj.SetActive(true);
                break;
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


}


//class untuk menampung GameObject dan nama objeknya
[System.Serializable]
public class UniformObject
{
    public GameObject Obj;
    public string Name;

}

[System.Serializable]
public class UniformNames
{
    public string Name;
    public bool isSelected;

}