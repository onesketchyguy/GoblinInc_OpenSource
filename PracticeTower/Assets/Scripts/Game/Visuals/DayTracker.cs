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

        private void UpdateDay()
        {
            float currentDay = TimeScale.days;

            string DayName = "";

            switch (currentDay)
            {
                case 1:
                    DayName = "Mondarit";
                    break;

                case 2:
                    DayName = "Tuesdarit";
                    break;

                case 3:
                    DayName = "Wednesdarit";
                    break;

                case 4:
                    DayName = "Thursdarit";
                    break;

                case 5:
                    DayName = "Fridarit";
                    break;

                case 6:
                    DayName = "Saturdarit";
                    break;

                case 7:
                    DayName = "Sundarit";
                    break;
            }

            GetComponent<Text>().text = DayName;
        }
    }
}