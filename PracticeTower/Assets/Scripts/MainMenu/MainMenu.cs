using UnityEngine;

public class MainMenu : MonoBehaviour
{
    #region Menus
    public enum Menus { Title, Options, Quit }

    private Menus activeMenu = Menus.Title;

    public GameObject[] menus;

    public LoadingScreen loadingScreen;

    private void Start()
    {
        SwapMenu(0);

        loadingScreen.gameObject.SetActive(false);
    }

    public void SwapMenu(int menuToSwitchTo)
    {
        activeMenu = (Menus)menuToSwitchTo;

        for (int i = 0; i < menus.Length; i++)
        {
            var menu = menus[i];

            menu.SetActive(i == (int)activeMenu);
        }
    }
    #endregion

    public void QuitGame()
    {
        Application.Quit();
    }

    public void LoadScene(string sceneToLoad)
    {
        loadingScreen.gameObject.SetActive(true);

        loadingScreen.StartLoad(sceneToLoad);
    }
}