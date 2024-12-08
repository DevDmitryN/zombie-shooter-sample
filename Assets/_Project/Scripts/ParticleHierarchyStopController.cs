using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DefaultNamespace
{
    public class ParticleHierarchyStopController : MonoBehaviour
    {
        public List<GameObject> children;
        private List<ParticleSystem> childParticleSystems;

        private ParticleSystem _currentParticle;
        
        private void OnEnable()
        {
            _currentParticle = GetComponent<ParticleSystem>();
            childParticleSystems = children
                .Select(_ => _.GetComponent<ParticleSystem>())
                .ToList();
            
            var mainModule = _currentParticle.main;
            mainModule.stopAction = ParticleSystemStopAction.Callback;

            // Подпишитесь на событие остановки родительского Particle System
            //_currentParticle. += OnParentParticleSystemStop;
        }


        private void OnDisable()
        {
            
        }

        void OnParentParticleSystemStop(ParticleSystem system)
        {
            // Остановите все дочерние Particle System
            foreach (var child in childParticleSystems)
            {
                child.Stop();
            }
        }
        
    }
}