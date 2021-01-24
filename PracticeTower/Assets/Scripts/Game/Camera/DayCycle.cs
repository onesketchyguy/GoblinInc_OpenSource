using UnityEngine;

namespace LowEngine.TimeManagement
{
    public class DayCycle : MonoBehaviour
    {
        internal static float timeScale = 60;

        public UnityEngine.UI.Slider timeScaleSlider;

        private byte playedSound = 0;

        private void Start()
        {
            SetTime();
            TimeScale.HourChanged += SetTime;

            timeScaleSlider.onValueChanged.AddListener((float value) =>
            {
                // Evaluate time
                daySpeed = Mathf.RoundToInt(value);
                SetTime();
            });
        }

        private float accum;

        private int daySpeed = 1;

        private void LateUpdate()
        {
            playedSound--;

            Tasks.TaskWorkerAI[] workers = FindObjectsOfType<Tasks.TaskWorkerAI>();

            int numberOfWorkersStillAtWork = workers.Length;

            foreach (var worker in workers)
            {
                if (worker.AtHome == true)
                {
                    numberOfWorkersStillAtWork -= 1;
                }
            }

            if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad0))
            {
                daySpeed = 0;
                timeScaleSlider.value = daySpeed;
            }
            else
            if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad1))
            {
                daySpeed = 1;
                timeScaleSlider.value = daySpeed;
            }
            else
            if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad2))
            {
                daySpeed = 5;
                timeScaleSlider.value = daySpeed;
            }
            else
            if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad3))
            {
                daySpeed = 7;
                timeScaleSlider.value = daySpeed;
            }
            else
            if (Input.GetKeyDown(KeyCode.Alpha5) || Input.GetKeyDown(KeyCode.Keypad4))
            {
                daySpeed = 10;
                timeScaleSlider.value = daySpeed;
            }

            timeScale = (TimeScale.isDayTime() == false && numberOfWorkersStillAtWork < 1) ? 50 : (FindObjectsOfType<Tasks.TaskWorkerAI>().Length < 1) ? 0 : daySpeed;

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
            else
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
        private static int lastDay = 0;
        private static int lastHour = 0;

        public static TimeChangedEvent DayChanged;
        public static TimeChangedEvent HourChanged;

        public static void SetTime()
        {
            if (minutes >= 60)
            {
                hours += 1;
                minutes = 0;

                if (lastHour != hours)
                {
                    HourChanged?.Invoke();
                    lastHour = hours;
                }
            }
            if (hours > 24)
            {
                days += 1;
                hours = 1;

                if (days > 7)
                {
                    days = 1;
                }

                if (lastDay != days)
                {
                    DayChanged?.Invoke();
                    lastDay = days;
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