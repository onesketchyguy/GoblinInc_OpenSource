using UnityEngine;
using LowEngine.Saving;

namespace LowEngine
{
    public class WorkerSpriteGenerator : MonoBehaviour
    {
        public static WorkerSpriteGenerator instance;

        private void Awake()
        {
            instance = this;
        }

        public Sprite[] Heads;
        public Sprite[] Eyes;
        public Sprite[] Noses;
        public Sprite[] Mouths;
        public Sprite[] Hairs;

        public Sprite[] Bodies;

        public GameObject GetWorker(string name, out SaveManager.SavableObject.Worker worker)
        {
            GameObject toReturn = new GameObject($"{name}.Base");

            int headIndex;
            GameObject head = CreateNewPart($"{name}.head", Heads, 1, out headIndex);
            head.transform.SetParent(toReturn.transform);

            int eyeIndex;
            GameObject eyes = CreateNewPart($"{name}.eyes", Eyes, 2, out eyeIndex);
            eyes.transform.SetParent(toReturn.transform);

            int noseIndex;
            GameObject nose = CreateNewPart($"{name}.nose", Noses, 3, out noseIndex);
            nose.transform.SetParent(toReturn.transform);

            int mouthIndex;
            GameObject mouth = CreateNewPart($"{name}.mouth", Mouths, 2, out mouthIndex);
            mouth.transform.SetParent(toReturn.transform);

            int bodyIndex;
            GameObject body = CreateNewPart($"{name}.body", Bodies, 0, out bodyIndex);
            body.transform.SetParent(toReturn.transform);

            float hasHairLoss = Random.Range(0, 1f);

            int hairIndex = -1;
            Color hairColor = Color.black;
            if (hasHairLoss >= 0.85f) // Taken from a real statistic!
            {
                GameObject hair = CreateNewPart($"{name}.hair", Hairs, 3, out hairIndex);
                hair.transform.SetParent(toReturn.transform);

                hairColor = hair.GetComponent<SpriteRenderer>().color;
            }

            int day = TimeManagement.TimeScale.days % 7;

            float skill = Random.Range(30 * day, 50 * day) % 70;

            float income = Random.Range(5, 500);

            income = (float)System.Math.Round(income, 2);

            worker = new SaveManager.SavableObject.Worker(name, headIndex, eyeIndex, noseIndex, mouthIndex, hairIndex, hairColor, skill, income, income + skill, 0, new float[] { 100, 100 });

            return toReturn;
        }

        public GameObject GetWorker(SaveManager.SavableObject.Worker worker)
        {
            string name = worker.name;

            GameObject toReturn = new GameObject($"{name}.Base");

            GameObject head = CreateNewPart($"{name}.head", Heads, worker.headIndex, 1);
            head.transform.SetParent(toReturn.transform);

            GameObject eyes = CreateNewPart($"{name}.eyes", Eyes, worker.eyeIndex, 2);
            eyes.transform.SetParent(toReturn.transform);

            GameObject nose = CreateNewPart($"{name}.nose", Noses, worker.noseIndex, 3);
            nose.transform.SetParent(toReturn.transform);

            GameObject mouth = CreateNewPart($"{name}.mouth", Mouths, worker.mouthIndex, 2);
            mouth.transform.SetParent(toReturn.transform);

            if (worker.hairIndex > -1)
            {
                GameObject hair = CreateNewPart($"{name}.hair", Hairs, worker.hairIndex, 3);
                hair.transform.SetParent(toReturn.transform);
                hair.GetComponent<SpriteRenderer>().color = worker.color;
            }

            return toReturn;
        }

        public Sprite[] GetWorkerSprites(out SaveManager.SavableObject.Worker worker)
        {
            Sprite[] toReturn = new Sprite[5];

            string name = DialogueSys.GetName();

            int headIndex;
            GameObject head = CreateNewPart($"{name}.head", Heads, 1, out headIndex);
            toReturn[0] = head.GetComponent<SpriteRenderer>().sprite;

            Destroy(head);

            int eyeIndex;
            GameObject eyes = CreateNewPart($"{name}.eyes", Eyes, 2, out eyeIndex);
            toReturn[1] = eyes.GetComponent<SpriteRenderer>().sprite;

            Destroy(eyes);

            int noseIndex;
            GameObject nose = CreateNewPart($"{name}.nose", Noses, 3, out noseIndex);
            toReturn[2] = nose.GetComponent<SpriteRenderer>().sprite;

            Destroy(nose);

            int mouthIndex;
            GameObject mouth = CreateNewPart($"{name}.mouth", Mouths, 2, out mouthIndex);
            toReturn[3] = mouth.GetComponent<SpriteRenderer>().sprite;

            Destroy(mouth);

            bool hasHairLoss = Random.Range(0, 1f) > 0.85f;// Taken from a real statistic!

            int hairIndex = -1;
            Color hairColor = Color.black;
            if (hasHairLoss)
            {
                GameObject hair = CreateNewPart($"{name}.hair", Hairs, 3, out hairIndex);
                toReturn[4] = hair.GetComponent<SpriteRenderer>().sprite;

                hairColor = hair.GetComponent<SpriteRenderer>().color;

                Destroy(hair);
            }

            int day = TimeManagement.TimeScale.days % 7;

            float skill = Random.Range(30 * day, 50 * day) % 70;

            float income = Random.Range(5f, 500);

            income = (float)System.Math.Round(income, 2);

            worker = new SaveManager.SavableObject.Worker(name, headIndex, eyeIndex, noseIndex, mouthIndex, hairIndex, hairColor, skill, income, income + skill, 0, new float[] { 100, 100 });

            return toReturn;
        }

        public Sprite[] GetWorkerSprites(SaveManager.SavableObject.Worker worker)
        {
            Sprite[] toReturn = new Sprite[5];

            GameObject head = CreateNewPart($"{name}.head", Heads, worker.headIndex, 1);
            toReturn[0] = head.GetComponent<SpriteRenderer>().sprite;
            Destroy(head);

            GameObject eyes = CreateNewPart($"{name}.eyes", Eyes, worker.eyeIndex, 2);
            toReturn[1] = eyes.GetComponent<SpriteRenderer>().sprite;
            Destroy(eyes);

            GameObject nose = CreateNewPart($"{name}.nose", Noses, worker.noseIndex, 3);
            toReturn[2] = nose.GetComponent<SpriteRenderer>().sprite;
            Destroy(nose);

            GameObject mouth = CreateNewPart($"{name}.mouth", Mouths, worker.mouthIndex, 2);
            toReturn[3] = mouth.GetComponent<SpriteRenderer>().sprite;
            Destroy(mouth);

            if (worker.hairIndex > -1)
            {
                GameObject hair = CreateNewPart($"{name}.hair", Hairs, worker.hairIndex, 3);
                hair.GetComponent<SpriteRenderer>().color = worker.color;
                toReturn[4] = hair.GetComponent<SpriteRenderer>().sprite;
                Destroy(hair);
            }

            return toReturn;
        }

        private GameObject CreateNewPart(string name, Sprite[] sprites, int order, out int val)
        {
            GameObject head = new GameObject($"{name}.head");

            SpriteRenderer spr = head.AddComponent<SpriteRenderer>();
            spr.sprite = GetSpriteFromArray(sprites, out val);
            spr.sortingOrder = order;

            if (sprites == Hairs)
            {
                spr.color = Utilities.GetRandomColor();
            }

            return head;
        }

        private GameObject CreateNewPart(string name, Sprite[] sprites, int index, int order)
        {
            GameObject part = new GameObject($"{name}.head");

            SpriteRenderer spr = part.AddComponent<SpriteRenderer>();
            spr.sprite = sprites[index];
            spr.sortingOrder = order;

            if (sprites == Hairs)
            {
                spr.color = Utilities.GetRandomColor();
            }

            return part;
        }

        private Sprite GetSpriteFromArray(Sprite[] sprites, out int val)
        {
            val = Random.Range(0, sprites.Length);

            return sprites[val];
        }
    }
}