using UnityEngine;

namespace LowEngine.TimeManagement
{
    public class DayCycle : MonoBehaviour
    {
        internal static float timeScale = 60;

        private void Start()
        {
            SetTime();
            TimeScale.HourChanged += SetTime;
        }

        float accum;

        void LateUpdate()
        {
            Tasks.TaskWorkerAI[] workers = FindObjectsOfType<Tasks.TaskWorkerAI>();

            int numberOfWorkersStillAtWork = workers.Length;

            foreach (var worker in workers)
            {
                if (worker.AtHome == true)
                {
                    numberOfWorkersStillAtWork -= 1;
                }
            }

            timeScale = (TimeScale.isDayTime() == false && numberOfWorkersStillAtWork < 1) ? 50 : (FindObjectsOfType<Tasks.TaskWorkerAI>().Length < 1) ? 0 : 5;

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

            if (TimeScale.hours == 8)
            {
                //Play awake sound
                AudioManager.instance.PlayOpenSound(transform.position);
            }

            if (TimeScale.hours == 17)
            {
                //Play closingtime sound
                AudioManager.instance.PlayClosingSound(transform.position);
            }

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