using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSurfaceWaveWobble : MonoBehaviour
{
    [SerializeField] Transform graphic;
    [SerializeField] AnimationCurve xCurve;
    [SerializeField] AnimationCurve yCurve;
    Vector3 startLocalPosition;
    Vector3 p;
    [SerializeField] Vector2 xBounds;
    [SerializeField] Vector2 yBounds;
    float t;
    [SerializeField] Vector2 frequencyBounds;
    [SerializeField] AnimationCurve frequencyTargetCurve;
    float frequencyTarget;
    [SerializeField] float frequencySmoothTime;
    float frequency_v;
    float frequency;

    Quaternion startLocalRotation;
    Quaternion r;
    [SerializeField] Vector2 zRotBounds;
    private void Start()
    {
        startLocalPosition = graphic.localPosition;
        startLocalRotation = graphic.localRotation;
    }
    private void Update()
    {
        frequency = Mathf.SmoothDamp(frequency, frequencyTarget, ref frequency_v, frequencySmoothTime);
        frequencyTarget = Mathf.Lerp(frequencyBounds.x, frequencyBounds.y, frequencyTargetCurve.Evaluate(t));
        t += Time.deltaTime * frequency;

        p = startLocalPosition;
        p.x += Mathf.Lerp(xBounds.x, xBounds.y, xCurve.Evaluate(t));
        p.y += Mathf.Lerp(yBounds.x, yBounds.y, yCurve.Evaluate(t));

        r = startLocalRotation;
        r *= Quaternion.AngleAxis(Mathf.Lerp(zRotBounds.x, zRotBounds.y, xCurve.Evaluate(t)), Vector3.forward);

        graphic.localPosition = p;
        graphic.localRotation = r;
    }

}
