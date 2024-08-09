using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Entity : MonoBehaviour
{
    [SerializeField] Transform graphicParent;
    NavMeshAgent agent;
    SpriteRenderer[] spriteRenderers;
    public Color entityColor;
    protected virtual void Start()
    {
        spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
        agent = GetComponent<NavMeshAgent>();
    }
    protected virtual void UpdateColor()
    {
        Color color = entityColor * Level.LevelDepthGradient.Evaluate(Level.ins.GetDepthValueAtPoint(transform.position));
        foreach (SpriteRenderer spriteRenderer in spriteRenderers)
        {
            spriteRenderer.color = color;
        }
    }
    protected virtual void Update()
    {
        UpdateColor();
        Vector3 localScale = graphicParent.transform.localScale;
        if (agent.velocity.x > 0)
        {
            localScale.x = -1;
        }
        if(agent.velocity.x < 0)
        {
            localScale.x = 1;
        }
        graphicParent.transform.localScale = localScale;
    }
}
