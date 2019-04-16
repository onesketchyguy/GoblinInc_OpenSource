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

                        currentThought = "That's better";

                        state = State.WaitingForNewTask;
                    }),
                };

                currentTask = "Taking a break.";
            }

            return fulfil;
        }

        public TaskSystem.Task GoHome => new TaskSystem.Task
        {
            moveToPosition = new TaskSystem.Task.MoveTo(FindObjectOfType<MapLayoutManager>().PlayAreaSize - Vector2.one, 0, () => 
            {
                worker.GetNeed(NeedDefinition.Hunger).Set(100);
                worker.GetNeed(NeedDefinition.Thirst).Set(100);

                currentTask = "Going home.";

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

        float timeSinceLastTask;

        private void Update()
        {
            if (timeSinceLastTask > 101 * Time.deltaTime)
            {
                RequestNextTask();

                return;
            }

            switch (state)
            {
                case State.WaitingForNewTask:
                    //Wait to request a new task
                    WaitingTimer -= Time.deltaTime;

                    if (WaitingTimer <= 0)
                    {
                        timeSinceLastTask += WaitingTimer;

                        WaitingTimer = worker.ineffiency * Time.deltaTime;

                        RequestNextTask();
                    }
                    break;
                case State.ExecutingTask:
                    timeSinceLastTask = 0;
                    break;
            }
        }

        private void HandleNeeds()
        {
            float valX = Mathf.Abs(worker.workerData.skill - worker.unhappiness) + (worker.workerData.skill / 2);

            if (valX <= worker.workerData.skill)
            {
                //Alieve this unhappiness

                taskManager.tasks.Clear();

                currentThought = "Need a break.";

                float skill = 150 - worker.workerData.skill;

                if (worker.GetNeed(NeedDefinition.Hunger).value <= skill)
                {
                    currentThought = "Hungery";

                    TaskSystem.Task hunger = FulfilNeed(NeedDefinition.Hunger);

                    if (hunger != null)
                    {
                        taskManager.tasks.Enqueue(hunger);
                    }
                    else
                    {
                        taskManager.tasks.Enqueue(GoHome);
                    }
                }

                if (worker.GetNeed(NeedDefinition.Thirst).value <= skill)
                {
                    currentThought = "Thirsty";

                    TaskSystem.Task thirst = FulfilNeed(NeedDefinition.Thirst);

                    if (thirst != null)
                    {
                        taskManager.tasks.Enqueue(thirst);
                    }
                    else
                    {
                        taskManager.tasks.Enqueue(GoHome);
                    }
                }

                taskManager.tasks.Enqueue(ComeBack);
            }
        }

        private void RequestNextTask()
        {
            TaskSystem.Task task = null;

            if (taskManager.tasks.Count > 0) task = taskManager.tasks.Dequeue();

            if (task == null)
            {
                currentTask = "Waiting for a task.";

                state = State.WaitingForNewTask;

                if (GetNearestDesk != null)
                {
                    taskManager.tasks.Enqueue(GetNearestDesk);
                }
            }
            else
            {
                state = State.ExecutingTask;

                if (task.moveToPosition != null)
                {
                    currentTask = "Moving to location.";

                    worker.MoveTo(task.moveToPosition.targetPosition, task.moveToPosition.stoppingDistance, () =>
                    {
                        if (task.moveToPosition.executeAction != null)
                        {
                            task.moveToPosition.executeAction.Invoke();
                        }

                        state = State.WaitingForNewTask;
                    });
                }

                if (task.executeAction != null)
                {
                    currentTask = "Executing task.";

                    worker.ExecuteAction(task.executeAction);

                    state = State.WaitingForNewTask;
                }

                if (task.executeActionRecurring != null)
                {
                    worker.workerData.experience += Random.Range(0f, 1 - ((worker.workerData.skill * 0.005f) + (worker.unhappiness * 0.01f))) * Time.deltaTime;

                    currentTask = "Executing a recurring task.";

                    worker.ExecuteAction(task.executeActionRecurring);

                    TaskSystem.Task recurringTask = new TaskSystem.Task
                    {
                        executeActionRecurring = task.executeActionRecurring
                    };


                    state = State.WaitingForNewTask;
                }
            }

            HandleNeeds();
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

                if (dist < nearest && (desk.currentWorker == null || desk.currentWorker == this))
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