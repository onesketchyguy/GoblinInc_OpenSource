using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using LowEngine.Tasks;

namespace LowEngine
{
    public class SelectionUIHandler : MonoBehaviour
    {
        public static TaskWorkerAI SelectedWorker { get; private set; }

        public Color SelectedColor = Color.white;

        public Text NameDisplay, SkillDisplay, EfficiencyDisplay, UnhappinessDisplay;

        public Slider ExperienceDisplay, HungerSlider, ThirstSlider;

        public static void SelectWorker(TaskWorkerAI worker)
        {
            DeselectWorker();

            UIManager.instance.UpdateShowing(UIManager.Show.workerInfo);

            SelectedWorker = worker;
        }

        public static void DeselectWorker()
        {
            if (SelectedWorker != null)
            {
                SelectedWorker = null;

                UIManager.instance.UpdateShowing(UIManager.Show.none);
            }
        }

        public void SendWorkerHome()
        {
            if (SelectedWorker)
            {
                SelectedWorker.taskManager.tasks.Clear();

                SelectedWorker.taskManager.tasks.Enqueue(SelectedWorker.GoHome);

                DeselectWorker();
            }
        }

        private void Update()
        {
            if (Input.GetButton("Fire2"))
            {
                DeselectWorker();

                return;
            }

            NameDisplay.text = $"{SelectedWorker.name} : ${SelectedWorker.worker.workerData.pay}";
            SkillDisplay.text = $"Skill Level: {SelectedWorker.worker.workerData.skill}/100";
            EfficiencyDisplay.text = $"Efficiency: { 100 - SelectedWorker.worker.ineffiency}%";
            UnhappinessDisplay.text = $"Unhappiness:{SelectedWorker.worker.unhappiness}%";

            ExperienceDisplay.value = SelectedWorker.worker.workerData.experience;
            HungerSlider.value = 1 - (SelectedWorker.worker.workerData.hunger * 0.01f);
            ThirstSlider.value = 1 - (SelectedWorker.worker.workerData.thirst * 0.01f);
        }

        public void FireWorker()
        {
            if (SelectedWorker)
            {
                ParticleManager.instance.StartEffect_FireWorker(SelectedWorker.transform.position);

                Destroy(SelectedWorker.gameObject);

                DeselectWorker();
            }
        }
    }
}