using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class OnBuildingEvents : MonoBehaviour
{
    [SerializeField] BoxCollider2D buildBounds;
    public UnityEvent onSelectedForBuilding;
    public UnityEvent onBuild;
    SpriteRenderer[] spriteRenderers;
    Color[] originalColors;
    public Collider2D[] overlapResults;
    [SerializeField] Vector3 boxOffset;
    [SerializeField] Vector2 boxSize;
    public void SelectedForBuilding()
    {
        if (spriteRenderers == null)
        {
            spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            originalColors = new Color[spriteRenderers.Length];
            for (int i = 0; i < spriteRenderers.Length; i++)
            {
                originalColors[i] = spriteRenderers[i].color;
            }
        }
        onSelectedForBuilding?.Invoke();
    }
    public void Build()
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].color = originalColors[i];
        }
        onBuild?.Invoke();
    }
    public void WhileNotBuilt(Vector2 mouseWorldPos)
    {
        transform.position = mouseWorldPos;
    }
    void Update()
    {

    }
    public bool BuildingOverlapCheck()
    {
        boxSize = buildBounds.size;
        boxSize.Scale(buildBounds.transform.lossyScale);

        boxOffset = buildBounds.offset;
        boxOffset.Scale(buildBounds.transform.lossyScale);
        overlapResults = Physics2D.OverlapBoxAll(transform.position + boxOffset, boxSize, transform.localEulerAngles.z, RestaurantParameters.ins.BuildingLayerMask);
        return overlapResults.Length > 0;
    }
    public void ChangeColorTint(Color colorTint)
    {
        for (int i = 0; i < spriteRenderers.Length; i++)
        {
            spriteRenderers[i].color = originalColors[i] * colorTint;
        }
    }
    void OnDrawGizmos()
    {
        if(!Application.isPlaying)
        {
            return;
        }
        Gizmos.color = Color.yellow * 0.5f;
        Gizmos.DrawWireCube(buildBounds.transform.position + boxOffset, boxSize);
    }
}
