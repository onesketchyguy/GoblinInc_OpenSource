using UnityEngine;
using LowEngine.Tasks.Needs;

namespace LowEngine.Tasks
{
    public class TaskWorkerAI : MonoBehaviour
    {
        public string currentTask;

        public string currentThought;

        internal IWorker worker;

        public enum State { WaitingForNewTask, ExecutingTask }

        public State state;

        float WaitingTimer;

        public TaskSystem taskManager;

        public TaskSystem.Task GetNearestDesk => new TaskSystem.Task
        {
            executeAction = () =>
            {
                if (GetNearestJob() != null)
                {
                    currentThought = ("I found a desk.");

                    GetNearestJob().SetWorker(this);
                }
                else
                {
                    currentThought = ("I can't find a desk.");
                }

                state = State.WaitingForNewTask;
            }
        };

        public TaskSystem.Task FulfilNeed(NeedDefinition need)
        {
            TaskSystem.Task fulfil = null;

            if (GetNearestNeed(need) != null)
            {
                fulfil = new TaskSystem.Task
                {
                    moveToPosition = new TaskSystem.Task.MoveTo(GetNearestNeed(need).transform.position, 1f, () =>
                    {
                        GetNearestNeed(need).FulFillneed(worker);

                        state = State.WaitingForNewTask;
                    }),
                };
            }

            return fulfil;
        }

        public TaskSystem.Task GoHome => new TaskSystem.Task
        {
            moveToPosition = new TaskSystem.Task.MoveTo(FindObjectOfType<MapLayoutManager>().PlayAreaSize/2, 0, () => 
            {
                worker.GetNeed(NeedDefinition.Hunger).Set(100);
                worker.GetNeed(NeedDefinition.Thirst).Set(100);

                state = State.WaitingForNewTask;
            }),
        };

        public TaskSystem.Task ComeBack => new TaskSystem.Task
        {
            moveToPosition = new TaskSystem.Task.MoveTo(transform.position, 0, () => state = State.WaitingForNewTask)
        };

        public TaskSystem.Task celebrate => new TaskSystem.Task
        {
            executeAction = () =>
            {
                currentThought = DialogueSys.GetCelebration();

                state = State.WaitingForNewTask;
            }
        };

        public void Setup(IWorker worker)
        {
            this.worker = worker;
            taskManager = new TaskSystem();
            state = State.WaitingForNewTask;

            BoxCollider2D boxCollider = gameObject.AddComponent<BoxCollider2D>();
            boxCollider.isTrigger = true;
        }

        private void Update()
        {
            switch (state)
            {
                case State.WaitingForNewTask:
                    //Wait to request a new task
                    WaitingTimer -= Time.deltaTime;

                    if (WaitingTimer <= 0)
                    {
                        WaitingTimer = worker.ineffiency * Time.deltaTime;
                        RequestNextTask();

                        float valX = Mathf.Abs(worker.workerData.skill - worker.unhappiness) + (worker.workerData.skill/2); 

                        if (valX <= worker.workerData.skill)
                        {
                            //Alieve this unhappiness

                            taskManager.tasks.Clear();

                            if (worker.GetNeed(NeedDefinition.Hunger).value <= 100 - worker.workerData.skill)
                            {
                                if (FulfilNeed(NeedDefinition.Hunger) != null)
                                {
                                    taskManager.tasks.Enqueue(FulfilNeed(NeedDefinition.Hunger));
                                }
                                else
                                {
                                    taskManager.tasks.Enqueue(GoHome);
                                }
                            }

                            if (worker.GetNeed(NeedDefinition.Thirst).value <= 100 - worker.workerData.skill)
                            {
                                if (FulfilNeed(NeedDefinition.Thirst) != null)
                                {
                                    taskManager.tasks.Enqueue(FulfilNeed(NeedDefinition.Thirst));
                                }
                                else
                                {
                                    taskManager.tasks.Enqueue(GoHome);
                                }
                            }

                            taskManager.tasks.Enqueue(ComeBack);
                        }

                        if (worker.workerData.experience > 1f)
                        {
                            if (worker.workerData.skill < 99)
                            {
                                worker.workerData.skill += 1;
                                worker.workerData.experience = 0;
                            }
                        }

                        if (worker.workerData.experience < 0f)
                        {
                            if (worker.workerData.skill > 1)
                            {
                                worker.workerData.skill -= 1;
                                worker.workerData.experience = 0.99f;
                            }
                        }
                    }
                    break;
                case State.ExecutingTask:
                    break;
            }
        }

        private void RequestNextTask()
        {
            TaskSystem.Task task = null;

            if (taskManager.tasks.Count > 0) task = taskManager.tasks.Dequeue();

            if (task == null)
            {
                state = State.WaitingForNewTask;

                taskManager.tasks.Enqueue(GetNearestDesk);
            }
            else
            {
                worker.workerData.experience += Random.Range(0f, 1 - ((worker.workerData.skill * 0.005f) + (worker.unhappiness * 0.01f))) * Time.deltaTime;

                state = State.ExecutingTask;

                if (task.moveToPosition != null)
                {
                    worker.MoveTo(task.moveToPosition.targetPosition, task.moveToPosition.stoppingDistance, () =>
                    {
                        if (task.moveToPosition.executeAction != null)
                        {
                            task.moveToPosition.executeAction.Invoke();
                        }

                        state = State.WaitingForNewTask;

                        taskManager.tasks.Enqueue(celebrate);
                    });
                }

                if (task.executeAction != null)
                {
                    state = State.WaitingForNewTask;

                    worker.ExecuteAction(task.executeAction);

                    if (task.executeAction != celebrate.executeAction)
                        taskManager.tasks.Enqueue(celebrate);
                }

                if (task.executeActionRecurring != null)
                {
                    state = State.WaitingForNewTask;

                    worker.ExecuteAction(task.executeActionRecurring);

                    TaskSystem.Task recurringTask = new TaskSystem.Task
                    {
                        executeActionRecurring = task.executeActionRecurring
                    };

                    taskManager.tasks.Enqueue(recurringTask);
                    taskManager.tasks.Enqueue(celebrate);
                }
            }
        }

        private NeedFulfiller GetNearestNeed(NeedDefinition need)
        {
            NeedFulfiller nearestNeed = null;
            NeedFulfiller[] workerButtons = FindObjectsOfType<NeedFulfiller>();

            float nearest = 20;

            foreach (var desk in workerButtons)
            {
                float dist = Vector2.Distance(transform.position, desk.transform.position);

                if (dist < nearest && desk.Fulfills == need)
                {
                    nearestNeed = desk;
                    nearest = dist;
                }
            }

            return nearestNeed;
        }

        private WorkerButton GetNearestJob()
        {
            WorkerButton button = null;
            WorkerButton[] workerButtons = FindObjectsOfType<WorkerButton>();

            float nearest = 20;

            foreach (var desk in workerButtons)
            {
                float dist = Vector2.Distance(transform.position, desk.transform.position);

                if (dist < nearest && desk.currentWorker == null)
                {
                    button = desk;
                    nearest = dist;
                }
            }

            return button;
        }

        private void OnMouseDown()
        {
            SelectionUIHandler.SelectWorker(this);
        }
    }
}