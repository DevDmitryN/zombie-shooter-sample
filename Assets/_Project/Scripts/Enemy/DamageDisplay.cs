using _Project.Scripts.Player;
using Extensions;
using UnityEngine;
using DG.Tweening;
using UniRx;

namespace _Project.Scripts.Enemy
{
    public class DamageDisplayParams
    {
        public float Damage;
        public Vector3 Position;
    }
    
    public class DamageDisplay : MonoBehaviour
    {
        public static Subject<DamageDisplayParams> OnShow = new();
        
        [SerializeField] private Camera _camera;
        [SerializeField] private DamageText _damageTextPrefab;
        [SerializeField] private GameObject _damageTextContainer;

        [Range(0, 0.5f)]
        public float RangeSeed = 1;
        public int DisplayTextTime = 2000;
        public float TextMoveYDuration = 0.8f;
        [Range(0, 2)]
        public float RangeMoveYHeightMin = 0;
        [Range(0, 2)]
        public float RangeMoveYHeightMax = 2;
        public float TextScale = 1;
        
        
        private MonoPool<DamageText> _pool;
        
        private void OnEnable()
        {
            _pool = new MonoPool<DamageText>(_damageTextPrefab, _damageTextContainer.transform, 50);
            OnShow
                .TakeUntilDestroy(this)
                .Subscribe(ShowDamage);
        }
        
        public void ShowDamage(DamageDisplayParams displayParams)
        {
            var damageTextObj = _pool.Get();
            damageTextObj.Rect.localScale = new Vector3(TextScale, TextScale, 1);
            var direction = (PlayerState.Transform.Value.position - displayParams.Position).normalized;
            // Вычисляем перпендикулярный вектор к directionAB в плоскости XY
            var perpendicularDirection = new Vector3(-direction.z, 0, direction.x).normalized;
            // Генерируем случайную длину отрезка в заданном диапазоне
            var segmentLength = Random.Range(-RangeSeed, RangeSeed);
            damageTextObj.Rect.position = displayParams.Position + perpendicularDirection *segmentLength;
            Debug.Log($"Position {damageTextObj.Rect.position.x} {damageTextObj.Rect.position.y}  {damageTextObj.Rect.position.z}");
            
            damageTextObj.Text.text = ((int)displayParams.Damage).ToString();
            damageTextObj.Rect.DOMoveY(damageTextObj.Rect.position.y +  Random.Range(RangeMoveYHeightMin, RangeMoveYHeightMax), TextMoveYDuration);
            _pool.Hide(damageTextObj, DisplayTextTime);
        }
    }
}