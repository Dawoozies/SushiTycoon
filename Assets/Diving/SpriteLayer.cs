using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[ExecuteInEditMode]
public class SpriteLayer : MonoBehaviour
{
    [SerializeField] int layer;
    bool updateLayers;
    int[] originalLayers;
    SpriteRenderer[] spriteRenderers;
    private void Start()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        originalLayers = new int[spriteRenderers.Length];
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            originalLayers[i] = spriteRenderers[i].sortingOrder;
            spriteRenderers[i].sortingOrder += layer;
        }
    }
    private void Update()
    {
        if(updateLayers)
        {
            UpdateLayers();
        }
    }
    void UpdateLayers()
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].sortingOrder = originalLayers[i] + layer;
        }
        updateLayers = false;
    }
    public void SetLayer(int layer)
    {
        this.layer = layer;
        updateLayers = true;
    }
    [SerializeField] int spriteLayerShift = 5;
    [ContextMenu("ShiftLayers")]
    public void ShiftLayers()
    {
        if (spriteRenderers == null)
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        foreach (var spriteRenderer in spriteRenderers)
        {
            spriteRenderer.sortingOrder += spriteLayerShift;
        } 
    }
}
