using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using uPools;
public class BuiltObject : MonoBehaviour, IBuiltObject, IPoolCallbackReceiver
{
    public GameObject buildObjectInstance => gameObject;
    public ObjectBuilder builder => _builder;
    ObjectBuilder _builder;
    public SpriteLayer spriteLayer => _spriteLayer;
    SpriteLayer _spriteLayer;

    public UnityEvent onBuild;
    public UnityEvent onReturnToPool;

    public BoxCollider2D boxCollider;
    Collider2D[] overlapResults;
    public LayerMask mustOverlap;
    public LayerMask mustNotOverlap;

    Vector2 boxSize;
    Vector2 boxOffset;
    public void OnRent()
    {
        _spriteLayer = GetComponentInChildren<SpriteLayer>();
    }
    public void OnReturn()
    {
        onReturnToPool?.Invoke();
    }
    public void SetBuilder(ObjectBuilder builder)
    {
        _builder = builder;
    }
    public virtual void Build()
    {
        onBuild?.Invoke();
    }
    public virtual void CheckOverlaps(out bool isOnMustOverlap, out bool isOnMustNotOverlap)
    {
        isOnMustOverlap = false;
        isOnMustNotOverlap = false;
        boxSize = boxCollider.size;
        boxSize.Scale(boxCollider.transform.lossyScale);
        boxOffset = boxCollider.offset;
        boxOffset.Scale(boxCollider.transform.lossyScale);

        overlapResults = Physics2D.OverlapBoxAll(transform.position + (Vector3)boxOffset, boxSize, transform.localEulerAngles.z, mustNotOverlap);
        if(overlapResults.Length > 0)
        {
            isOnMustNotOverlap = true;
        }
        if(mustOverlap.value > 0)
        {
            Collider2D overlappingCollider = Physics2D.OverlapBox(transform.position + (Vector3)boxOffset, boxSize, transform.localEulerAngles.z, mustOverlap);
            if (overlappingCollider != null)
            {
                isOnMustOverlap = true;
            }
        }
        else
        {
            isOnMustOverlap = true;
        }
    }
    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
            return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(transform.position + (Vector3)boxOffset, boxSize);
    }
    public void Remove()
    {
        builder.RemoveObject((IBuiltObject)this);
    }
}

//walls -- grid
//tiling -- grid
//furniture -- free