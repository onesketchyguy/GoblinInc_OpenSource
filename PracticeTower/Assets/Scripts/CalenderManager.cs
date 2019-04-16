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

                obj.checkBox.gameObject.SetActive(TimeScale.days > day);

                obj.PayDayDisplay.gameObject.SetActive(TimeScale.days + GameHandler.daysUntilPayDay == day);
            }
        }
    }
}