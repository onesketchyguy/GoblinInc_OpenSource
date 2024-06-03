using UnityEngine;
using LowEngine.Tasks;
using LowEngine.Saving;

namespace LowEngine
{
    public class WorkerButton : MonoBehaviour
    {
        private SelectionUIHandler selectionUIHandler;

        public float moneyToAdd = 1;

        public Transform chair;

        public TaskWorkerAI currentWorker;

        public void Clicked()
        {
            if (currentWorker == null || Vector2.Distance(transform.position, currentWorker.transform.position) > 2)
            {
                return;
            }

            float mons = moneyToAdd * GameHandler.GetEarnDifficulty();
            GameHandler.instance.Money += mons * (1 - (currentWorker.worker.ineffiency / 100.0f));

            AudioManager.instance.PlayKeyClick(transform.position);
        }

        private void OnDestroy()
        {
            if (currentWorker != null)
                currentWorker.taskManager.tasks.Clear();
        }

        private void OnMouseDown()
        {
            if (ObjectPlacingManager.bullDozing || ObjectPlacingManager.Spawning != null) return;

            if (currentWorker != null)
            {
                selectionUIHandler.SelectWorker(currentWorker);
            }
            else
            if (SelectionUIHandler.SelectedWorker != null && currentWorker == null)
            {
                SetWorker(SelectionUIHandler.SelectedWorker);
            }
        }

        private void Start()
        {
            selectionUIHandler = FindObjectOfType<SelectionUIHandler>();
        }

        private void Update()
        {
            if (currentWorker != null)
            {
                float distanceToWorker = Vector2.Distance(currentWorker.transform.position, transform.position);

                if (distanceToWorker < 1)
                {
                    currentWorker.worker.Face(transform.position);
                }

                if (currentWorker.worker.Desk != this)
                {
                    currentWorker = null;
                }
            }            
        }

        public void SetWorker(TaskWorkerAI worker)
        {
            if (currentWorker != null && worker != currentWorker) return;

            currentWorker = worker;
            currentWorker.worker.Desk = this;

            TaskSystem.Task StartWorking = new TaskSystem.Task
            {
                moveToPosition = new TaskSystem.Task.MoveTo(chair.position, 0, () =>
                {
                    currentWorker = worker;
                    worker.SetThought(DialogueSys.GetWorkPhrase());
                }),
                executeActionRecurring = () =>
                {
                    Clicked();
                    if (Time.frameCount % 300 == 1)
                        worker.SetThought(DialogueSys.GetRandomPhrase());
                }
            };

            worker.taskManager.tasks.Enqueue(StartWorking);
        }
    }
}