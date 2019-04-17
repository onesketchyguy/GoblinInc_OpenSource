﻿using UnityEngine;
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
        public Text DebtText;

        private void Update()
        {
            if (GameHandler.MoneyToPayOnPayDay() > 0) DebtText.text = $"$:{GameHandler.MoneyToPayOnPayDay()} due on payday"; else DebtText.text = "";

            MoneyText.text = $"$:{GameHandler.instance.Money}";
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