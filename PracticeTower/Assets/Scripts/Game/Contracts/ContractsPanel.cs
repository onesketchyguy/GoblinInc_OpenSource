using LowEngine.Saving;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace LowEngine
{
    public class ContractsPanel : MonoBehaviour
    {
        public Toggle ToggleAvailable;
        private bool accepting = true;

        public Text ContextButtonText;
        public Button contextButton;

        [Header("Active")]
        public GameObject availableRegion;

        public static List<Contract> currentContracts = new List<Contract>() { };

        public Transform HiredSpawningParent;

        [Header("Available")]
        public GameObject hiringRegion;

        public Sprite DeclineIcon;
        public Sprite AcceptIcon;

        private List<Contract> todaysContracts = new List<Contract>() { };

        public void UpdateContracts()
        {
            //Remove old employees
            if (todaysContracts.Count > 0)
            {
                var applicantsToRemove = new List<Contract>() { };

                int ran = Random.Range(0, todaysContracts.Count);

                for (int i = 0; i < ran; i++)
                {
                    if (todaysContracts[i] != null)
                    {
                        applicantsToRemove.Add(todaysContracts[i]);
                    }
                }

                while (applicantsToRemove.Count > 0)
                {
                    todaysContracts.Remove(todaysContracts[0]);
                    applicantsToRemove.Remove(applicantsToRemove[0]);
                }
            }
        }

        private void Start()
        {
            ToggleAvailable.onValueChanged.AddListener(ToggleHiringValue);
            TimeManagement.TimeScale.HourChanged += UpdateContracts;
        }

        public void ToggleHiringValue(bool newVal)
        {
            accepting = newVal;

            UpdateView();
        }

        private void OnEnable()
        {
            UpdateView();
        }

        private void UpdateView()
        {
            ContextButtonText.text = (accepting) ? "Hire worker" : "Fire worker";
            contextButton.image.sprite = (accepting) ? AcceptIcon : DeclineIcon;

            contextButton.onClick.RemoveAllListeners();

            hiringRegion.SetActive(accepting);
            availableRegion.SetActive(!accepting);

            if (accepting)
            {
                contextButton.onClick.AddListener(() => AcceptContract());
                UpdateAvailableContracts();
            }
            else
            {
                contextButton.onClick.AddListener(() => CancelContract());
                UpdateCurrentContracts();
            }
        }

        private void UpdateAvailableContracts()
        {
            throw new System.NotImplementedException();
        }

        private void UpdateCurrentContracts()
        {
            throw new System.NotImplementedException();
        }

        private void DestroyChildren(Transform parent)
        {
            foreach (var item in parent.GetComponentsInChildren<Transform>())
            {
                if (item.transform == parent) continue;

                Destroy(item.gameObject);
            }
        }

        private void Display(string showText)
        {
            throw new System.NotImplementedException();
        }

        public void AcceptContract()
        {
            throw new System.NotImplementedException();
        }

        public void CancelContract()
        {
            throw new System.NotImplementedException();
        }
    }
}