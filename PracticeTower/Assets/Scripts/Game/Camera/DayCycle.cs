using UnityEngine;

namespace LowEngine.TimeManagement
{
    public class DayCycle : MonoBehaviour
    {
        private float timeScale = 60;

        private void Start()
        {
            SetTime();
            TimeScale.HourChanged += SetTime;
        }

        float accum;

        void LateUpdate()
        {
            timeScale = (TimeScale.isDayTime() == false && FindObjectsOfType<Tasks.Worker>().Length > 0) ? 20 : 1;

            TimeScale.SetTime();

            accum += timeScale * Time.deltaTime;

            if (accum >= 1)
            {
                TimeScale.minutes += 1;
                accum = 0;
            }
        }

        public void SetTime()
        {
            int time = TimeScale.hours - 12;

            int rot = (15 * (time % 24)) % 360;

            transform.eulerAngles = new Vector3(0, rot, 0);
        }
    }

    public class TimeScale
    {
        public static int minutes = 0, hours = 6, days = 1;

        public static TimeChangedEvent DayChanged;
        public static TimeChangedEvent HourChanged;

        public static void SetTime()
        {
            if (minutes >= 60)
            {
                hours += 1;
                minutes = 0;

                if (HourChanged != null)
                {
                    HourChanged.Invoke();
                }
            }
            if (hours > 24)
            {
                days += 1;
                hours = 1;

                if (DayChanged != null)
                {
                    DayChanged.Invoke();
                }
            }
        }

        public static bool isDayTime()
        {   //Time of day
            return (hours > 7 && hours < 17);
        }

        public delegate void TimeChangedEvent();
    }
}