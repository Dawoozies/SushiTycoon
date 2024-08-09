using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level : MonoBehaviour
{
    public static Level ins;
    void Awake()
    {
        ins = this;
    }
    public static Vector2 LevelDepthBounds;
    [SerializeField] Vector2 levelDepthBounds;

    public static Gradient LevelDepthGradient;
    [SerializeField] Gradient levelDepthGradient;
    void Update()
    {
        Vector3 levelCenter = transform.position;
        LevelDepthBounds = levelDepthBounds + (Vector2)levelCenter;

        LevelDepthGradient = levelDepthGradient;
    }
    public float GetDepthValueAtPoint(Vector3 p)
    {
        return Mathf.InverseLerp(LevelDepthBounds.x, LevelDepthBounds.y, p.y);
    }
}
