﻿using System;
using UnityEngine;
using LowEngine.Saving;
using LowEngine.Tasks.Needs;
using LowEngine.Navigation;
using LowEngine.TimeManagement;

namespace LowEngine.Tasks
{
    public class Worker : MonoBehaviour, IWorker
    {
        public float speed = 5;

        public float unhappiness { get; set; }

        private GameObject body;

        public WorkerButton Desk { get; set; }

        public float ineffiency { get; set; }

        public SaveManager.SavableObject.Worker workerData { get; set; }
        public Need[] needs { get; set; } = new Need[] { new Need { thisNeed = NeedDefinition.Hunger }, new Need { thisNeed = NeedDefinition.Thirst } };

        public Need GetNeed(NeedDefinition needToGet)
        {
            foreach (var need in needs)
            {
                if (need.thisNeed == needToGet)
                    return need;
            }

            return null;
        }

        private void OnEnable()
        {
            GameHandler.RegisterWorker(this);
        }

        private void OnDisable()
        {
            GameHandler.DeregisterWorker(this);
        }

        public float GetAllNeeds()
        {
            float total = 0;

            foreach (var need in needs)
            {
                total += Mathf.Round(need.value / needs.Length);
            }

            return total;
        }

        public void ExecuteAction(Action action = null)
        {
            if (action != null)
            {
                action.Invoke();
            }
        }

        public void MoveTo(Vector3 MoveToPosition, float stoppingDistance, Action onArrivedAtPosition = null)
        {
            moveTo = new MoveToPosition
            {
                position = MoveToPosition,
                stoppingDistance = stoppingDistance,
                onArrivedAtPosition = onArrivedAtPosition,
                pathFinding = new PathFinding()
            };

            moveTo.pathFinding.FindPath(transform.position, moveTo.position, FindObjectOfType<MapLayoutManager>());
        }

        private MoveToPosition moveTo;

        public void InitializeWorker(SaveManager.SavableObject.Worker bodyData)
        {
            if (body != null)
            {
                Destroy(body);
            }

            workerData = bodyData;

            GetNeed(NeedDefinition.Hunger).Set(workerData.hunger);
            GetNeed(NeedDefinition.Thirst).Set(workerData.hunger);

            body = FindObjectOfType<WorkerSpriteGenerator>().GetWorker(bodyData);

            body.transform.localScale = Vector2.one * 0.5f;

            body.transform.position = transform.position;

            body.transform.SetParent(transform);
        }

        private void Update()
        {
            float totalHappiness = Mathf.Clamp(GetAllNeeds(), 0, 100);

            speed = 5 + Mathf.Abs(DayCycle.timeScale);

            unhappiness = 100 - totalHappiness;

            float skill = Mathf.Clamp(workerData.skill - unhappiness, 0, 100);

            workerData.position = transform.position;
            workerData.rotation = transform.rotation;

            ineffiency = 100f - skill;

            if (moveTo != null)
            {
                PathToPoint();
            }

            if (TimeScale.isDayTime())
            {
                float hungerRate = 0.25f * DayCycle.timeScale;
                float thirstRate = 0.5f * DayCycle.timeScale;

                GetNeed(NeedDefinition.Hunger).Modify(-UnityEngine.Random.Range(0f, hungerRate * Time.deltaTime));
                GetNeed(NeedDefinition.Thirst).Modify(-UnityEngine.Random.Range(0f, thirstRate * Time.deltaTime));
            }

            workerData.hunger = GetNeed(NeedDefinition.Hunger).value;
            workerData.thirst = GetNeed(NeedDefinition.Thirst).value;
            workerData.pay = workerData.FlatPay + workerData.skill;

            if (workerData.experience > 1f)
            {
                if (workerData.skill < 99)
                {
                    workerData.skill += 1;
                    workerData.experience = 0;
                }
            }

            if (workerData.experience < 0f)
            {
                if (workerData.skill > 1)
                {
                    workerData.skill -= 1;
                    workerData.experience = 0.99f;
                }
            }

            SetVisable(Vector2.Distance(transform.position, TaskWorkerAI.home) > 5);
        }

        private void PathToPoint()
        {
            float distanceToTarget = Vector3.Distance(transform.position, moveTo.position);

            if (distanceToTarget < 2f + moveTo.stoppingDistance)
            {
                transform.position = Vector3.MoveTowards(transform.position, moveTo.position, speed * Time.deltaTime);

                if (distanceToTarget <= moveTo.stoppingDistance)
                {
                    if (moveTo.onArrivedAtPosition != null)
                    {
                        moveTo.onArrivedAtPosition.Invoke();
                    }

                    moveTo = null;
                }
                else
                {
                    Face(moveTo.position);
                }
            }
            else
            {
                if (moveTo.pathFinding.Path.Count == 0 || moveTo.pathFinding.Path == null)
                {
                    moveTo.pathIndex = 0;

                    if (moveTo.pathFinding.stopwatch.IsRunning == false)
                    {
                        if (moveTo.pathingAttempts < moveTo.maxAttempts)
                        {
                            moveTo.pathFinding.FindPath(transform.position, moveTo.position, FindObjectOfType<MapLayoutManager>());

                            moveTo.pathingAttempts++;
                        }
                        else
                        {
                            transform.position = Vector3.MoveTowards(transform.position, moveTo.position, speed * Time.deltaTime);
                        }
                    }
                    else
                    {
                        if (moveTo.pathFinding.stopwatch.ElapsedMilliseconds > 1000)
                        {
                            moveTo.pathingAttempts++;

                            moveTo.pathFinding.stopwatch.Stop();
                        }
                    }
                }
                else
                {
                    MapLayoutManager.ChosenNodes = moveTo.pathFinding.Path;

                    //----------------------------------------Node movement-------------------------------
                    for (int first = 0; first < moveTo.pathFinding.Path.Count; first++)
                    {
                        for (int second = 1; second < moveTo.pathFinding.Path.Count + 1; second++)
                        {
                            if (second < moveTo.pathFinding.Path.Count)
                            {
                                Debug.DrawLine(moveTo.pathFinding.Path[first].position, moveTo.pathFinding.Path[second].position, Color.red);
                            }
                            else
                            {
                                Debug.DrawLine(moveTo.pathFinding.Path[first].position, transform.position, Color.blue);
                            }
                        }
                    }

                    Node currentNode = null;

                    if (moveTo.pathIndex < moveTo.pathFinding.Path.Count)
                    {
                        currentNode = moveTo.pathFinding.Path[moveTo.pathIndex];
                    }
                    else
                    {
                        int val = moveTo.pathFinding.Path.Count - 1 >= 0 ? moveTo.pathFinding.Path.Count - 1 : 0;
                        currentNode = moveTo.pathFinding.Path[val];
                    }

                    if (currentNode == null) return;

                    float distanceToNode = Vector3.Distance(transform.position, currentNode.position);

                    transform.position = Vector3.MoveTowards(transform.position, currentNode.position, speed * Time.deltaTime);

                    if (distanceToNode < 0.5f + moveTo.stoppingDistance)
                    {
                        if (moveTo.pathIndex < moveTo.pathFinding.Path.Count)
                            moveTo.pathIndex++;
                    }
                    //----------------------------------------end Node movement-------------------------------
                }
            }
        }

        public void Face(Vector3 position)
        {
            foreach (var spriteRenderer in GetComponentsInChildren<SpriteRenderer>())
            {
                if (position.x > transform.position.x + 0.1f)
                {
                    spriteRenderer.flipX = true;
                }
                else
                if (position.x < transform.position.x - 0.1f)
                {
                    spriteRenderer.flipX = false;
                }
            }
        }

        public void SetVisable(bool visability)
        {
            foreach (var spriteRenderer in GetComponentsInChildren<SpriteRenderer>())
            {
                spriteRenderer.enabled = visability;
            }
        }
    }

    public class MoveToPosition
    {
        public PathFinding pathFinding;
        public int pathIndex = 0;

        public int maxAttempts
        {
            get
            {
                return 2;
            }
        }

        public int pathingAttempts;

        public Vector3 position;

        public float stoppingDistance;

        public Action onArrivedAtPosition;
    }

    public interface IWorker
    {
        WorkerButton Desk { get; set; }

        SaveManager.SavableObject.Worker workerData { get; set; }

        void MoveTo(Vector3 position, float stoppingDistance, System.Action onArrivedAtPosition = null);

        void ExecuteAction(System.Action action = null);

        void InitializeWorker(SaveManager.SavableObject.Worker bodyData);

        void Face(Vector3 position);

        float unhappiness { get; set; }

        Need GetNeed(NeedDefinition needDefinition);

        Need[] needs { get; set; }

        float ineffiency { get; set; }
    }
}