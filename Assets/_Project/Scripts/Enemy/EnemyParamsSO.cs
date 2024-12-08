using UnityEngine;

namespace _Project.Scripts.Enemy
{
    [CreateAssetMenu(fileName = "Enemy", menuName = "Enemy Params", order = 0)]
    public class EnemyParamsSO : ScriptableObject
    {
        public float Health;
        public float Damage;
        public float MoveSpeed;
        public float AttackCooldown;
        public float StopDistance;

        public EnemyParams GetParams()
        {
            return new EnemyParams()
            {
                Health = Health,
                Damage = Damage,
                MoveSpeed = MoveSpeed,
                AttackCooldown = AttackCooldown,
                StopDistance = StopDistance
            };
        }
    }
}