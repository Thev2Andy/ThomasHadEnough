﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using CameraShake;

public class HealthSystem : MonoBehaviour
{
    public GameObject RagdollPrefab;
    public SpriteRenderer PlayerRenderer;
    public Color[] PlayerColorPalette;
    public GameObject[] PlayerControllerObjects;
    public GameObject[] ObjectsToDisable;
    public float KnockbackMultiplier;
    public float RespawnDelay;
    public float StaticRagdollTorqueMultiplier;
    public float RagdollTorqueForce;
    public CharacterController2D CharacterController;
    public Rigidbody2D Rigidbody;
    public AudioSource AudioSource;
    public AudioClip DeathSound;
    public AudioClip HitSound;
    public Inventory Inventory;
    public Volume HurtFXVolume;
    public int BaseHealthVariation;
    public int Health;
    public bool StartAtSpawnpoint;

    // Private / Hidden variables..
    [HideInInspector] public int InitialHealth;
    [HideInInspector] public int LastHealth;
    [HideInInspector] public bool IsDead;
    [HideInInspector] public bool Stunned;
    private bool WasDamaged;
    private bool SoftlockDeath;


    private void Update()
    {
        if (!PauseMenu.Instance.IsPaused)
        {
            float HurtWeightTarget = (1f - (((float)Health) / ((float)InitialHealth)));
            HurtFXVolume.weight = Mathf.Lerp(HurtFXVolume.weight, HurtWeightTarget, (1 - Mathf.Exp(-0.95f * Time.deltaTime)));

            if (Health < LastHealth && !WasDamaged)
            {
                int Difference = LastHealth - Health;
                Health = LastHealth;
                this.Damage(Difference, Random.insideUnitCircle);
            }


            if (Health <= 0 && !IsDead)
            {
                GameObject RagdollInstance = Instantiate(RagdollPrefab, this.transform.position, this.transform.rotation);
                RagdollInstance.GetComponent<SpriteRenderer>().color = PlayerRenderer.color;

                Rigidbody2D RagdollRigidbody = RagdollInstance.GetComponent<Rigidbody2D>();
                RagdollRigidbody.velocity = Rigidbody.velocity;
                RagdollRigidbody.angularVelocity = Rigidbody.angularVelocity;
                RagdollRigidbody.AddTorque((RagdollTorqueForce * -((!float.IsNaN((RagdollRigidbody.velocity.x / Mathf.Abs(RagdollRigidbody.velocity.x))) ? (RagdollRigidbody.velocity.x / Mathf.Abs(RagdollRigidbody.velocity.x)) : 1f))) * ((RagdollRigidbody.velocity.x == 0f) ? (StaticRagdollTorqueMultiplier * ((Random.Range(-1, 2) >= 0) ? 1 : -1)) : 1f));


                SoftlockDeath = ((Inventory != null && Inventory?.HasWeapon != null) ? !Inventory.HasWeapon : false);

                PromptController.Instance.Clear();
                Inventory?.Drop(false);
                IsDead = true;



                for (int I = 0; I < PlayerControllerObjects.Length; I++) {
                    PlayerControllerObjects[I].SetActive(false);
                }

                for (int I = 0; I < ObjectsToDisable.Length; I++) {
                    ObjectsToDisable[I].SetActive(false);
                }

                Rigidbody.simulated = false;
                this.StartCoroutine(Respawn());
            }


            LastHealth = Health;
            WasDamaged = false;
        }
    }

    public void Damage(int Damage, Vector2? DamageLocation = null)
    {
        if (!PauseMenu.Instance.IsPaused)
        {
            if (!IsDead)
            {
                DamageLocation = ((DamageLocation != null) ? DamageLocation : new Vector2(this.transform.position.x, this.transform.position.y));
                Vector2 KnockbackDirection = new Vector2(this.transform.position.x, this.transform.position.y) - new Vector2((((Vector2)DamageLocation).x), (((Vector2)DamageLocation).y));
                KnockbackDirection.Normalize();

                Rigidbody.velocity += (KnockbackDirection * (Damage / 5) * KnockbackMultiplier);


                if (CharacterController.enabled) {
                    CharacterController.enabled = false;
                    Stunned = true;
                }

                WasDamaged = true;
                Health -= Damage;

                if (CameraShaker.Instance != null) {
                    CameraShaker.Instance.ShakeOnce((9.5f * (Damage / 100f) * (float.Parse((Settings.Get("Screenshake Intensity", 1f).ToString())))), 1.95f, 0.15f, 0.85f, (Vector3.one * 0.15f), Vector3.one);
                }

                AudioSource.PlayOneShot(((Health <= 0) ? DeathSound : HitSound));
            }
        }
    }

    public IEnumerator Respawn(float RespawnDelayOverride = float.NaN)
    {
        yield return new WaitForSeconds(((!float.IsNaN(RespawnDelayOverride)) ? RespawnDelayOverride : RespawnDelay));

        StartCoroutine(this.TeleportToRandomSpawnpoint());

        for (int I = 0; I < PlayerControllerObjects.Length; I++)
        {
            PlayerControllerObjects[I].SetActive(true);
        }

        if (SoftlockDeath)
        {
            GameObject SoftlockPickupObject = new GameObject("Softlock Pickup");
            Pickup SoftlockPickup = SoftlockPickupObject.AddComponent<Pickup>();
            SoftlockPickup.Identifier = Inventory.AvailableEntries[0].Identifier;

            Inventory.Pickup(SoftlockPickup);
            SoftlockDeath = false;
        }

        Health = Mathf.Clamp((InitialHealth + ((int)((BaseHealthVariation * System.Convert.ToInt32((Settings.Get("Difficulty", 1).ToString()))) * Random.Range(-1f, 1f)))), 1, (InitialHealth + (BaseHealthVariation * System.Convert.ToInt32((Settings.Get("Difficulty", 1).ToString())))));
        PlayerRenderer.color = PlayerColorPalette[Random.Range(0, PlayerColorPalette.Length)];

        Rigidbody.simulated = true;

        Rigidbody.velocity = Vector2.zero;
        Rigidbody.angularVelocity = 0f;

        this.Stun();

        IsDead = false;
    }

    public IEnumerator TeleportToRandomSpawnpoint(bool WaitOneFrame = false)
    {
        if (WaitOneFrame) {
            yield return null;
        }

        Rigidbody.velocity = Vector2.zero;
        Rigidbody.angularVelocity = 0f;

        GameObject[] SpawnPoints = GameObject.FindGameObjectsWithTag("Spawn Point");
        if (SpawnPoints.Length > 0) this.transform.position = SpawnPoints[Random.Range(0, SpawnPoints.Length)].transform.position;
    }

    public void Stun()
    {
        if (CharacterController.enabled) {
            CharacterController.enabled = false;
            Stunned = true;
        }
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        if (Stunned) {
            CharacterController.enabled = true;
            Stunned = false;
        }
    }

    private void Start()
    {
        if (StartAtSpawnpoint) {
            StartCoroutine(this.TeleportToRandomSpawnpoint());
        }

        this.Stun();
    }


    private void Awake()
    {
        PlayerRenderer.color = PlayerColorPalette[Random.Range(0, PlayerColorPalette.Length)];
        Health = Mathf.Max(Health, 1);
        InitialHealth = Health;

        // TODO: We shouldn't really do this, it absolutely breaks the development workflow and makes it impossible for some stuff to work (e.g. a level selection system).
        // One way to fix this would be to use a singleton, set the index we wish to load, then load it if there is actually an index there.
        // Absolutely don't do this, don't hardcode scene build indices in your code.
        SceneLoader.Instance?.StartLoadingLevel(2);
    }
}
