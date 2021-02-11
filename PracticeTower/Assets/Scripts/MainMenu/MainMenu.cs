using UnityEngine;

public class MainMenu : MonoBehaviour
{
    #region Menus

    public enum Menus { Title, Options, Quit }

    private Menus activeMenu = Menus.Title;

    public GameObject[] menus;

    private void Start()
    {
        SwapMenu(0);
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

    #endregion Menus

    public void QuitGame()
    {
        Application.Quit();
    }
}