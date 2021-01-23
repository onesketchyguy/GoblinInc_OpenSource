using UnityEngine;
using UnityEngine.EventSystems;
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

        public enum Show { none, crafting, hiring, workerInfo, options, contracts }

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

        public void ToggleObject(GameObject @object)
        {
            @object.SetActive(!@object.activeSelf);
        }

        public GameObject ContractsPanel;

        public void ToggleContractsPanel()
        {
            UpdateShowing(Show.contracts);
        }

        public GameObject WorkerInfoPanel;

        public Text MoneyText;
        public Text DebtText;

        private void Update()
        {
            if (EventSystem.current.IsPointerOverGameObject() == false && Input.GetButtonDown("Fire2"))
            {
                if (ObjectPlacingManager.Spawning != null || ObjectPlacingManager.bullDozing)
                {
                    FindObjectOfType<ObjectPlacingManager>().ClearObject();

                    CursorManager.instance.UpdateCursor();

                    return;
                }

                Cursor.visible = true;

                CursorManager.instance.UpdateCursor();

                UpdateShowing(Show.none);
            }

            if (GameHandler.MoneyToPayOnPayDay() > 0) DebtText.text = $"{GameHandler.MoneyToPayOnPayDay()} due on payday"; else DebtText.text = "";

            MoneyText.text = $"{GameHandler.instance.Money}";
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
            ContractsPanel.SetActive(CurrentlyDisplaying == Show.contracts);
        }
    }
}