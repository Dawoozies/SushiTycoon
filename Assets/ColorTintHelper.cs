using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ColorTintHelper : MonoBehaviour
{
    public Color colorTint;
    SpriteRenderer[] spriteRenderers;
    Color[] originalColors;
    [ContextMenu("TintSprites")]
    public void TintSprites()
    {
        if (spriteRenderers == null)
        {
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                spriteRenderers[i].color *= colorTint;
            }
        }
    }
}
