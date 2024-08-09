using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Collectable : MonoBehaviour, ICollectable
{
    public bool collected => collectionProgress > collectionTime;
    [SerializeField] Transform collectableGraphic;
    public Transform graphic => collectableGraphic;
    public Vector2 position => transform.position;

    [SerializeField] float collectionTime;
    float collectionProgress;
    public void Collect(float collectSpeed)
    {
        collectionProgress += collectSpeed * Time.deltaTime;
    }
}
public interface ICollectable
{
    public bool collected { get; }
    public void Collect(float collectSpeed);
    public Transform graphic { get; }
    public Vector2 position { get; }
}