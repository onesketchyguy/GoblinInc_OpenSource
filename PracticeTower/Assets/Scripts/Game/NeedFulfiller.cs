using LowEngine.Saving;
using UnityEngine;
namespace LowEngine.Tasks.Needs
{
    public enum NeedDefinition { Hunger, Thirst }

    public class NeedFulfiller : MonoBehaviour, ISaveableObject
    {
        public NeedDefinition Fulfills;

        public void FulFillneed(IWorker worker)
        {
            worker.GetNeed(Fulfills).Fill();

            switch (Fulfills)
            {
                case NeedDefinition.Hunger:
                    AudioManager.instance.PlayEat(transform.position);
                    break;
                case NeedDefinition.Thirst:
                    AudioManager.instance.PlayDrink(transform.position);
                    break;
            }
        }

        public void SetupSaveableObject()
        {
            if (GetComponent<PlacedObject>())
            {
                GetComponent<PlacedObject>().thisObject = new Saving.SaveManager.SavableObject.WorldObject
                {
                    objectType = PlacedObjectType.Need,
                    fulFills = Fulfills,
                    position = transform.position,
                    rotation = transform.rotation,
                    name = $"{gameObject.name}.{transform.position}",
                    sprite = GetComponent<SpriteRenderer>().sprite,
                    color = Color.white
                };
            }
        }

        private void Update()
        {
            SetupSaveableObject();
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