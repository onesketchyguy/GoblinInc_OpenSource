using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace LowEngine
{
    public class UIManager : MonoBehaviour
    {
        public static UIManager instance;

        public Text MoneyText;
        public Text DebtText;

        public UnityEvent onHideMenus;

        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            HideMenus();
        }

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

                HideMenus();
            }

            if (GameHandler.MoneyToPayOnPayDay() > 0) DebtText.text = $"{GameHandler.MoneyToPayOnPayDay()} due on payday"; else DebtText.text = "";

            MoneyText.text = $"{GameHandler.instance.Money}";
        }

        public void HideMenus()
        {
            onHideMenus?.Invoke();
        }
    }
}