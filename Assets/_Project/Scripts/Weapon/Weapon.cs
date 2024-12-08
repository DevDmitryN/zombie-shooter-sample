using _Project.Scripts.Player;
using DefaultNamespace.Player.enums;
using UnityEngine;
using UniRx;

namespace _Project.Scripts.Weapon
{
    public class Weapon : MonoBehaviour
    {
        private Animator _animator;
        private readonly string _aimAnimationKey = "IsAiming";
        private readonly string _movingAnimationKey = "IsMoving";
        private readonly string _shootTrigger = "Shoot";
        
        private void OnEnable()
        {
            _animator = GetComponent<Animator>();

            PlayerState.PlayerMovementState
                .TakeUntilDestroy(this)
                .Subscribe(OnPlayerMovementChanged);
            
            PlayerState.IsPlayerAiming
                .TakeUntilDestroy(this)
                .Skip(1)
                .Subscribe(OnAim);
            
            WeaponEvent.ShootEvent
                .TakeUntilDestroy(this)
                .Subscribe(OnShoot);
        }
        
        private void OnAim(bool aim)
        {
            _animator.SetBool(_aimAnimationKey, aim);
        }

        private void OnPlayerMovementChanged(PlayerMovementStateType stateType)
        {
            switch (stateType)
            {
                case PlayerMovementStateType.Stay:
                    _animator.SetBool(_movingAnimationKey, false);
                    break;
                default:
                    _animator.SetBool(_movingAnimationKey, true);
                    break;
            }
        }

        private void OnShoot(ShootEvent e)
        {
            _animator.SetTrigger(_shootTrigger);
        }
    }
}