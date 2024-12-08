using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Extensions
{
    public class MonoPool<T> where T : MonoBehaviour
    {
        private List<T> _pool;
        private T _prefab;
        private Transform _parent;

        public List<T> All => _pool;
        
        public int Count => _pool.Count(_ => _.gameObject.activeSelf);
        
        public MonoPool(T prefab, Transform rect, int count)
        {
            _pool = new List<T>();
            _prefab = prefab;
            _parent = rect;
            for (int i = 0; i < count; i++)
                CreateObject();
        }
        
        private T CreateObject()
        {
            var obj = GameObject.Instantiate(_prefab, _parent);
            obj.gameObject.SetActive(false);
            _pool.Add(obj);
            return obj;
        }

        public T Get()
        {
            foreach (var obj in _pool)
            {
                if (!obj.gameObject.activeSelf)
                {
                    obj.gameObject.SetActive(true);
                    return obj;
                }
            }
            
            var instance = CreateObject();
            instance.gameObject.SetActive(true);
            return instance;
        }

        public List<T> GetAllActive()
        {
            return _pool.Where(_ => _.gameObject.activeSelf).ToList();
        }
        
        public bool IsExistActive()
        {
            return _pool.Any(_ => _.gameObject.activeSelf);
        }
        
        public void Hide(T obj)
        {
            obj.gameObject.SetActive(false);
        }
        
        
        public void HideAll()
        {
            foreach (var obj in _pool)
            {
                obj.gameObject.SetActive(false);
            }
        }
        
        public async void Hide(T obj, int delay)
        {
            await UniTask.Delay(delay);
            if (obj != null)
            {
                obj.gameObject.SetActive(false);
            }
            
        }
    }
}