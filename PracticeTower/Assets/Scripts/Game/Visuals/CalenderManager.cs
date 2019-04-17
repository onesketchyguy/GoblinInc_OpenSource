using UnityEngine;

namespace LowEngine.TimeManagement
{
    public class CalenderManager : MonoBehaviour
    {
        public DayDisplay[] Days;

        private void Start()
        {
            TimeScale.DayChanged += UpdateDays;
        }

        private void OnEnable()
        {
            UpdateDays();
        }

        public void UpdateDays()
        {
            for (int i = 0; i < Days.Length; i++)
            {
                DayDisplay obj = Days[i];

                int day = 0;

                int.TryParse(obj.Day.text, out day);

                obj.Day.text = $"{day}";

                int CurrentDay = TimeScale.days % 7;

                obj.checkBox.gameObject.SetActive(CurrentDay > day);

                obj.PayDayDisplay.gameObject.SetActive(CurrentDay + GameHandler.daysUntilPayDay == day);
            }
        }
    }
}