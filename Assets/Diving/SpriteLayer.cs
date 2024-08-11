using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class SpriteLayer : MonoBehaviour
{
    [SerializeField] int layer;
    int _layer;
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
        if (_layer != layer)
        {
            _layer = layer;
            UpdateLayers();
        }
    }
    void UpdateLayers()
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].sortingOrder = originalLayers[i] + _layer;
        }
    }
    public void SetLayer(int layer)
    {

    }
}
