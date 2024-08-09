using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteColorDepth : MonoBehaviour
{
    SpriteRenderer[] spriteRenderers;
    Color[] originalColor;
    void Start()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        originalColor = new Color[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            originalColor[i] = spriteRenderers[i].color;
        }
    }
    void Update()
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].color = originalColor[i] * Level.LevelDepthGradient.Evaluate(Level.ins.GetDepthValueAtPoint(spriteRenderers[i].transform.position));
        }
    }
}
