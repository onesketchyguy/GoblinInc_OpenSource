using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

namespace LowEngine
{
    public class AudioManager : MonoBehaviour
    {
        public static AudioManager instance;

        public AudioMixerGroup sfxMixer;
        public int maxVoices = 20;
        private AudioSource[] audioSources;

        private Transform mainCamera;

        private AudioSource GetAudioSource()
        {
            foreach (var item in audioSources)
            {
                if (item.isPlaying) continue;
                return item;
            }

            // Could not find an audiosource
            // return first one instead
            return audioSources[0];
        }

        private void Awake()
        {
            instance = this;
            mainCamera = Camera.main.transform;
        }

        private void Start()
        {
            // Create the voices
            audioSources = new AudioSource[maxVoices];

            for (int i = 0; i < maxVoices; i++)
            {
                var go = new GameObject($"AudioSource_Voice: {i}");
                go.transform.SetParent(transform);

                audioSources[i] = go.AddComponent<AudioSource>();
                audioSources[i].outputAudioMixerGroup = sfxMixer;
                audioSources[i].playOnAwake = false;
                audioSources[i].loop = false;
                //audioSources[i].spatialBlend = 1;
            }

            // Create the sound effect keys
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

        private static float timeLastSoundPlayed;
        private static AudioClip lastClipPlayed;

        /// <summary>
        /// Plays an Audioclip at a position.
        /// </summary>
        /// <param name="clip">Which clip to play.</param>
        /// <param name="position">Where the clip will originate.</param>
        /// <param name="volume">How loud to play the clip. (Whithin the range of 0 and 1.)</param>
        public static void PlayClip(AudioClip clip, Vector3 position, float volume = 1)
        {
            if (Time.time <= timeLastSoundPlayed && clip == lastClipPlayed) return;
            timeLastSoundPlayed = Time.time + Time.deltaTime;
            lastClipPlayed = clip;

            if (volume > 1 || volume < 0)
            {
                Debug.LogError($"Volume of {volume} is outside of volume range!(0, 1)");

                volume = 0.5f;
            }

            var audioSource = instance.GetAudioSource();

            audioSource.clip = clip;
            audioSource.transform.position = position;
            audioSource.Play();
            audioSource.volume = volume;
            //AudioSource.PlayClipAtPoint(clip, position, volume);
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
            bool wasThisSound()
            {
                foreach (var item in OpeningSounds)
                {
                    if (item == lastClipPlayed) return true;
                }

                return false;
            }

            if (wasThisSound() && timeLastSoundPlayed < 10) return;

            float distToCam = Mathf.Clamp(Vector3.Distance(Camera.main.transform.position, fromPoint) - Camera.main.orthographicSize, 0, 1f);

            AudioClip clip = OpeningSounds[Random.Range(0, OpeningSounds.Length)];

            PlayClip(clip, fromPoint, distToCam);
        }

        public AudioClip[] ClosingSounds;

        public void PlayClosingSound(Vector3 fromPoint)
        {
            bool wasThisSound()
            {
                foreach (var item in ClosingSounds)
                {
                    if (item == lastClipPlayed) return true;
                }

                return false;
            }

            if (wasThisSound()) return;

            float distToCam = Mathf.Clamp(Vector3.Distance(Camera.main.transform.position, fromPoint) - Camera.main.orthographicSize, 0, 1f);

            AudioClip clip = ClosingSounds[Random.Range(0, ClosingSounds.Length)];

            PlayClip(clip, fromPoint, distToCam);
        }
    }
}