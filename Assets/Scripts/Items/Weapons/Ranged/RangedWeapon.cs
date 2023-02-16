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
    public float ImpactForce;
    public GameObject ImpactEffect;
    public int ImpactParticleCount;
    public float ImpactVerticalOffset;
    public Transform FirePoint;
    public AudioSource AudioSource;
    public float RateOfFireInRPM;
    public bool Automatic;


    [Header("Look Settings")]
    public Camera LookCamera;
    public LookAt2D Look;
    public float LookOffset;


    [Header("Recoil Settings")]
    public float Magnitude;
    public float Roughness;
    public float FadeInTime;
    public float FadeOutTime;

    // Private / Hidden variables..
    private float ShotTimer;

    private void Update()
    {
        if (!PauseMenu.Instance.IsPaused)
        {
            Look.PointTorwards(LookCamera.ScreenToWorldPoint(Input.mousePosition), LookOffset, true);
            if (((Automatic) ? Input.GetKey(KeyCode.Mouse0) : Input.GetKeyDown(KeyCode.Mouse0)) && ShotTimer <= 0) this.Shoot();

            if (ShotTimer > 0) ShotTimer -= Time.deltaTime;
        }
    }

    public void Shoot()
    {
        RaycastHit2D Hit = Physics2D.Raycast(FirePoint.position, FirePoint.up);
        ShotTimer = 60 / RateOfFireInRPM;

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

            Debug.DrawLine(FirePoint.position, Hit.point);
        }

        CameraShaker.Instance.ShakeOnce((Magnitude * (float.Parse((Settings.Get("Screenshake Intensity", 1f).ToString())))), Roughness, FadeInTime, FadeOutTime);

        AudioSource?.PlayOneShot(ShootSound);
        MuzzleFlash.SetActive(true);
    }
}
