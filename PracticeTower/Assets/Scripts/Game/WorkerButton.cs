using UnityEngine;
using LowEngine.Tasks;
using LowEngine.Saving;

namespace LowEngine
{
    public class WorkerButton : MonoBehaviour, ISaveableObject
    {
        public int moneyToAdd = 1;

        public Transform chair;

        public TaskWorkerAI currentWorker;

        public void Clicked()
        {
            if (Vector2.Distance(transform.position, currentWorker.transform.position) >= 2)
            {
                currentWorker.taskManager.tasks.Clear();

                return;
            }

            GameHandler.instance.Money += moneyToAdd;

            AudioManager.instance.PlayKeyClick(transform.position);
        }

        private void OnDestroy()
        {
            if (currentWorker != null)
                currentWorker.taskManager.tasks.Clear();
        }

        private void OnMouseDown()
        {
            if (PlaceObjectMenu.bullDozing) return;

            if (SelectionUIHandler.SelectedWorker != null)
            {
                SetWorker(SelectionUIHandler.SelectedWorker);
            }
            else if (currentWorker != null)
            {
                SelectionUIHandler.SelectWorker(currentWorker);
            }
        }

        public void SetupSaveableObject()
        {
            if (GetComponent<PlacedObject>())
            {
                GetComponent<PlacedObject>().thisObject = new Saving.SaveManager.SavableObject.WorldObject
                {
                    objectType = PlacedObjectType.Desk,
                    position = transform.position,
                    rotation = transform.rotation,
                    name = $"{gameObject.name}.{transform.position}",
                    childPos = chair.position,
                    sprite = GetComponent<SpriteRenderer>().sprite,
                    color = Color.white
                };
            }
        }

        float inactivityTime = 0;

        private void Update()
        {
            SetupSaveableObject();

            if (currentWorker != null)
            {
                float distanceToWorker = Vector2.Distance(currentWorker.transform.position, transform.position);

                if (distanceToWorker < 1)
                {
                    currentWorker.worker.Face(transform.position);

                    inactivityTime = 0;
                }
                else
                {
                    if (currentWorker.taskManager.tasks.Count > 0)
                    {
                        inactivityTime += Time.deltaTime;

                        if (inactivityTime >= 2)
                        {
                            currentWorker.state = TaskWorkerAI.State.WaitingForNewTask;

                            currentWorker.taskManager.tasks.Clear();

                            currentWorker = null;

                            return;
                        }
                    }
                }

                if (currentWorker.taskManager.tasks.Count == 0)
                {
                    currentWorker = null;
                }
            }
        }

        public void SetWorker(TaskWorkerAI worker)
        {
            if (currentWorker != null) return;

            TaskSystem.Task getOverHere = new TaskSystem.Task
            {
                moveToPosition = new TaskSystem.Task.MoveTo(chair.position, 0)
            };

            TaskSystem.Task clickButton = new TaskSystem.Task
            {
                executeActionRecurring = () =>
                {
                    Clicked();
                }
            };

            worker.taskManager.tasks.Enqueue(getOverHere);

            worker.taskManager.tasks.Enqueue(clickButton);

            currentWorker = worker;
        }
    }
}