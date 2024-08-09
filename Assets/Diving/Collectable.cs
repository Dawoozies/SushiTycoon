using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class Collectable : MonoBehaviour, ICollectable
{
    public bool collected => collectionProgress > collectionTime;
    public Vector2 position => transform.position;
    public float weight => _weight;
    float _weight;
    [SerializeField] float collectionTime;
    float collectionProgress;
    public UnityEvent onSetCollectableData;
    public UnityEvent onCollect;
    public void CollectProgress(float collectSpeed)
    {
        collectionProgress += collectSpeed * Time.deltaTime;
    }
    public void SetCollectableData(CollectableData collectableData)
    {
        onSetCollectableData?.Invoke();
        _weight = collectableData.weight;
        collectionTime = collectableData.collectionTime;
        GameObject entityBase = Instantiate(collectableData.entityBase, transform);
        entityBase.transform.localPosition = Vector3.zero;
    }
    public void Collect(Transform collectionParent)
    {
        onCollect?.Invoke();
        transform.parent = collectionParent;
    }
}
public interface ICollectable
{
    public bool collected { get; }
    public void CollectProgress(float collectSpeed);
    public Vector2 position { get; }
    public float weight { get; }
    public void SetCollectableData(CollectableData collectableData);
    public void Collect(Transform collectionParent);
}