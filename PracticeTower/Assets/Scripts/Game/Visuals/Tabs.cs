using System.Collections.Generic;
using UnityEngine;

namespace LowEngine.UI
{
    public class Tabs : MonoBehaviour
    {
        public List<GameObject> tabs = new List<GameObject>();

        public void HideTabs()
        {
            foreach (var item in tabs) item.SetActive(false);
        }

        public void SetActiveTab(GameObject tab)
        {
            AddTab(tab);

            foreach (var item in tabs) item.SetActive(item == tab);
        }

        public void ToggleTab(GameObject tab)
        {
            AddTab(tab);

            foreach (var item in tabs)
            {
                var active = item == tab ? item.activeSelf == false : false;
                item.SetActive(active);
            }
        }

        public void AddTab(GameObject tab)
        {
            if (tabs.Contains(tab) == false)
                tabs.Add(tab);
        }
    }
}