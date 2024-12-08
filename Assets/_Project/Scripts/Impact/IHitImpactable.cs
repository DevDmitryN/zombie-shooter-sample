using _Project.Scripts.Player;
using UnityEngine;

namespace _Project.Scripts.Impact
{
    public interface IHitImpactable
    {
        void PlayHitImpact(RaycastHit hit);
    }
}