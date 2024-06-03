using LowEngine.Saving;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LowEngine
{
    public class HiringPanel : MonoBehaviour
    {
        public Toggle ToggleHiring;
        private bool hiring = true;

        public Text ContextButtonText;
        public Button contextButton;

        [Header("Hired")]
        public GameObject hiredRegion;

        public static List<SaveManager.SavableObject.Worker> hiredEmployees = new List<SaveManager.SavableObject.Worker>() { };

        public Transform HiredSpawningParent;

        public Sprite FireIcon;

        [Header("Hiring")]
        public GameObject hiringRegion;

        private int Hirable = 3;
        public Transform HiringSpawningParent;

        public Sprite HireIcon;

        private List<SaveManager.SavableObject.Worker> todaysEmployees = new List<SaveManager.SavableObject.Worker>() { };

        private SaveManager.SavableObject.Worker selectedEmployee;

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
            ToggleHiring.onValueChanged.AddListener(ToggleHiringValue);
            TimeManagement.TimeScale.DayChanged += UpdateApplicants;
        }

        public void ToggleHiringValue(bool newVal)
        {
            hiring = newVal;

            UpdateView();
        }

        private void OnEnable()
        {
            UpdateView();
        }

        private void UpdateView()
        {
            ContextButtonText.text = (hiring) ? "Hire worker" : "Fire worker";
            contextButton.image.sprite = (hiring) ? HireIcon : FireIcon;

            contextButton.onClick.RemoveAllListeners();

            hiringRegion.SetActive(hiring);
            hiredRegion.SetActive(!hiring);

            if (hiring)
            {
                contextButton.onClick.AddListener(() => Hire());
                UpdateHireButtons();
            }
            else
            {
                contextButton.onClick.AddListener(() => Fire());
                UpdateHiredButtons();
            }
        }

        private void UpdateHireButtons()
        {
            Display("Select an employee...");

            if (Hirable > 0)
            {
                for (int i = 0; i < Hirable; i++)
                {
                    SpawnHireButton(null);
                }

                Hirable = 0;
            }

            DestroyChildren(HiringSpawningParent);
            DestroyChildren(HiredSpawningParent);

            if (todaysEmployees.Count > 0)
            {
                foreach (var worker in todaysEmployees)
                {
                    SpawnHireButton(worker);
                }
            }

            if (todaysEmployees.Count == 0)
            {
                Display("There are no more applications today.");
            }
        }

        private void UpdateHiredButtons()
        {
            Display("Select an employee...");

            hiredEmployees = new List<SaveManager.SavableObject.Worker>() { };

            Tasks.Worker[] workers = FindObjectsOfType<Tasks.Worker>();

            for (int i = 0; i < workers.Length; i++)
            {
                hiredEmployees.Add(workers[i].workerData);
            }

            DestroyChildren(HiringSpawningParent);
            DestroyChildren(HiredSpawningParent);

            if (hiredEmployees.Count > 0)
            {
                foreach (var worker in hiredEmployees)
                {
                    SpawnHiredButton(worker);
                }
            }

            if (hiredEmployees.Count == 0)
            {
                Display("You don't have any employees.");
            }
        }

        private void DestroyChildren(Transform parent)
        {
            foreach (var item in parent.GetComponentsInChildren<Transform>())
            {
                if (item.transform == parent) continue;

                Destroy(item.gameObject);
            }
        }

        private void SpawnHireButton(SaveManager.SavableObject.Worker worker)
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

            if (worker.skill < 20)
            {
                return;
            }

            GameObject spawnable = new GameObject(worker.name); //Create a temporary object to spawn
            spawnable.AddComponent<Image>();

            GameObject go = Instantiate(spawnable, HiringSpawningParent);
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
                    child.GetComponent<RectTransform>().localScale = go.GetComponent<RectTransform>().localScale / 2;

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

        private void SpawnHiredButton(SaveManager.SavableObject.Worker worker)
        {
            Sprite[] images = WorkerSpriteGenerator.instance.GetWorkerSprites(worker);

            GameObject spawnable = new GameObject(worker.name); //Create a temporary object to spawn
            spawnable.AddComponent<Image>();

            GameObject go = Instantiate(spawnable, HiredSpawningParent);
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
                    child.GetComponent<RectTransform>().localScale = go.GetComponent<RectTransform>().localScale / 2;

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

                Display($"{worker.name}\nEfficiency:{eff}%\nWorks for: ${worker.pay}(Due on payday)");
                selectedEmployee = worker;
            });

            Destroy(spawnable); //Clean up temp object
        }

        private void Display(string showText)
        {
            selectedEmployeeBio.text = showText;
        }

        public void Hire()
        {
            if (selectedEmployee == null) return;

            Constructor.GetWorker(selectedEmployee);

            todaysEmployees.Remove(selectedEmployee);
            selectedEmployee = null;

            UpdateHireButtons();
        }

        public void Fire()
        {
            if (selectedEmployee == null) return;

            Tasks.Worker[] workers = FindObjectsOfType<Tasks.Worker>();

            for (int i = 0; i < workers.Length; i++)
            {
                if (workers[i].workerData == selectedEmployee)
                {
                    if (workers[i].Desk != null)
                    {
                        workers[i].Desk.currentWorker = null;
                        workers[i].Desk = null;
                    }

                    ParticleManager.instance.StartEffect_FireWorker(workers[i].transform.position);

                    hiredEmployees.Remove(workers[i].workerData);

                    Destroy(workers[i].gameObject);
                }
            }

            selectedEmployee = null;
            UpdateHireButtons();
        }

        public void Fire(Tasks.IWorker iworker)
        {
            var worker = (Tasks.Worker)iworker;
            if (worker.Desk != null)
            {
                worker.Desk.currentWorker = null;
                worker.Desk = null;
            }

            ParticleManager.instance.StartEffect_FireWorker(worker.transform.position);
            hiredEmployees.Remove(worker.workerData);
            Destroy(worker.gameObject);

            UpdateHireButtons();
        }
    }
}