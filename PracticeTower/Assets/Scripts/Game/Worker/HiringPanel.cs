using LowEngine.Saving;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LowEngine
{
    public class HiringPanel : MonoBehaviour
    {
        int Hirable = 3;

        public Transform spawningParent;

        SaveManager.SavableObject.Worker selectedEmployee;

        List<SaveManager.SavableObject.Worker> todaysEmployees = new List<SaveManager.SavableObject.Worker>() { };

        public Text selectedEmployeeBio;

        public void UpdateApplicants()
        {
            //Remove old employees
            if (todaysEmployees.Count > 0)
            {
                List<SaveManager.SavableObject.Worker> applicantsToRemove = new List<SaveManager.SavableObject.Worker>() { };

                int ran = Random.Range(0, todaysEmployees.Count);

                for (int i = 0; i < ran; i++)
                {
                    if (todaysEmployees[i] != null)
                    {
                        applicantsToRemove.Add(todaysEmployees[i]);
                    }
                }


                while (applicantsToRemove.Count > 0)
                {
                    todaysEmployees.Remove(todaysEmployees[0]);
                    applicantsToRemove.Remove(applicantsToRemove[0]);
                }
            }

            Hirable = Random.Range(1, 7);
        }

        private void Start()
        {
            TimeManagement.TimeScale.DayChanged += UpdateApplicants;
        }

        private void OnEnable()
        {
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            Display("Select an employee...");

            foreach (var item in spawningParent.GetComponentsInChildren<Transform>())
            {
                if (item.transform == spawningParent.transform) continue;

                Destroy(item.gameObject);
            }

            if (todaysEmployees.Count > 0)
            {
                foreach (var worker in todaysEmployees)
                {
                    SpawnButton(worker);
                }
            }
            else
            {
                for (int i = 0; i < Hirable; i++)
                {
                    SpawnButton(null);
                }

                Hirable = 0;
            }

            if (todaysEmployees.Count == 0)
            {
                Display("There are no more applications today.");
            }
        }

        void SpawnButton(SaveManager.SavableObject.Worker worker)
        {
            Sprite[] images;

            if (worker == null)
            {
                images = WorkerSpriteGenerator.instance.GetWorkerSprites(out worker);

                todaysEmployees.Add(worker);
            }
            else
            {
                images = WorkerSpriteGenerator.instance.GetWorkerSprites(worker);
            }

            GameObject spawnable = new GameObject(worker.name); //Create a temporary object to spawn
            spawnable.AddComponent<Image>();

            GameObject go = Instantiate(spawnable, spawningParent);
            Button button = go.AddComponent<Button>();
            GameObject[] children = new GameObject[images.Length];

            for (int i = 0; i < images.Length; i++)
            {
                var child = children[i];

                if (i == 0)
                {
                    child = go;

                    child.GetComponent<Image>().raycastTarget = true;

                    button.targetGraphic = child.GetComponent<Image>();
                }
                else
                {
                    child = Instantiate(spawnable, go.transform);
                    child.GetComponent<RectTransform>().localScale = go.GetComponent<RectTransform>().localScale/2;

                    child.GetComponent<Image>().raycastTarget = false;
                }

                child.name = $"{worker.name}.Layer{i}";


                child.GetComponent<Image>().sprite = images[i];

                if (images[i] == null)
                {
                    Destroy(child.gameObject);

                    continue;
                }

                if (i == 4) //Hair
                {
                    child.GetComponent<Image>().color = worker.color;
                }

                child.transform.SetSiblingIndex(i);
            }

            button.onClick.AddListener(delegate ()
            {
                float eff = worker.skill;

                Display($"{worker.name}\nEfficiency:{eff}%\nWill work for: ${worker.pay}(Due on payday)");
                selectedEmployee = worker;
            });

            Destroy(spawnable); //Clean up temp object
        }

        void Display(string showText)
        {
            selectedEmployeeBio.text = showText;
        }

        public void Hire()
        {
            if (selectedEmployee == null) return;

            Constructor.GetWorker(selectedEmployee);

            todaysEmployees.Remove(selectedEmployee);

            UpdateButtons();
        }
    }
}