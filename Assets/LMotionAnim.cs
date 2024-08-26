using LitMotion;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class LMotionAnim : ScriptableObject
{
    [Tooltip(".WithDelay(CHARINDEX * delay)")]
    public float delay;
    [Tooltip(".WithEase(charEasingArray[CHARINDEX])")]
    public Ease easing;
    [Tooltip("LMotion.Create(_, _, motionTime)")]
    public float motionTime;
    [Tooltip("LMotion.Create(colorPairs.A, colorPairs.B, _).BindToTMPCharColor(TEXT, CHARINDEX)")]
    public ColorPair colorPair;
    [Tooltip("Sets to use LMotion.Punch.Create instead of LMotion.Create")]
    public bool usePunch;
    [Tooltip("LMotion.Create(vector2Pairs.A, vector2Pairs.B, _).BindToTMPCharPosition(TEXT, CHARINDEX)")]
    public Vector3Pair vectorPair;
}
[Serializable]
public struct ColorPair
{
    public Color A;
    public Color B;
}
[Serializable]
public struct Vector3Pair
{
    public Vector3 A;
    public Vector3 B;
}