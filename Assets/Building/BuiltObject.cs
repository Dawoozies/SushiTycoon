using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using uPools;
public class BuiltObject : MonoBehaviour, IBuiltObject, IPoolCallbackReceiver
{
    public GameObject buildObjectInstance => gameObject;
    public int builderPrefabIndex;
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

    [SerializeField] Sprite objectSprite;
    public Sprite sprite => objectSprite;
    [SerializeField] string objectName;
    public string itemName => objectName;
    [SerializeField] float objectCost;
    public float itemCost => objectCost;
    public virtual void OnRent()
    {
        _spriteLayer = GetComponentInChildren<SpriteLayer>();
    }
    public virtual void OnReturn()
    {
        onReturnToPool?.Invoke();
    }
    public void SetBuilder(ObjectBuilder builder, int builderPrefabIndex)
    {
        _builder = builder;
        this.builderPrefabIndex = builderPrefabIndex;
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
    public virtual void Build()
    {
        onBuild?.Invoke();
        //send my data to BuiltObjects
        BuiltObjects.ins.AddToBuiltObjects(_builder, this);
    }
    public virtual void Remove()
    {
        builder.RemoveObject((IBuiltObject)this);
        RestaurantParameters.ins.SellItem(itemCost);
        //remove my data from BuiltObjects
        BuiltObjects.ins.RemoveFromBuiltObjects(this);
    }
}

//walls -- grid
//tiling -- grid
//furniture -- free