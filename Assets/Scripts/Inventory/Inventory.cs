using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CameraShake;

public class Inventory : MonoBehaviour
{
    public float PickupRadius;
    public Entry[] AvailableEntries;

    public Rigidbody2D PlayerRigidbody;
    public AudioSource AudioSource;
    public AudioClip PickupSound;
    public float HorizontalDropForce;
    public float VerticalDropForce;
    public float DropTorque;


    // Private / Hidden variables..
    private Entry ActiveEntry;



    private void Update()
    {
        for (int I = 0; I < AvailableEntries.Length; I++)
        {
            AvailableEntries[I].WeaponObject.SetActive(AvailableEntries[I] == ActiveEntry);
        }


        Collider2D[] PickupColliders = Physics2D.OverlapCircleAll(this.transform.position, PickupRadius);
        List<Pickup> Pickups = new List<Pickup>();
        for (int I = 0; I < PickupColliders.Length; I++)
        {
            if (PickupColliders[I].TryGetComponent<Pickup>(out Pickup FoundPickup)) {
                Pickups.Add(FoundPickup);
            }
        }
        

        Pickup Pickup = null;
        for (int I = 0; I < Pickups.Count; I++)
        {
            if (Pickup == null || Vector3.Distance(this.transform.position, Pickups[I].transform.position) < Vector3.Distance(this.transform.position, Pickup.transform.position))
            {
                Pickup = Pickups[I];
            }
        }


        if (Input.GetKeyDown(KeyCode.E) && Pickup != null) {
            this.Pickup(Pickup);
        }
    }

    public void Pickup(Pickup Pickup)
    {
        Entry NewActiveEntry = null;
        for (int I = 0; I < AvailableEntries.Length; I++)
        {
            if (AvailableEntries[I].Identifier == Pickup.Identifier)
            {
                NewActiveEntry = AvailableEntries[I];
                break;
            }
        }

        if (NewActiveEntry != null)
        {
            this.Drop(true);

            if (CameraShaker.Instance != null) {
                CameraShaker.Instance.ShakeOnce((4.5f * (float.Parse((Settings.Get("Screenshake Intensity", 1f).ToString())))), 0f, 0f, 0.65f);
            }

            ActiveEntry = NewActiveEntry;
            AudioSource.PlayOneShot(PickupSound);
            Pickup.Collect();
        }
    }

    public void Drop(bool Throw)
    {
        if (ActiveEntry != null && ActiveEntry.PickupPrefab != null && ActiveEntry.DropPoint != null)
        {
            GameObject DroppedPickup = Instantiate(ActiveEntry.PickupPrefab, ActiveEntry.DropPoint.position, ActiveEntry.DropPoint.rotation);
            Rigidbody2D PickupRigidbody = DroppedPickup.GetComponent<Rigidbody2D>();
            if (PickupRigidbody != null)
            {
                PickupRigidbody.velocity = PlayerRigidbody.velocity;
                PickupRigidbody.angularVelocity = PlayerRigidbody.angularVelocity;

                if (Throw) {
                    PickupRigidbody.AddRelativeForce(new Vector2(HorizontalDropForce, 0f));
                    PickupRigidbody.AddForce(new Vector2(0f, VerticalDropForce));
                    PickupRigidbody.AddTorque(DropTorque);
                }
            }

            if (ActiveEntry.WeaponObject.transform.localScale.y < 0f)
            {
                DroppedPickup.transform.localScale = new Vector3(-DroppedPickup.transform.localScale.x, DroppedPickup.transform.localScale.y, DroppedPickup.transform.localScale.z);
                DroppedPickup.transform.eulerAngles = new Vector3(DroppedPickup.transform.eulerAngles.x, DroppedPickup.transform.eulerAngles.y, (DroppedPickup.transform.eulerAngles.z + 180));
            }


            ActiveEntry = null;
        }
    }




    [System.Serializable] public class Entry
    {
        public string Identifier;
        public GameObject WeaponObject;
        public GameObject PickupPrefab;
        public Transform DropPoint;
    }
}
