using UnityEngine;

/**
 * main driver for the main menu
 */
public class MainMenu : MonoBehaviour
{
    public GameObject titleUI;
    public GameObject mainMenuUI;
    public GameObject settingsUI;

    private void Start()
    {
        OpenMainMenu();
    }

    public void OpenSettings()
    {
        mainMenuUI.SetActive(false);
        settingsUI.SetActive(true);
    }

    public void OpenMainMenu()
    {
        settingsUI.SetActive(false);
        mainMenuUI.SetActive(true);
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
