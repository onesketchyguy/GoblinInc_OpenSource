using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LowEngine.Tasks;
using LowEngine.Tasks.Needs;
using LowEngine.Navigation;

namespace LowEngine.Saving
{
    public enum PlacedObjectType { Static, Desk, Need }

    public class Constructor : MonoBehaviour
    {
        public static GameObject GetWorker(SaveManager.SavableObject.Worker WorkerToSpawn)
        {
            Vector3 spawnPoint = FindObjectOfType<MapLayoutManager>().PlayAreaSize/2;

            GameObject obj = new GameObject(WorkerToSpawn.name);

            TaskWorkerAI workerAI = obj.AddComponent<TaskWorkerAI>();
            PathFinding pathFinding = obj.AddComponent<PathFinding>();
            Worker worker = obj.AddComponent<Worker>();

            worker.InitializeWorker(WorkerToSpawn, pathFinding);
            workerAI.Setup(worker);

            if (WorkerToSpawn.position != Vector2.zero)
            {
                //Old worker
                obj.transform.position = WorkerToSpawn.position;
                obj.transform.rotation = WorkerToSpawn.rotation;
            }
            else
            {
                //New worker
                obj.transform.position = spawnPoint;
                obj.transform.rotation = Quaternion.identity;

                Vector3 pos = FindObjectOfType<MapLayoutManager>().NodeFromWorldPosition(Vector3.zero).position;

                obj.transform.position = pos;

                /*
                TaskSystem.Task comeToview = new TaskSystem.Task
                {
                    moveToPosition = new TaskSystem.Task.MoveTo(pos, Random.Range(0, 2f))
                };

                workerAI.taskManager.tasks.Enqueue(comeToview);
                */
            }

            return obj;
        }

        public static GameObject GetObject(SaveManager.SavableObject.WorldObject data)
        {
            GameObject obj = new GameObject(data.name);

            SpriteRenderer spr = obj.AddComponent<SpriteRenderer>();
            spr.sprite = data.sprite;
            spr.sortingOrder = data.spriteSortingLayer;

            Color savedColor = data.color;
            Debug.Log(savedColor);
            spr.color = savedColor;

            obj.transform.position = data.position;
            obj.transform.rotation = data.rotation;

            obj.AddComponent<PlacedObject>();

            switch (data.objectType)
            {
                case PlacedObjectType.Desk:
                    obj.AddComponent<WorkerButton>();

                    Transform child = new GameObject("Chair").transform;

                    child.SetParent(obj.transform);

                    child.position = (data.childPos != Vector2.zero) ? child.position = data.childPos : obj.transform.position;

                    obj.GetComponent<WorkerButton>().chair = child;
                    break;
                case PlacedObjectType.Need:
                    obj.AddComponent<NeedFulfiller>();
                    break;
                default:
                    break;
            }

            return obj;
        }
    }

    public interface ISaveableObject
    {
        void SetupSaveableObject();
    }
}