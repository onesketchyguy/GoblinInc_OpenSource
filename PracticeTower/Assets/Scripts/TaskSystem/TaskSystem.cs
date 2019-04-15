using System.Collections.Generic;
using UnityEngine;

namespace LowEngine.Tasks
{
    public class TaskSystem
    {
        public class Task
        {
            public MoveTo moveToPosition;

            public System.Action executeAction;

            public System.Action executeActionRecurring;

            public class MoveTo : Task
            {
                public Vector3 targetPosition;

                public float stoppingDistance;

                public MoveTo(Vector3 targetPosition, float stoppingDistance)
                {
                    this.targetPosition = targetPosition;
                    this.stoppingDistance = stoppingDistance;
                }

                public MoveTo(Vector3 targetPosition, float stoppingDistance, System.Action executeAction)
                {
                    this.targetPosition = targetPosition;
                    this.stoppingDistance = stoppingDistance;
                    this.executeAction = executeAction;
                }
            }
        }

        public Queue<Task> tasks = new Queue<Task>() { };
    }
}