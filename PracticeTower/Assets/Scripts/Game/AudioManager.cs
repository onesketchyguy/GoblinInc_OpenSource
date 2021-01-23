using System.Collections.Generic;
using UnityEngine;

namespace LowEngine
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;

        private Transform mainCamera;

        private void Awake()
        {
            instance = this;
            mainCamera = Camera.main.transform;
        }

        private void Start()
        {
            foreach (var item in SFX)
            {
                int index = 0;

                for (int i = item.name.Length - 1; i >= 0; i--)
                {
                    if (item.name[i] == ' ')
                    {
                        index = i;
                        break;
                    }
                }

                var key = item.name.Substring(0, index);

                if (sfxDictionary.ContainsKey(key))
                {
                    sfxDictionary[key] = Utilities.Add(item, sfxDictionary[key]);
                }
                else
                {
                    sfxDictionary.Add(key, new AudioClip[] { item });
                    Debug.Log($"Created sound effect key: \"{key}\"");
                }
            }
        }

        private static float lastSoundPlayed;

        /// <summary>
        /// Plays an Audioclip at a position.
        /// </summary>
        /// <param name="clip">Which clip to play.</param>
        /// <param name="position">Where the clip will originate.</param>
        /// <param name="volume">How loud to play the clip. (Whithin the range of 0 and 1.)</param>
        public static void PlayClip(AudioClip clip, Vector3 position, float volume = 1)
        {
            if (Time.time <= lastSoundPlayed) return;
            lastSoundPlayed = Time.time + Time.deltaTime;

            if (volume > 1 || volume < 0)
            {
                Debug.LogError($"Volume of {volume} is outside of volume range!(0, 1)");

                volume = 0.5f;
            }

            AudioSource.PlayClipAtPoint(clip, position, volume * PlayerPrefsManager.SFXVolume);
        }

        [SerializeField]
        private AudioClip[] SFX;

        private Dictionary<string, AudioClip[]> sfxDictionary = new Dictionary<string, AudioClip[]>();

        public void PlaySound(string name)
        {
            AudioClip[] clips;
            sfxDictionary.TryGetValue(name, out clips);

            if (clips != null)
            {
                int ran = Random.Range(0, clips.Length);
                PlayClip(clips[ran], mainCamera.position);
            }
        }

        public void PlaySound(string name, Vector3 position)
        {
            AudioClip[] clips;
            sfxDictionary.TryGetValue(name, out clips);

            if (clips != null)
            {
                int ran = Random.Range(0, clips.Length);
                PlayClip(clips[ran], position);
            }
            else
            {
                Debug.LogError($"No sound exists with a name of: {name}");
            }
        }

        public AudioClip[] onPressedSounds;

        public void PlayKeyClick(Vector3 fromPoint)
        {
            float distToCam = Mathf.Clamp(Vector3.Distance(Camera.main.transform.position, fromPoint) - Camera.main.orthographicSize, 0, 1f);

            AudioClip clip = onPressedSounds[Random.Range(0, onPressedSounds.Length)];

            PlayClip(clip, fromPoint, distToCam);
        }

        public AudioClip[] BuildSounds;

        public void PlayBuildSound(Vector3 fromPoint)
        {
            float distToCam = Mathf.Clamp(Vector3.Distance(Camera.main.transform.position, fromPoint) - Camera.main.orthographicSize, 0, 1f);

            AudioClip clip = BuildSounds[Random.Range(0, BuildSounds.Length)];

            PlayClip(clip, fromPoint, distToCam);
        }

        public AudioClip[] KillWorkerSounds;

        public void PlayFiredWorkerSound(Vector3 fromPoint)
        {
            float distToCam = Mathf.Clamp(Vector3.Distance(Camera.main.transform.position, fromPoint) - Camera.main.orthographicSize, 0, 1f);

            AudioClip clip = KillWorkerSounds[Random.Range(0, KillWorkerSounds.Length)];

            PlayClip(clip, fromPoint, distToCam);
        }

        public AudioClip[] OpeningSounds;

        public void PlayOpenSound(Vector3 fromPoint)
        {
            float distToCam = Mathf.Clamp(Vector3.Distance(Camera.main.transform.position, fromPoint) - Camera.main.orthographicSize, 0, 1f);

            AudioClip clip = OpeningSounds[Random.Range(0, OpeningSounds.Length)];

            PlayClip(clip, fromPoint, distToCam);
        }

        public AudioClip[] ClosingSounds;

        public void PlayClosingSound(Vector3 fromPoint)
        {
            float distToCam = Mathf.Clamp(Vector3.Distance(Camera.main.transform.position, fromPoint) - Camera.main.orthographicSize, 0, 1f);

            AudioClip clip = ClosingSounds[Random.Range(0, ClosingSounds.Length)];

            PlayClip(clip, fromPoint, distToCam);
        }
    }
}