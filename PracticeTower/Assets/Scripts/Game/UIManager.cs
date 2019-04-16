using UnityEngine;
using UnityEngine.UI;

namespace LowEngine
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager instance;

        private void Awake()
        {
            instance = this;
        }

        public enum Show { none, crafting, hiring, workerInfo, options, calender }

        public Show CurrentlyDisplaying;

        public GameObject CraftingMenu;

        public void ToggleCraftingMenu()
        {
            UpdateShowing(Show.crafting);
        }

        public GameObject HiringMenu;

        public void ToggleHiringMenu()
        {
            UpdateShowing(Show.hiring);
        }

        public GameObject OptionsPanel;

        public void ToggleOptionsPanel()
        {
            UpdateShowing(Show.options);
        }

        public GameObject CalenderPanel;

        public void ToggleCalenderPanel()
        {
            UpdateShowing(Show.calender);
        }

        public GameObject WorkerInfoPanel;

        public Text MoneyText;

        private void Update()
        {
            string Money = $"{GameHandler.instance.Money}";

            string display = GameHandler.MoneyToPayOnPayDay() == 0 ? Money : $"{GameHandler.instance.Money} - ${GameHandler.MoneyToPayOnPayDay()} on payday";

            MoneyText.text = $"$:{display}";
        }

        public void UpdateShowing(Show show)
        {
            if (CurrentlyDisplaying == show)
            {
                CurrentlyDisplaying = Show.none;
            }
            else
            {
                CurrentlyDisplaying = show;
            }

            CraftingMenu.SetActive(CurrentlyDisplaying == Show.crafting);
            HiringMenu.SetActive(CurrentlyDisplaying == Show.hiring);
            OptionsPanel.SetActive(CurrentlyDisplaying == Show.options);
            WorkerInfoPanel.SetActive(CurrentlyDisplaying == Show.workerInfo);
            CalenderPanel.SetActive(CurrentlyDisplaying == Show.calender);
        }
    }
}