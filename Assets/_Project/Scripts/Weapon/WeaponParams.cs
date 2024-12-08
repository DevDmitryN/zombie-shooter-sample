using UnityEngine;

namespace _Project.Scripts.Weapon
{
    [CreateAssetMenu(fileName = "Default Weapon Params", menuName = "Weapon Params", order = 0)]
    public class WeaponParams : ScriptableObject
    {
        public float Damage;
    }
}