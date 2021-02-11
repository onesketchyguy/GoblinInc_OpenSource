using UnityEngine;
using UnityEngine.UI;
using LowEngine.Tasks;

namespace LowEngine
{
    public class SelectionUIHandler : MonoBehaviour
    {
        public static TaskWorkerAI SelectedWorker { get; private set; }

        public Color SelectedColor = Color.white;

        public Text NameDisplay, CostDisplay, SkillDisplay, EfficiencyDisplay, UnhappinessDisplay, ThoughtDisplay;

        public Slider ExperienceDisplay, HungerSlider, ThirstSlider;

        public UnityEngine.Events.UnityEvent onSelectEvent;

        private void Update()
        {
            if (SelectedWorker == null) return;

            if (Input.GetButton("Fire2")) DeselectWorker();

            if (Time.frameCount % 10 == 0) UpdateInformation();
        }

        public void SelectWorker(TaskWorkerAI worker)
        {
            DeselectWorker();

            SelectedWorker = worker;

            UpdateInformation();

            onSelectEvent?.Invoke();
        }

        private void UpdateInformation()
        {
            NameDisplay.text = $"{SelectedWorker.name}";

            CostDisplay.text = $"This worker is costing you: ${SelectedWorker.worker.workerData.pay} per week.";
            SkillDisplay.text = $"Skill Level: {SelectedWorker.worker.workerData.skill}/100";
            EfficiencyDisplay.text = $"Efficiency: { 100 - SelectedWorker.worker.ineffiency}%";
            UnhappinessDisplay.text = $"Unhappiness:{SelectedWorker.worker.unhappiness}%";
            ThoughtDisplay.text = $"Last thought: \"{SelectedWorker.currentThought}\"";

            ExperienceDisplay.value = SelectedWorker.worker.workerData.experience;
            HungerSlider.value = 1 - (SelectedWorker.worker.workerData.hunger * 0.01f);
            ThirstSlider.value = 1 - (SelectedWorker.worker.workerData.thirst * 0.01f);
        }

        public static void DeselectWorker()
        {
            if (SelectedWorker != null)
            {
                SelectedWorker = null;

                UIManager.instance.HideMenus();
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

        public void FireWorker()
        {
            if (SelectedWorker)
            {
                ParticleManager.instance.StartEffect_FireWorker(SelectedWorker.transform.position);

                SelectedWorker.worker.Desk.currentWorker = null;

                Destroy(SelectedWorker.gameObject);

                DeselectWorker();
            }
        }
    }
}