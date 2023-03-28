using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickupResetTrigger : MonoBehaviour
{
    public LayerMask LineOfSightMask;
    public bool CheckForLineOfSight;

    private void OnTriggerStay2D(Collider2D Collision)
    {
        if (Collision.gameObject.layer == LayerMask.NameToLayer("Pickup"))
        {
            if (Physics2D.Raycast(this.transform.position, (Collision.gameObject.transform.position - this.transform.position), float.MaxValue, LineOfSightMask).transform == Collision.transform || !CheckForLineOfSight)
            {
                Rigidbody2D Rigidbody = Collision.gameObject.GetComponent<Rigidbody2D>();
                Rigidbody.velocity = Vector2.zero;
                Rigidbody.angularVelocity = 0f;

                GameObject[] SpawnPoints = GameObject.FindGameObjectsWithTag("Spawn Point");
                if (SpawnPoints.Length > 0) Rigidbody.transform.position = SpawnPoints[Random.Range(0, SpawnPoints.Length)].transform.position;
            }
        }
    }
}
