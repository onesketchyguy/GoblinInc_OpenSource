#if UNITY_EDITOR // We don't want the build runtime to try to add this

using UnityEditor;
using UnityEngine.EventSystems;

namespace LowEngine
{
    public class SetupButtonControler : Editor
    {
        [MenuItem("Tools/Setup bouncy button")]
        public static void SetupBouncyButton()
        {
            var selected = Selection.activeGameObject;

            // Setup the scaler component
            var scaler = selected.GetComponent<ScaleTween>() ? selected.GetComponent<ScaleTween>() : selected.AddComponent<ScaleTween>();
            scaler.curveType = LeanTweenType.linear;
            scaler.scaleTo = UnityEngine.Vector3.one * 1.05f;
            scaler.time = scaler.stopTime = 0.1f;
            scaler.animateOnStart = false;

            // Setup the sound component
            var soundPlayer = selected.GetComponent<SoundPlayer>() ? selected.GetComponent<SoundPlayer>() : selected.AddComponent<SoundPlayer>();
            // FIXME: Setup audio clip

            // Setup hover events
            var eventHandler = selected.GetComponent<EventTrigger>() ? selected.GetComponent<EventTrigger>() : selected.AddComponent<EventTrigger>();

            // entry event system
            var entry = new EventTrigger.Entry()
            {
                eventID = EventTriggerType.PointerEnter,
            };

            entry.callback.AddListener(_ => soundPlayer.PlaySound());
            entry.callback.AddListener(_ => scaler.AnimateOnce());

            // Add that event to our event handler
            eventHandler.triggers.Add(entry);
        }
    }
}

#endif