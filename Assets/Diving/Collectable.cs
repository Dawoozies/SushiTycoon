using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
public class Collectable : MonoBehaviour, ICollectable
{
    public SpriteRenderer spriteRenderer;
    public BoxCollider2D boxCollider;
    public bool collected => collectionProgress > collectionTime;
    public Vector2 position => transform.position;
    public float weight => _weight;
    float _weight;
    [SerializeField] float collectionTime;
    float collectionProgress;
    public UnityEvent onSetCollectableData;
    public UnityEvent onCollect;
    public float progressPercentage { 
        get { return (collectionProgress / collectionTime)*100f; } 
    }

    public bool beingCollected { get => collectProgressThisFrame; }
    bool collectProgressThisFrame;

    bool inABag;
    public GameObject temporaryRigidbody { get { return _temporaryRigidbody; } }
    GameObject _temporaryRigidbody;
    public bool isHeld { get { return _isHeld; } }
    public CollectableData collectableData => _collectableData;
    CollectableData _collectableData;
    bool _isHeld;

    SpriteLayer spriteLayer;

    Action collectableReturnCallback;


    public void SetSpawnerReturnCallback(Action a)
    {
        collectableReturnCallback = a;
    }
    public void ReturnToSpawner()
    {
        Debug.Log($"Collecting {collectableData.name}");
        CollectionNotificationPool.ins.Request(_collectableData);
        collectableReturnCallback?.Invoke();
    }
    public void CollectProgress(float collectSpeed)
    {
        collectProgressThisFrame = true;
        collectionProgress += collectSpeed * Time.deltaTime;
    }
    public void SetCollectableData(CollectableData collectableData)
    {
        onSetCollectableData?.Invoke();
        _weight = collectableData.weight;
        collectionTime = collectableData.collectionTime;
        _collectableData = collectableData;
        collectionProgress = 0f;
        collectProgressThisFrame = false;
        _isHeld = false;
        inABag = false;

        spriteRenderer.sprite = collectableData.sprite;
        boxCollider.offset = collectableData.boxColliderOffset;
        boxCollider.size = collectableData.boxColliderSize;
    }
    public void Collect(Transform collectionParent, ref List<ICollectable> bag)
    {
        if (inABag)
            return;

        onCollect?.Invoke();
        transform.parent = collectionParent;
        bag.Add(this);
        inABag = true;
    }
    public void BoatCollect(Vector3 boatCollectPos)
    {
        Transform rigidBodyTransform;
        Vector2 boxSize = collectableData.boxColliderSize;
        boxSize.Scale(collectableData.entityBase.transform.localScale);
        TemporaryRigidbodyPool.ins.Request(transform, out rigidBodyTransform, boxSize);
        rigidBodyTransform.position = boatCollectPos;
        inABag = false;

        _temporaryRigidbody = rigidBodyTransform.gameObject;
    }
    public void DockWorkerCollect(Transform holdParent)
    {
        TemporaryRigidbodyPool.ins.Return(temporaryRigidbody);
        transform.parent = holdParent;
        transform.position = holdParent.position;
        _isHeld = true;
    }
    void LateUpdate()
    {
        collectProgressThisFrame = false;
    }


    public void SetSpriteLayer(int layer)
    {
        if (spriteLayer == null)
            return;
        spriteLayer.SetLayer(layer);
    }
}
public interface ICollectable
{
    public CollectableData collectableData { get; }
    public bool collected { get; }
    public void CollectProgress(float collectSpeed);
    public Vector2 position { get; }
    public float weight { get; }
    public void SetCollectableData(CollectableData collectableData);
    public void Collect(Transform collectionParent, ref List<ICollectable> bag);
    public void BoatCollect(Vector3 boatCollectPos);
    public void DockWorkerCollect(Transform holdParent);
    public float progressPercentage { get; }
    bool beingCollected { get; }
    public GameObject temporaryRigidbody { get; }
    public bool isHeld { get; }
    public void ReturnToSpawner();
    public void SetSpriteLayer(int layer);
}