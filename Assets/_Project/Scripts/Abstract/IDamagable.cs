namespace DefaultNamespace.Abstract
{
    public class DamageParams
    {
        public float Damage;
    }
    
    public interface IDamagable
    {
        void TakeDamage(DamageParams damageParams);
    }
}