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
            if (currentWorker == null || Vector2.Distance(transform.position, currentWorker.transform.position) > 2)
            {
                Debug.Log($"Some one is trying to click {gameObject.name} but I dont have any workers assigned to do that!");

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


            if (currentWorker != null)
            {
                SelectionUIHandler.SelectWorker(currentWorker);
            }
            else
            if (SelectionUIHandler.SelectedWorker != null && currentWorker == null)
            {
                SetWorker(SelectionUIHandler.SelectedWorker);
            }
        }

        public void SetupSaveableObject()
        {
            if (GetComponent<PlacedObject>())
            {
                GetComponent<PlacedObject>().objectData = new Saving.SaveManager.SavableObject.WorldObject
                {
                    type = ObjectType.Table,
                    objectType = PlacedObjectType.Desk,
                    position = transform.position,
                    rotation = transform.rotation,
                    name = $"{gameObject.name}.{transform.position}",
                    childPos = chair.localPosition,
                    sprite = GetComponent<SpriteRenderer>().sprite,
                    color = Color.white
                };
            }
        }

        float inactiveTime;

        private void Update()
        {
            SetupSaveableObject();

            if (currentWorker != null)
            {
                float distanceToWorker = Vector2.Distance(currentWorker.transform.position, transform.position);

                if (distanceToWorker < 1)
                {
                    currentWorker.worker.Face(transform.position);
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
                moveToPosition = new TaskSystem.Task.MoveTo(chair.position, 0, () => currentWorker = worker),
                executeActionRecurring = () =>
                {
                    Clicked();
                }
            };

            worker.taskManager.tasks.Enqueue(StartWorking);
        }
    }
}