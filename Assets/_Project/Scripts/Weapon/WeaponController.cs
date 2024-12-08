using System;
using _Project.Scripts.Impact;
using _Project.Scripts.Weapon;
using DefaultNamespace.Abstract;
using Extensions;
using UnityEngine;

namespace DefaultNamespace
{
    public class WeaponController : MonoBehaviour
    {
        public WeaponParams WeaponParams;
        public GameObject FirePoint;
        public float fireRate = 0.5f;
        public float range = 100f;
        public Camera fpsCam;
        public Muzzle muzzleFlash;
        public ImpactEffect impactEffect;
        
        private float nextTimeToFire = 0f;

        private bool _isFire;
        private MonoPool<Muzzle> _muzzlePool;
        private MonoPool<ImpactEffect> _impactPool;

        private void OnEnable()
        {
            _muzzlePool = new MonoPool<Muzzle>(muzzleFlash, fpsCam.transform, 10);
            _impactPool = new MonoPool<ImpactEffect>(impactEffect, transform, 10);
        }

        void Update()
        {
            if (Input.GetButton("Fire1") && Time.time >= nextTimeToFire)
            {
                nextTimeToFire = Time.time + 1f / fireRate;
                Shoot();
            }
        }

        void Shoot()
        {
            WeaponEvent.ShootEvent.OnNext(new ShootEvent());
            PlayMuzzleEffect();

            RaycastHit hit;
            if (Physics.Raycast(FirePoint.transform.position, fpsCam.transform.forward, out hit, range))
            {
                
                if (hit.transform.TryGetComponent<IHitImpactable>(out var hitImpactable))
                {
                    hitImpactable.PlayHitImpact(hit);
                }
                else
                {
                    PlayDefaultImpact(hit);
                }

                if (hit.transform.TryGetComponent<IDamagable>(out var damagable))
                {
                    Debug.Log($"Hit damagable {damagable}");
                    damagable.TakeDamage(new DamageParams()
                    {
                        Damage = WeaponParams.Damage
                    });
                }
             
            }
        }

        private void PlayDefaultImpact(RaycastHit hit)
        {
            ImpactUtils.Play(_impactPool, hit);
        }
        
        private void PlayMuzzleEffect()
        {
            // Muzzle muzzle = Instantiate(muzzleFlash, muzzleFlash.transform.position, muzzleFlash.transform.rotation);
            // muzzle.PlayOne();
            // Destroy(muzzle, 1f);
            var muzzle = _muzzlePool.Get();
            muzzle.transform.position = muzzleFlash.transform.position;
            muzzle.transform.rotation = muzzleFlash.transform.rotation;
            muzzle.PlayOne();
            _muzzlePool.Hide(muzzle, 500);
        }
    }
}