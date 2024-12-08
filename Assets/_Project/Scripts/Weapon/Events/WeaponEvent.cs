using System;
using UniRx;

namespace _Project.Scripts.Weapon
{
    public static class WeaponEvent
    {
        public static readonly Subject<ShootEvent> ShootEvent = new();
    }
}