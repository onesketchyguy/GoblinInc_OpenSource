using UnityEngine;

namespace LowEngine.TimeManagement
{
    public class DayCycle : MonoBehaviour
    {
        [Tooltip("Number of in game minutes per second.")] [SerializeField] public float timeScale = 60;

        private void Start()
        {
            TimeScale.days = PlayerPrefs.GetInt("CurrentDay");
            TimeScale.hours = PlayerPrefs.GetInt("CurrentHour");

            SetTime(TimeScale.hours);
        }

        void Update()
        {
            Rotate();

            TimeScale.SetTime();

            Vector3 myRot = transform.rotation.eulerAngles;

            TimeScale.minutes = Mathf.Abs((int)myRot.y - TimeScale.minutesPassed);
        }

        public void SetTime(int time)
        {
            int timeToSetTo = (time > 24) ? (time - 24) : time;

            if (time + TimeScale.hours > 24 || timeToSetTo < TimeScale.hours)
            {
                TimeScale.days += 1;
            }

            TimeScale.hours = timeToSetTo;
            TimeScale.minutes = 0;

            int rot = 15 * (time % 24) % 360;

            transform.eulerAngles = new Vector3(rot, 0, 0);
            TimeScale.minutesPassed = rot;
        }

        private void Rotate()
        {
            float angleThisFrame = (Time.deltaTime / 360) * timeScale;

            transform.RotateAround(transform.position, Vector3.right, angleThisFrame);
        }
    }

    public class TimeScale
    {
        public static int minutes = 0, hours = 12, days = 0;

        public static int minutesPassed = 0;

        public static void SetTime()
        {
            if (minutes >= 15)
            {
                hours += 1;
                minutesPassed += 15;
            }
            if (hours >= 24)
            {
                days += 1;
                hours = 0;
            }
        }

        public static bool isDayTime()
        {   //Time of day
            return (hours > 6 && hours < 17);
        }

        public TimeScale timeScale;
    }
}