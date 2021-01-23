using LowEngine.Saving;
using UnityEngine;

namespace LowEngine.Tasks.Needs
{
    public enum NeedDefinition { Hunger, Thirst }

    public class NeedFulfiller : MonoBehaviour
    {
        public NeedDefinition Fulfills;

        public void FulFillneed(IWorker worker)
        {
            worker.GetNeed(Fulfills).Fill();

            switch (Fulfills)
            {
                case NeedDefinition.Hunger:
                    AudioManager.instance.PlaySound("Eating", transform.position);
                    break;

                case NeedDefinition.Thirst:
                    AudioManager.instance.PlaySound("Drink potion", transform.position);
                    break;
            }
        }
    }

    public class Need
    {
        public NeedDefinition thisNeed;

        public float value { get; private set; } = 100;

        public void Fill()
        {
            value = 100;
        }

        public void Modify(float val)
        {
            if (value > 0)
            {
                value += val;
            }
        }

        public void Set(float val)
        {
            value = val;
        }
    }
}