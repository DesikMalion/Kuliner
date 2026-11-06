using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PenghubungIkonMenu : MonoBehaviour
{
    private void Start()
    {
        // Secara otomatis mengambil komponen Button dari Ikon ini
        Button tombolIkon = GetComponent<Button>();

        // Menambahkan perintah klik lewat script (menggantikan fungsi drag di Inspector)
        tombolIkon.onClick.AddListener(TriggerMenu);
    }

    private void TriggerMenu()
    {
        // Cari MainMenuManager yang bersembunyi di memori DontDestroyOnLoad
        MainMenuManager manager = FindObjectOfType<MainMenuManager>();

        if (manager != null)
        {
            manager.ToggleMenu();
        }
    }
}