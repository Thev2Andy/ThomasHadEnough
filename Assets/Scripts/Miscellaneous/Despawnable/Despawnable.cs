using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Despawnable : MonoBehaviour
{
    public float ColliderLifetime;
    public float TotalLifetime;
    public Collider2D Collider;

    public Vector2 RigidbodyForce;
    public float RigidbodyTorque;


    private IEnumerator Start()
    {
        yield return new WaitForSeconds(ColliderLifetime);
        Destroy(Collider);

        Rigidbody2D Rigidbody = this.GetComponent<Rigidbody2D>();
        Rigidbody.velocity = (RigidbodyForce * new Vector2(Random.Range(-1f, 1f), 1));
        Rigidbody.angularVelocity = (RigidbodyTorque * Random.Range(-1, 2));

        yield return new WaitForSeconds((TotalLifetime - ColliderLifetime));
        Destroy(this.gameObject);
    }
}
