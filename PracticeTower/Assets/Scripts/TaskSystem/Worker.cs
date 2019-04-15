using System;
using UnityEngine;
using LowEngine.Saving;
using LowEngine.Tasks.Needs;
using LowEngine.Navigation;

namespace LowEngine.Tasks
{
    public class Worker : MonoBehaviour, IWorker
    {
        public float speed = 5;

        public float unhappiness { get; set; }

        GameObject body;

        public float ineffiency { get; set; }

        PathFinding pathing;

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

        public float GetAllNeeds()
        {
            float total = 0;

            foreach (var need in needs)
            {
                total += Mathf.Round(need.value/needs.Length);
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
                onArrivedAtPosition = onArrivedAtPosition
            };
        }

        MoveToPosition moveTo;

        public void InitializeWorker(SaveManager.SavableObject.Worker bodyData, PathFinding pathFinding)
        {
            if (body != null)
            {
                Destroy(body);
            }

            pathing = pathFinding;

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

            unhappiness = 100 - totalHappiness;

            float skill = Mathf.Clamp(workerData.skill - unhappiness, 0, 100);

            workerData.position = transform.position;
            workerData.rotation = transform.rotation;

            ineffiency = 100f - skill;

            if (moveTo != null)
            {
                PathToPoint();
            }

            GetNeed(NeedDefinition.Hunger).Modify(-UnityEngine.Random.Range(0f, 1f) * Time.deltaTime);
            GetNeed(NeedDefinition.Thirst).Modify(-UnityEngine.Random.Range(0f, 2f) * Time.deltaTime);

            workerData.hunger = GetNeed(NeedDefinition.Hunger).value;
            workerData.thirst = GetNeed(NeedDefinition.Thirst).value;
        }

        int tries;

        int pathIndex = 0;
        void PathToPoint()
        {
            float distanceToTarget = Vector3.Distance(transform.position, moveTo.position);

            if (pathing.Path.Count == 0)
            {
                if (tries < 20)
                {
                    FindObjectOfType<MapLayoutManager>().UpdateGrid();

                    pathing.FindPath(transform.position, moveTo.position);

                    tries++;
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, moveTo.position, speed * Time.deltaTime);
                }

                return;
            }
            else
            {
                for (int i = 0; i < pathing.Path.Count; i++)
                {
                    Node node = pathing.Path[i];
                    if (i + 1 < pathing.Path.Count)
                        Debug.DrawLine(node.position, pathing.Path[i + 1].position, Color.red, 1);
                    else
                    {
                        Debug.DrawLine(node.position, transform.position, Color.blue);
                    }
                }

                Node currentNode = pathing.Path[pathIndex];

                float distanceToNode = Vector3.Distance(transform.position, currentNode.position);

                if (distanceToTarget < 2f + moveTo.stoppingDistance)
                {
                    transform.position = Vector3.MoveTowards(transform.position, moveTo.position, speed * Time.deltaTime);
                }
                else
                {
                    transform.position = Vector3.MoveTowards(transform.position, currentNode.position, speed * Time.deltaTime);

                    if (distanceToNode < 0.5f + moveTo.stoppingDistance)
                    {
                        pathIndex++;
                    }
                }
            }

            if (distanceToTarget <= moveTo.stoppingDistance)
            {
                if (moveTo.onArrivedAtPosition != null)
                {
                    moveTo.onArrivedAtPosition.Invoke();
                }

                pathIndex = 0;
                tries = 0;

                pathing.Path = new System.Collections.Generic.List<Node>();

                moveTo = null;
            }
            else
            {
                Face(moveTo.position);
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
    }

    public class MoveToPosition
    {
        public Vector3 position;

        public float stoppingDistance;

        public Action onArrivedAtPosition;
    }

    public interface IWorker
    {
        Saving.SaveManager.SavableObject.Worker workerData { get; set; }

        void MoveTo(Vector3 position, float stoppingDistance, System.Action onArrivedAtPosition = null);

        void ExecuteAction(System.Action action = null);

        void InitializeWorker(Saving.SaveManager.SavableObject.Worker bodyData, Navigation.PathFinding pathFinding);

        void Face(Vector3 position);

        float unhappiness { get; set; }

        Need GetNeed(NeedDefinition needDefinition);

        Need[] needs { get; set; }

        float ineffiency { get; set; }
    }
}