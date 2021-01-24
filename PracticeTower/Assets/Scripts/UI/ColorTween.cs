using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TopDownShooter
{
    public class ColorTween : MonoBehaviour
    {
        public Graphic graphic;
        public float time = 1;

        [Range(0, 1f)]
        public float minDistance = 0.01f;

        public Color colorA;
        public Color colorB;

        private bool goingToA;

        private Color target
        {
            get
            {
                return goingToA ? colorA : colorB;
            }
        }

        private void CheckSimilarities()
        {
            var compareA = graphic.color;
            var compareB = target;

            if (Vector4.Distance(new Vector4(compareA.r, compareA.g, compareA.b, compareA.a),
                new Vector4(compareB.r, compareB.g, compareB.b, compareB.a)) < minDistance)
            {
                goingToA = !goingToA;
            }
        }

        private void Start()
        {
            graphic.color = colorA;
            StartCoroutine(DoLoop());
        }

        private IEnumerator DoLoop()
        {
            while (true)
            {
                CheckSimilarities();

                yield return null;
                graphic.color = Color.Lerp(graphic.color, target, time * Time.deltaTime);
            }
        }
    }
}