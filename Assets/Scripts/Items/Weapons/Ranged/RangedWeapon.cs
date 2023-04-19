using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CameraShake;

[RequireComponent(typeof(LookAt2D))]
public class RangedWeapon : MonoBehaviour
{
    [Header("Weapon Settings")]
    public GameObject MuzzleFlash;
    public AudioClip ShootSound;
    public float Range;
    public int Damage;
    public LayerMask LayerMask;
    public float ImpactForce;
    public GameObject ImpactEffect;
    public int ImpactParticleCount;
    public float ImpactVerticalOffset;
    public LineRenderer Tracer;
    public Transform FirePoint;
    public AudioSource AudioSource;
    public Rigidbody2D PlayerRigidbody;
    public GameObject ShellPrefab;
    public Transform EjectionPort;
    public Vector2 EjectionForce;
    public float EjectionTorque;
    public float RecoilForce;
    public bool StunningRecoil;
    public float RateOfFireInRPM;
    public bool Automatic;


    [Header("Look Settings")]
    public Camera LookCamera;
    public LookAt2D Look;
    public float LookOffset;


    [Header("Screenshake Settings")]
    public float Magnitude;
    public float Roughness;
    public float FadeInTime;
    public float FadeOutTime;

    // Private / Hidden variables..
    private float ShotTimer;
    private bool ShotBefore;

    private void Update()
    {
        if (!PauseMenu.Instance.IsPaused)
        {
            Look.PointTorwards(LookCamera.ScreenToWorldPoint(Input.mousePosition), LookOffset, true);
            if (((Automatic) ? Input.GetKey(KeyCode.Mouse0) : Input.GetKeyDown(KeyCode.Mouse0)) && ShotTimer <= 0) this.Shoot();

            if (ShotTimer > 0) ShotTimer -= Time.deltaTime;
            if (!ShotBefore) {
                PromptController.Instance.Show("<b>Left Click</b> to shoot.");
            }
        }
    }

    public void Shoot()
    {
        RaycastHit2D Hit = Physics2D.Raycast(FirePoint.position, FirePoint.up, Range, LayerMask);
        ShotTimer = 60 / RateOfFireInRPM;


        if (!ShotBefore) {
            PromptController.Instance.Clear();
            ShotBefore = true;
        }


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

            if (Hit.transform.gameObject.TryGetComponent<EnemyAI>(out EnemyAI Enemy)) {
                Enemy.Damage(Damage);
            }
        }

        CameraShaker.Instance.ShakeOnce((Magnitude * (float.Parse((Settings.Get("Screenshake Intensity", 1f).ToString())))), Roughness, FadeInTime, FadeOutTime);


        Vector2 RecoilDirection = PlayerRigidbody.transform.position - FirePoint.position;
        RecoilDirection.Normalize();
        if (StunningRecoil) {
            PlayerRigidbody.GetComponent<HealthSystem>()?.Stun();
        }

        Vector2 RecoilAcceleration = ((RecoilDirection * RecoilForce) / PlayerRigidbody.mass);
        Vector2 RecoilVelocity = RecoilAcceleration * Time.fixedDeltaTime;
        PlayerRigidbody.velocity += RecoilVelocity;


        Rigidbody2D EjectedShellRigidbody = Instantiate(ShellPrefab, EjectionPort.transform.position, EjectionPort.transform.rotation).GetComponent<Rigidbody2D>();
        EjectedShellRigidbody.velocity = PlayerRigidbody.velocity;
        EjectedShellRigidbody.angularVelocity = PlayerRigidbody.angularVelocity;

        Vector2 PivotEjectionPortDifference = this.transform.position - EjectionPort.transform.position;
        PivotEjectionPortDifference.y = 0f;
        PivotEjectionPortDifference.Normalize();

        float TorqueSign = -PivotEjectionPortDifference.x;

        EjectedShellRigidbody.AddRelativeForce(EjectionForce);
        EjectedShellRigidbody.angularVelocity = EjectionTorque * TorqueSign;


        Vector2 FallbackHitPoint = new Ray2D(FirePoint.position, FirePoint.up).GetPoint(Range);
        Tracer.SetPositions(new Vector3[] { FirePoint.transform.position, ((Hit) ? Hit.point : FallbackHitPoint) });
        Tracer.gameObject.SetActive(true);

        AudioSource?.PlayOneShot(ShootSound);
        MuzzleFlash.SetActive(true);
    }
}
