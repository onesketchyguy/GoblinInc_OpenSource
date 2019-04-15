using UnityEngine;

namespace LowEngine
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;

        private void Awake()
        {
            instance = this;
        }

        public AudioClip[] onPressedSounds;

        public void PlayKeyClick(Vector3 fromPoint)
        {
            float distToCam = Mathf.Clamp(Vector3.Distance(Camera.main.transform.position, fromPoint) - Camera.main.orthographicSize, 0, 1f);

            AudioSource.PlayClipAtPoint(onPressedSounds[Random.Range(0, onPressedSounds.Length)], transform.position, distToCam); //Replace wioth SFX volume
        }

        public AudioClip[] EatSounds;

        public void PlayEat(Vector3 fromPoint)
        {
            float distToCam = Mathf.Clamp(Vector3.Distance(Camera.main.transform.position, fromPoint) - Camera.main.orthographicSize, 0, 1f);

            AudioSource.PlayClipAtPoint(EatSounds[Random.Range(0, EatSounds.Length)], transform.position, distToCam); //Replace wioth SFX volume
        }

        public AudioClip[] DrinkSounds;

        public void PlayDrink(Vector3 fromPoint)
        {
            float distToCam = Mathf.Clamp(Vector3.Distance(Camera.main.transform.position, fromPoint) - Camera.main.orthographicSize, 0, 1f);

            AudioSource.PlayClipAtPoint(DrinkSounds[Random.Range(0, DrinkSounds.Length)], transform.position, distToCam); //Replace wioth SFX volume
        }

        public AudioClip[] BuildSounds;

        public void PlayBuildSound(Vector3 fromPoint)
        {
            float distToCam = Mathf.Clamp(Vector3.Distance(Camera.main.transform.position, fromPoint) - Camera.main.orthographicSize, 0, 1f);

            AudioSource.PlayClipAtPoint(BuildSounds[Random.Range(0, BuildSounds.Length)], transform.position, distToCam); //Replace wioth SFX volume
        }

        public AudioClip[] KillWorkerSounds;

        public void PlayFiredWorkerSound(Vector3 fromPoint)
        {
            float distToCam = Mathf.Clamp(Vector3.Distance(Camera.main.transform.position, fromPoint) - Camera.main.orthographicSize, 0, 1f);

            AudioSource.PlayClipAtPoint(KillWorkerSounds[Random.Range(0, KillWorkerSounds.Length)], transform.position, distToCam); //Replace wioth SFX volume
        }
    }
}