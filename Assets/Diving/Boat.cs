using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using uPools;
public class Boat : MonoBehaviour
{
    List<ICollectable> bag = new();
    Rigidbody2D rb;
    [SerializeField] float acceleration;
    [SerializeField] float brakePercentage;
    [SerializeField] float maximumSpeed;
    Vector2 velocity;
    [SerializeField] Transform collectionPoint;
    [SerializeField] float collectionPointRadius;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    private void FixedUpdate()
    {
        float horizontalInput = Input.GetAxis("Horizontal");
        if (Mathf.Abs(horizontalInput) > 0.01f)
        {
            Vector2 v = Vector2.right * horizontalInput * acceleration;
            rb.AddForce(v);
        }
        else
        {
            rb.velocity += -rb.velocity * brakePercentage * Time.fixedDeltaTime;
        }
        Vector2 vel = rb.velocity;
        vel.x = Mathf.Clamp(vel.x, -maximumSpeed, maximumSpeed);
        rb.velocity = vel;
    }
    private void OnTriggerStay2D(Collider2D collider)
    {
        if(collider.CompareTag("Diver"))
        {
            Diver diver = collider.GetComponentInParent<Diver>();
            if(diver != null)
            {
                TakeCollectableFromDiverBag(diver);
            }
        }
    }
    void TakeCollectableFromDiverBag(Diver diver)
    {
        ICollectable collectable = diver.TakeCollectableFromBag();
        if(collectable != null) 
        {
            Vector3 p = collectionPoint.position + (Vector3)Random.insideUnitCircle * collectionPointRadius;
            collectable.BoatCollect(p);
            bag.Add(collectable);
        }
    }
    public ICollectable TakeCollectableFromBoat()
    {
        if (bag.Count == 0)
            return null;
        ICollectable collectable = bag[0];
        bag.RemoveAt(0);
        return collectable;
    }
    public int CollectablesOnBoat()
    {
        return bag.Count;
    }
    private void OnDrawGizmosSelected()
    {
        if (collectionPoint == null)
            return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(collectionPoint.position, collectionPointRadius);
    }
}