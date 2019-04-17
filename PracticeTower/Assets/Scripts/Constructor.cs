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
            Vector3 spawnPoint = FindObjectOfType<MapLayoutManager>().PlayAreaSize - Vector2.one;

            GameObject obj = new GameObject(WorkerToSpawn.name);

            TaskWorkerAI workerAI = obj.AddComponent<TaskWorkerAI>();
            Worker worker = obj.AddComponent<Worker>();

            worker.InitializeWorker(WorkerToSpawn);
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

                
                TaskSystem.Task comeToview = new TaskSystem.Task
                {
                    moveToPosition = new TaskSystem.Task.MoveTo(pos, Random.Range(0, 2f))
                };

                workerAI.taskManager.tasks.Enqueue(comeToview);
            }

            return obj;
        }

        public static GameObject GetObject(SaveManager.SavableObject.WorldObject data)
        {
            GameObject obj = new GameObject(data.name);

            SpriteRenderer spr = obj.AddComponent<SpriteRenderer>();
            spr.sprite = data.sprite;
            spr.sortingOrder = data.spriteSortingLayer;
            spr.material = GameHandler.instance.gameObjectMaterial;

            obj.AddComponent<BoxCollider2D>();

            Color savedColor = data.color;
            Debug.Log(savedColor);
            spr.color = savedColor;

            obj.transform.position = data.position;
            obj.transform.rotation = data.rotation;

            PlacedObject objectData = obj.AddComponent<PlacedObject>();

            objectData.objectData = data;

            switch (data.objectType)
            {
                case PlacedObjectType.Desk:
                    obj.AddComponent<WorkerButton>();

                    Transform child = new GameObject("Chair").transform;

                    child.SetParent(obj.transform);

                    child.localPosition = (data.childPos != Vector2.zero) ? child.position = data.childPos : obj.transform.position;

                    child.rotation = obj.transform.rotation;

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

        public static GameObject GetObject(SaveManager.SavableObject.WorldObject data, Transform parent)
        {
            GameObject obj = GetObject(data);

            obj.transform.SetParent(parent);

            return obj;
        }
    }

    public interface ISaveableObject
    {
        void SetupSaveableObject();
    }
}