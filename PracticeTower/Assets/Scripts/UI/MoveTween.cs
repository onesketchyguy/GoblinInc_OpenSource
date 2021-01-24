using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TopDownShooter
{
    public class MoveTween : MonoBehaviour
    {
        public Vector3 moveTo;

        public LeanTweenType curveType;
        public AnimationCurve curve;

        public float time = 1;
        public float stopTime = 1;

        private Vector3 startPosition;

        public bool animateOnStart = true;

        // Start is called before the first frame update
        private void Start()
        {
            startPosition = transform.position;

            if (animateOnStart) Animate();
        }

        public void Animate()
        {
            if (curveType == LeanTweenType.animationCurve)
                LeanTween.move(gameObject, transform.position + moveTo, time).setEase(curve).setLoopPingPong();
            else LeanTween.move(gameObject, transform.position + moveTo, time).setEase(curveType).setLoopPingPong();
        }

        public void StopAnimate()
        {
            LeanTween.cancel(gameObject);

            LeanTween.move(gameObject, startPosition, stopTime).setEase(curveType);
        }
    }
}