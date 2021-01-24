using UnityEngine;

namespace LowEngine
{
    public class SoundPlayer : MonoBehaviour
    {
        public AudioClip soundToPlay;

        public void PlaySound()
        {
            AudioManager.PlayClip(soundToPlay, transform.position);
        }
    }
}