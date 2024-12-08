using Extensions;
using UnityEngine;

namespace _Project.Scripts.Impact
{
    public static class ImpactUtils
    {
        public static void Play(MonoPool<ImpactEffect> pool, RaycastHit hit)
        {
            var impactEffect = pool.Get();
            impactEffect.SetDefaultPosition(hit);
            pool.Hide(impactEffect, 500);
        }
    }
}