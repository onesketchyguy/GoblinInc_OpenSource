﻿using UnityEngine;
using LowEngine.Tasks.Needs;
using LowEngine.TimeManagement;

namespace LowEngine.Tasks
{
    public class TaskWorkerAI : MonoBehaviour
    {
        private SelectionUIHandler selectionUIHandler;

        public string currentTask;
        public string currentThought { get; private set; }
        float thoughtTime = 0;
        public void SetThought(string thought)
        {
            if (Time.timeSinceLevelLoad < thoughtTime) return;
            thoughtTime = Time.timeSinceLevelLoad + 15;
            currentThought = thought;
        }

        internal IWorker worker;

        public enum State { WaitingForNewTask, ExecutingTask }

        public State state;

        private float WaitingTimer;

        public TaskSystem taskManager;

        private static MapLayoutManager map;
        public static Vector3 home
        {
            get
            {
                if (map == null) map = FindObjectOfType<MapLayoutManager>();
                if (map == null) Debug.LogError("Unable to locate MapLayoutManager!");
                return map.PlayAreaSize + Vector2.one;
            }
        }

        public bool AtHome { get { return Vector2.Distance(transform.position, home) < 10; } }

        public TaskSystem.Task GetNearestDesk => new TaskSystem.Task
        {
            executeAction = () =>
            {
                if (GetNearestJob() != null)
                {
                    SetThought("I found a desk.");

                    GetNearestJob().SetWorker(this);
                }
                else
                {
                    NotificationManager.instance.ShowNotificationOnce($"{worker.workerData.name} cannot find a desk");
                    SetThought("I can't find a desk.");
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

                        SetThought(DialogueSys.GetCelebration());

                        state = State.WaitingForNewTask;
                    }),
                };

                currentTask = "Taking a break.";
            } else Debug.Log($"There is no {need} fulfiller nearby.");

            return fulfil;
        }

        public TaskSystem.Task GoHome => new TaskSystem.Task
        {
            moveToPosition = new TaskSystem.Task.MoveTo(home, 0, () =>
            {
                worker.GetNeed(NeedDefinition.Hunger).Set(100);
                worker.GetNeed(NeedDefinition.Thirst).Set(100);

                currentTask = "Going home.";
                SetThought( DialogueSys.GetPitty());

                state = State.WaitingForNewTask;
            }),
        };

        public TaskSystem.Task ComeBack => new TaskSystem.Task
        {
            moveToPosition = new TaskSystem.Task.MoveTo(transform.position, 0, () =>
            {
                state = State.WaitingForNewTask;
            })
        };

        public TaskSystem.Task celebrate => new TaskSystem.Task
        {
            executeAction = () =>
            {
                SetThought( DialogueSys.GetCelebration());

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

        private void Start()
        {
            selectionUIHandler = FindObjectOfType<SelectionUIHandler>();
        }

        private void Update()
        {
            if (DayCycle.timeScale == 0)
                return;

            switch (state)
            {
                case State.WaitingForNewTask:
                    //Wait to request a new task
                    WaitingTimer -= (DayCycle.timeScale) * Time.deltaTime;

                    if (WaitingTimer <= 0)
                    {
                        WaitingTimer = (worker.ineffiency + (DayCycle.timeScale)) * Time.deltaTime;

                        RequestNextTask();
                    }
                    break;

                case State.ExecutingTask:
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

                currentThought = "Need a break. ";

                float skill = 200 - worker.workerData.skill;

                if (worker.GetNeed(NeedDefinition.Thirst).value <= skill)
                {
                    currentThought += "Thirsty. ";

                    TaskSystem.Task thirst = FulfilNeed(NeedDefinition.Thirst);

                    if (thirst != null)
                    {
                        taskManager.tasks.Enqueue(thirst);
                    }
                    else
                    {
                        taskManager.tasks.Clear();

                        taskManager.tasks.Enqueue(GoHome);

                        taskManager.tasks.Enqueue(ComeBack);

                        return;
                    }
                }

                if (worker.GetNeed(NeedDefinition.Hunger).value <= skill)
                {
                    currentThought += "Hungry. ";

                    TaskSystem.Task hunger = FulfilNeed(NeedDefinition.Hunger);

                    if (hunger != null)
                    {
                        taskManager.tasks.Enqueue(hunger);
                    }
                    else
                    {
                        taskManager.tasks.Clear();

                        taskManager.tasks.Enqueue(GoHome);

                        taskManager.tasks.Enqueue(ComeBack);

                        return;
                    }
                }

                taskManager.tasks.Enqueue(ComeBack);
            }
        }

        private void RequestNextTask()
        {
            if (TimeScale.isDayTime() == false && AtHome) // This is meant to negate a null error below
            {
                taskManager.tasks.Clear();
                return;
            }
            HandleNeeds();

            TaskSystem.Task task = null;

            if (taskManager.tasks.Count > 0) task = taskManager.tasks.Dequeue();

            if (task == null)
            {
                if (TimeScale.isDayTime())
                {
                    if (worker.Desk != null)
                    {
                        worker.Desk.SetWorker(this);
                    }
                    else
                    {
                        currentTask = "Waiting for a task.";

                        state = State.WaitingForNewTask;

                        if (GetNearestDesk != null)
                        {
                            taskManager.tasks.Enqueue(GetNearestDesk);
                        }
                    }
                }
                else
                {
                    taskManager.tasks.Enqueue(GoHome);
                }
            }
            else
            {

                if (task == ComeBack && TimeScale.isDayTime() == false && AtHome)
                {
                    taskManager.tasks.Clear();

                    return;
                }

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
        }

        private NeedFulfiller GetNearestNeed(NeedDefinition need)
        {
            NeedFulfiller nearestNeed = null;
            NeedFulfiller[] fulfillments = FindObjectsOfType<NeedFulfiller>();

            float nearest = 20;

            foreach (var desk in fulfillments)
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

            Vector2 vector2 = FindObjectOfType<MapLayoutManager>().PlayAreaSize * 2;

            float nearest = (vector2.x > vector2.y) ? vector2.x : vector2.y;

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
            selectionUIHandler.SelectWorker(this);
        }
    }
}