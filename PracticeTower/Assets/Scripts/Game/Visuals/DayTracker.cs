using UnityEngine;
using UnityEngine.UI;

namespace LowEngine.TimeManagement
{
    public class DayTracker : MonoBehaviour
    {
        private void Start()
        {
            UpdateDay();

            TimeScale.DayChanged += UpdateDay;
        }

        void UpdateDay()
        {
            float currentDay = TimeScale.days;

            string DayName = "";

            switch (currentDay)
            {
                case 1:
                    DayName = "Monday";
                    break;
                case 2:
                    DayName = "Tuesday";
                    break;
                case 3:
                    DayName = "Wednesday";
                    break;
                case 4:
                    DayName = "Thursday";
                    break;
                case 5:
                    DayName = "Friday";
                    break;
                case 6:
                    DayName = "Saturday";
                    break;
                case 7:
                    DayName = "Sunday";
                    break;
            }

            GetComponent<Text>().text = DayName;
        }
    }
}