using System.Collections;
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
    public float HorizontalKnockbackMultiplier;
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

    // Private / Hidden variables..
    [HideInInspector] public int InitialHealth;
    private bool IsDead;
    private int LastHealth;
    private bool WasDamaged;
    private bool Stunned;


    private void Awake()
    {
        PlayerRenderer.color = PlayerColorPalette[Random.Range(0, PlayerColorPalette.Length)];
        InitialHealth = Health;
    }

    private void Update()
    {
        if (!PauseMenu.Instance.IsPaused)
        {
            if (Input.GetKeyDown(KeyCode.V)) PlayerRenderer.color = PlayerColorPalette[Random.Range(0, PlayerColorPalette.Length)];
            if (Input.GetKeyDown(KeyCode.B)) this.Damage((this.InitialHealth / 10), new Vector2(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y));

            float HurtWeightTarget = (1f - (((float)Health) / ((float)InitialHealth)));
            HurtFXVolume.weight = Mathf.Lerp(HurtFXVolume.weight, HurtWeightTarget, Mathf.Clamp01((Time.deltaTime * 0.95f)));

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

                Inventory?.Drop(false);
                IsDead = true;


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

                Rigidbody.velocity += (KnockbackDirection * (Damage / 5) * HorizontalKnockbackMultiplier);


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

    public IEnumerator Respawn()
    {
        for (int I = 0; I < PlayerControllerObjects.Length; I++)
        {
            PlayerControllerObjects[I].SetActive(false);
        }

        for (int I = 0; I < ObjectsToDisable.Length; I++)
        {
            ObjectsToDisable[I].SetActive(false);
        }

        Rigidbody.simulated = false;

        yield return new WaitForSeconds(RespawnDelay);

        for (int I = 0; I < PlayerControllerObjects.Length; I++)
        {
            PlayerControllerObjects[I].SetActive(true);
        }

        Rigidbody.simulated = true;


        Rigidbody.velocity = Vector2.zero;
        Rigidbody.angularVelocity = 0f;

        Health = Mathf.Clamp((InitialHealth + ((int)((BaseHealthVariation * System.Convert.ToInt32((Settings.Get("Difficulty", 1).ToString()))) * Random.Range(-1f, 1f)))), 1, (InitialHealth + (BaseHealthVariation * System.Convert.ToInt32((Settings.Get("Difficulty", 1).ToString())))));
        PlayerRenderer.color = PlayerColorPalette[Random.Range(0, PlayerColorPalette.Length)];

        GameObject[] SpawnPoints = GameObject.FindGameObjectsWithTag("Spawn Point");
        if (SpawnPoints.Length > 0) this.transform.position = SpawnPoints[Random.Range(0, SpawnPoints.Length)].transform.position;

        CharacterController.enabled = true;
        IsDead = false;
    }

    public void OnCollisionStay2D(Collision2D collision)
    {
        if (Stunned) {
            CharacterController.enabled = true;
        }
    }
}
