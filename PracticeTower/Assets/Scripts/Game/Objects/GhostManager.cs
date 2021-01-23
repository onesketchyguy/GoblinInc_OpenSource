using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LowEngine.Tasks
{
    public class GhostManager : MonoBehaviour
    {
        private static List<GhostObject> Ghosts;

        private static int currentIndex;

        private static int firstGhost
        {
            get
            {
                if (currentIndex >= Ghosts.Count - 1)
                {
                    currentIndex = 0;
                }
                else
                {
                    currentIndex++;
                }

                return currentIndex;
            }
            set
            {
                currentIndex = value;
            }
        }

        public static GhostObject GetGhost(Saving.SaveManager.SavableObject.WorldObject placing)
        {
            foreach (var item in Ghosts)
            {
                if (item.gameObject.activeSelf == false)
                {
                    item.ghostData.placing = placing;
                    item.gameObject.SetActive(true);

                    return item;
                }
            }

            return Ghosts[firstGhost];
        }

        private void Start()
        {
            Ghosts = new List<GhostObject>();

            foreach (var Ghost in GetComponentsInChildren<GhostObject>())
            {
                Ghosts.Add(Ghost);
                Ghost.gameObject.SetActive(false);
            }
        }
    }
}