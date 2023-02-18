using CameraShake;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [Header("Enemy Settings")]
    public LookAt2D WeaponRotator;
    public Rigidbody2D Rigidbody;
    public Collider2D Collider;
    public SpriteRenderer EnemyRenderer;
    public Color[] EnemyColorPalette;
    public GameObject[] ObjectsToDestroy;
    public AudioSource EnemyAudioSource;
    public AudioClip HitSound;
    public AudioClip DeathSound;
    public GameObject WeaponObject;
    public float RagdollColliderLifetime;
    public float RagdollTotalLifetime;
    public int Health;
    public float Range;
    public float InitialStaticRagdollTorqueMultiplier;
    public float InitialRagdollTorque;
    public Vector2 DespawningRagdollForce;
    public float DespawningRagdollTorque;

    [Header("Drop Settings")]
    public GameObject PickupPrefab;
    public Transform DropPoint;

    [Header("Weapon Settings")]
    public GameObject MuzzleFlash;
    public AudioClip ShootSound;
    public float WeaponRange;
    public int BaseDamage;
    public LayerMask LayerMask;
    public float ImpactForce;
    public GameObject ImpactEffect;
    public int ImpactParticleCount;
    public float ImpactVerticalOffset;
    public Transform FirePoint;
    public AudioSource WeaponAudioSource;
    public float RateOfFireInRPM;

    [Header("Weapon Recoil Settings")]
    public float Magnitude;
    public float Roughness;
    public float FadeInTime;
    public float FadeOutTime;

    // Private / Hidden variables..
    private float WeaponShotTimer;
    private int LastHealth;
    private bool WasDamaged;


    private void Update()
    {
        if (!PauseMenu.Instance.IsPaused)
        {
            Collider2D[] Colliders = Physics2D.OverlapCircleAll(this.transform.position, Range);
            Transform Player = null;
            for (int I = 0; I < Colliders.Length; I++)
            {
                if (Colliders[I].gameObject.CompareTag("Player"))
                {
                    Player = Colliders[I].transform;
                    break;
                }
            }

            if (WeaponShotTimer > 0) {
                WeaponShotTimer -= Time.deltaTime;
            }

            if (Health < LastHealth && !WasDamaged)
            {
                int Difference = LastHealth - Health;
                Health = LastHealth;
                this.Damage(Difference);
            }

            if (Player != null)
            {
                Collider.enabled = false;
                RaycastHit2D PlayerRaycast = Physics2D.Raycast(this.transform.position, (Player.transform.position - this.transform.position), Range);
                Collider.enabled = true;

                if (PlayerRaycast.transform == Player)
                {
                    WeaponRotator.PointTorwards(Player.transform.position, 0f, true);

                    if (WeaponShotTimer <= 0f)
                    {
                        RaycastHit2D Hit = Physics2D.Raycast(FirePoint.position, FirePoint.up, Range, LayerMask);
                        WeaponShotTimer = 60 / RateOfFireInRPM;


                        if (Hit)
                        {
                            ParticleSystem BulletImpact = Instantiate(ImpactEffect, (Hit.point + (Hit.normal * ImpactVerticalOffset)), Quaternion.FromToRotation(Vector3.up, Hit.normal)).GetComponent<ParticleSystem>();
                            BulletImpact.Emit(ImpactParticleCount);

                            if (Hit.rigidbody != null)
                            {
                                Hit.rigidbody.AddForceAtPosition(-(Hit.normal * ImpactForce), Hit.point);

                                ParticleSystem.Particle[] Particles = new ParticleSystem.Particle[ImpactParticleCount];
                                BulletImpact.GetParticles(Particles, ImpactParticleCount);
                                for (int I = 0; I < Particles.Length; I++)
                                {
                                    Particles[I].velocity += new Vector3(Hit.rigidbody.velocity.x, Hit.rigidbody.velocity.y, 0f);
                                    Particles[I].angularVelocity = Hit.rigidbody.angularVelocity;
                                }

                                BulletImpact.SetParticles(Particles, Particles.Length);
                            }

                            if (Hit.transform.gameObject.TryGetComponent<HealthSystem>(out HealthSystem PlayerHS)) {
                                PlayerHS.Damage((BaseDamage * Mathf.Clamp(((int.Parse((Settings.Get("Difficulty", 1).ToString()))) + 1), 1, int.MaxValue)), this.transform.position);
                            }

                            if (Hit.transform.gameObject.TryGetComponent<EnemyAI>(out EnemyAI Enemy)) {
                                Enemy.Damage((BaseDamage * Mathf.Clamp(((int.Parse((Settings.Get("Difficulty", 1).ToString()))) + 1), 1, int.MaxValue)));
                            }
                        }

                        CameraShaker.Instance.ShakeOnce((Magnitude * (float.Parse((Settings.Get("Screenshake Intensity", 1f).ToString())))), Roughness, FadeInTime, FadeOutTime);

                        WeaponAudioSource?.PlayOneShot(ShootSound);
                        MuzzleFlash.SetActive(true);
                    }
                }
            }

            LastHealth = Health;
            WasDamaged = false;
        }
    }

    public void Damage(int Damage)
    {
        this.WasDamaged = true;
        this.Health -= Damage;
        this.EnemyAudioSource.PlayOneShot(((Health > 0) ? HitSound : DeathSound));

        if (Health <= 0)
        {
            GameObject DroppedPickup = Instantiate(PickupPrefab, DropPoint.position, DropPoint.rotation);
            Rigidbody2D PickupRigidbody = DroppedPickup.GetComponent<Rigidbody2D>();
            Rigidbody.freezeRotation = false;
            if (PickupRigidbody != null)
            {
                PickupRigidbody.velocity = Rigidbody.velocity;
                PickupRigidbody.angularVelocity = Rigidbody.angularVelocity;
            }

            if (WeaponObject.transform.localScale.y < 0f)
            {
                DroppedPickup.transform.localScale = new Vector3(-DroppedPickup.transform.localScale.x, DroppedPickup.transform.localScale.y, DroppedPickup.transform.localScale.z);
                DroppedPickup.transform.eulerAngles = new Vector3(DroppedPickup.transform.eulerAngles.x, DroppedPickup.transform.eulerAngles.y, (DroppedPickup.transform.eulerAngles.z + 180));
            }


            for (int I = 0; I < ObjectsToDestroy.Length; I++) {
                Destroy(ObjectsToDestroy[I]);
            }

            this.gameObject.layer = LayerMask.NameToLayer("Dead Body");

            Rigidbody2D RagdollRigidbody = this.gameObject.GetComponent<Rigidbody2D>();
            RagdollRigidbody.AddTorque((InitialRagdollTorque * -((!float.IsNaN((RagdollRigidbody.velocity.x / Mathf.Abs(RagdollRigidbody.velocity.x))) ? (RagdollRigidbody.velocity.x / Mathf.Abs(RagdollRigidbody.velocity.x)) : 1f))) * ((RagdollRigidbody.velocity.x == 0f) ? (InitialStaticRagdollTorqueMultiplier * ((Random.Range(-1, 2) >= 0) ? 1 : -1)) : 1f));

            DeadBody Body = this.gameObject.AddComponent<DeadBody>();
            Body.RigidbodyForce = DespawningRagdollForce;
            Body.RigidbodyTorque = DespawningRagdollTorque;
            Body.Collider = Collider;
            Body.ColliderLifetime = RagdollColliderLifetime;
            Body.TotalLifetime = RagdollTotalLifetime;
            Destroy(this);
        }
    }

    private void Awake() {
        EnemyRenderer.color = EnemyColorPalette[Random.Range(0, EnemyColorPalette.Length)];
        Health = Mathf.Max(Health, 1);
    }
}
