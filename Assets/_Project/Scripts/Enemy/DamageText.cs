using System;
using _Project.Scripts.Player;
using TMPro;
using UnityEngine;

namespace _Project.Scripts.Enemy
{
    public class DamageText : MonoBehaviour
    {
        public TextMeshPro Text;
        public RectTransform Rect;

        private void Update()
        {
            RotateToHero();
        }
        
        private void RotateToHero()
        {
            // Поворот к игроку
            Vector3 direction = (PlayerState.Transform.Value.position - Rect.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
            Rect.rotation = lookRotation;
        }  
    }
}