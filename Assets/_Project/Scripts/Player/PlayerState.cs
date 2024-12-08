using DefaultNamespace.Player.enums;
using UniRx;
using UnityEngine;

namespace _Project.Scripts.Player
{
    public static class PlayerState
    {
        public static readonly ReactiveProperty<Transform> Transform = new();
        public static readonly ReactiveProperty<PlayerMovementStateType> PlayerMovementState = new(PlayerMovementStateType.Stay);
        public static readonly ReactiveProperty<bool> IsPlayerAiming = new(false);
        public static readonly ReactiveProperty<bool> IsPlayerCrouching = new(false);
        
    }
}