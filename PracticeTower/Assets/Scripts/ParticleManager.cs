using UnityEngine;

namespace LowEngine
{
    public class ParticleManager : MonoBehaviour
    {
        public static ParticleManager instance;

        private void Awake()
        {
            instance = this;
        }

        public GameObject KillWorker;

        public GameObject StartEffect_FireWorker(Vector3 position)
        {
            AudioManager.instance.PlayFiredWorkerSound(position);

            GameObject toReturn = Instantiate(KillWorker, position, Quaternion.identity);

            toReturn.transform.localScale = Vector3.one * 0.5f;

            return toReturn;
        }
    }
}