using UnityEngine;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;

    public void Menu()
    {
        if(mainMenu.activeInHierarchy)
        {
            mainMenu.SetActive(false);
        }
        else
        {
            mainMenu.SetActive(true);
        }
    }
}
