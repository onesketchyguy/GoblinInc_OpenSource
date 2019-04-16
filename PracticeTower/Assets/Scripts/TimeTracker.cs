using UnityEngine;
using UnityEngine.UI;

namespace LowEngine.TimeManagement
{
    public class TimeTracker : MonoBehaviour
    {
        void Update()
        {
            string minute = TimeScale.minutes > 9 ? $"{TimeScale.minutes}" : $"0{TimeScale.minutes}";

            string hour = TimeScale.hours > 9 ? $"{TimeScale.hours}" : $"0{TimeScale.hours}";

            GetComponent<Text>().text = $"Time - {hour}:{minute}";
        }
    }
}