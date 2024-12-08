using System;
using _Project.Scripts.Impact;
using _Project.Scripts.Player;
using DefaultNamespace.Abstract;
using Extensions;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace _Project.Scripts.Enemy
{
    [RequireComponent(typeof(Animator), typeof(NavMeshAgent))]
    public class Enemy : MonoBehaviour, IHitImpactable, IDamagable
    {
        public ImpactEffect hitImpact;
        public GameObject attackColliderGO;
        public EnemyParamsSO paramsSo;
        public GameObject DamageTextPointSpawner;
        
        private MonoPool<ImpactEffect> _hitImpactPool;
        private NavMeshAgent agent;
        
        
        #region Animations

        private Animator _animator;
        private readonly string _isMovingAnim = "IsMoving";
        private readonly string _attackAnim = "Attack";
        
        #endregion

        private EnemyParams _params;
        private bool _isMoving = false;
        private float lastAttackTime;
        private bool isEnableAttack;
        
        private Subject<Unit> _onDisable = new Subject<Unit>();

        private bool CooldownFinished => Time.time - lastAttackTime >= _params.AttackCooldown;

        void OnEnable()
        {
            _params = paramsSo.GetParams();
            _animator = GetComponent<Animator>();
            
            agent = GetComponent<NavMeshAgent>();
            agent.updateRotation = false;
            agent.updateUpAxis = false;
            agent.speed = _params.MoveSpeed;

            _hitImpactPool = new MonoPool<ImpactEffect>(hitImpact, transform, 5);

            HandleAttack();
        }

        private void OnDisable()
        {
            _onDisable.OnNext(Unit.Default);
        }

        void Update()
        {
            if (!PlayerState.Transform.HasValue)
                return;
           // MoveToHero();
           RotateToHero();
        }

        private void MoveToHero()
        {
           
            float distance = Vector3.Distance(PlayerState.Transform.Value.position, transform.position);

            if (distance < _params.StopDistance)
            {
                agent.ResetPath();
                _isMoving = false;
            }
            else
            {
                agent.SetDestination(PlayerState.Transform.Value.position);
                _isMoving = true;
            }
            
            _animator.SetBool(_isMovingAnim, _isMoving);
        }

        private void RotateToHero()
        {
            // Поворот врага к игроку
            Vector3 direction = (PlayerState.Transform.Value.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }                                  

        
        
        private void HandleAttack()
        {
            attackColliderGO.OnTriggerEnterAsObservable()
                .TakeUntil(_onDisable)
                .Where(collider => collider.TryGetComponent<GamePlayer>(out var player))
                .Subscribe(_ => isEnableAttack = true);
            
            attackColliderGO.OnTriggerExitAsObservable()
                .TakeUntil(_onDisable)
                .Where(collider => collider.TryGetComponent<GamePlayer>(out var player))
                .Subscribe(_ => isEnableAttack = false);
            
            attackColliderGO.OnTriggerStayAsObservable()
                .TakeUntil(_onDisable)
                .Where(_ => isEnableAttack)
                .Subscribe(collider =>
                {
                    if (collider.TryGetComponent<GamePlayer>(out var player))
                    {
                       Attack();
                    }
                });
        }

        private void Attack()
        {
            if (CooldownFinished)
            {
                _animator.SetTrigger(_attackAnim);
                lastAttackTime = Time.time;
            }
        }
        
        public void PlayHitImpact(RaycastHit hit)
        {
           ImpactUtils.Play(_hitImpactPool, hit);
        }

        public void TakeDamage(DamageParams damageParams)
        {
            DamageDisplay.OnShow.OnNext(new DamageDisplayParams()
            {
                Damage = damageParams.Damage,
                Position = DamageTextPointSpawner.transform.position,
            });
            
            _params.Health -= damageParams.Damage;

            // if (_params.Health <= 0)
            // {
            //     Debug.Log("Zombie Dead");
            // }
        }
    }
}