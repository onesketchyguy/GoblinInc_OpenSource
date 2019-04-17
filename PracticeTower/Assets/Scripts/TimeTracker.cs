using UnityEngine;
using UnityEngine.UI;

namespace LowEngine.TimeManagement
{
    public class TimeTracker : MonoBehaviour
    {
        public Toggle hourToggle;

        bool amClock;

        private void Start()
        {
            hourToggle.onValueChanged.AddListener(SetClockType);
        }

        public void SetClockType(bool type)
        {
            amClock = type;
        }

        void Update()
        {
            string minute = TimeScale.minutes > 9 ? $"{TimeScale.minutes}" : $"0{TimeScale.minutes}";

            int _hour = amClock ? TimeScale.hours % 12 : TimeScale.hours;

            string hour = TimeScale.hours > 9 ? $"{_hour}" : $"0{_hour}";

            string extension = (amClock) ? (TimeScale.hours > 12 ? ".PM" : ".AM") : "";

            GetComponent<Text>().text = $"Time - {hour}:{minute}{extension}";
        }
    }
}