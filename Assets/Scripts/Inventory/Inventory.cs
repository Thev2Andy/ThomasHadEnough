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

    // Properties..
    public bool HasWeapon { get { return ActiveEntry != null; } }



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


        if (Pickup != null)
        {
            PromptController.Instance.Show("Press <b>E</b> to pick up.", float.Epsilon);

            if (Input.GetKeyDown(KeyCode.E))
            {
                PromptController.Instance.Clear();
                this.Pickup(Pickup);
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && HasWeapon && !PauseMenu.Instance.IsPaused)
        {
            PromptController.Instance.Clear();
            this.Drop(true);
            AudioSource.PlayOneShot(PickupSound);
        }
    }

    public void Pickup(Pickup Pickup)
    {
        Entry NewActiveEntry = null;
        for (int I = 0; I < AvailableEntries.Length; I++)
        {
            for (int J = 0; J < AvailableEntries[I].ObjectsToDisableOnSwitch.Length; J++) {
                AvailableEntries[I].ObjectsToDisableOnSwitch[J].SetActive(false);
            }

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

                if (Throw)
                {
                    PickupRigidbody.AddRelativeForce(new Vector2(HorizontalDropForce, 0f));
                    PickupRigidbody.AddForce(new Vector2(0f, VerticalDropForce));


                    Vector2 PivotDifference = this.transform.position - ActiveEntry.DropPoint.position;
                    PivotDifference.y = 0f;
                    PivotDifference.Normalize();

                    float TorqueSign = -PivotDifference.x;
                    PickupRigidbody.AddTorque((DropTorque * TorqueSign));
                }
            }

            if (ActiveEntry.WeaponObject.transform.localScale.y < 0f)
            {
                DroppedPickup.transform.localScale = new Vector3(-DroppedPickup.transform.localScale.x, DroppedPickup.transform.localScale.y, DroppedPickup.transform.localScale.z);
                DroppedPickup.transform.eulerAngles = new Vector3(DroppedPickup.transform.eulerAngles.x, DroppedPickup.transform.eulerAngles.y, (DroppedPickup.transform.eulerAngles.z + 180));
            }

            for (int I = 0; I < AvailableEntries.Length; I++)
            {
                for (int J = 0; J < AvailableEntries[I].ObjectsToDisableOnSwitch.Length; J++) {
                    AvailableEntries[I].ObjectsToDisableOnSwitch[J].SetActive(false);
                }
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
        public GameObject[] ObjectsToDisableOnSwitch;
    }
}
