using UnityEngine;

public class ScaleTween : MonoBehaviour
{
    public Vector3 scaleTo = new Vector3(1, 1, 1);

    public LeanTweenType curveType = LeanTweenType.linear;
    public AnimationCurve curve;

    public float time = 1;
    public float stopTime = 1;

    private Vector3 startScale;

    public bool animateOnStart = true;

    // Start is called before the first frame update
    private void Start()
    {
        startScale = transform.localScale;

        if (animateOnStart) Animate();
    }

    public void Animate()
    {
        if (curveType == LeanTweenType.animationCurve)
            LeanTween.scale(gameObject, scaleTo, time).setEase(curve).setLoopPingPong();
        else LeanTween.scale(gameObject, scaleTo, time).setEase(curveType).setLoopPingPong();
    }

    public void AnimateOnce()
    {
        if (curveType == LeanTweenType.animationCurve)
            LeanTween.scale(gameObject, scaleTo, time).setEase(curve).setOnComplete(() => StopAnimate());
        else LeanTween.scale(gameObject, scaleTo, time).setEase(curveType).setOnComplete(() => StopAnimate());
    }

    public void StopAnimate()
    {
        LeanTween.cancel(gameObject);

        LeanTween.scale(gameObject, startScale, stopTime).setEase(curveType);
    }
}