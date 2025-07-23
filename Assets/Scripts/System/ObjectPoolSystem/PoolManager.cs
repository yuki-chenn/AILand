using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Pool;
using AILand.System.Base;
using System.Linq;

namespace AILand.System.ObjectPoolSystem
{
    [Serializable]
    public class PoolSettings
    {
        public GameObject prefab;
        public int defaultCapacity = 10;
        public int maxSize = 100;
        
        [HideInInspector]
        public Type pooledType;
    }
    
    public class PoolManager : Singleton<PoolManager>
    {
        [SerializeField]
        private List<PoolSettings> poolSettingsList = new List<PoolSettings>();
        
        private Dictionary<Type, ObjectPool<GameObject>> pools = new Dictionary<Type, ObjectPool<GameObject>>();
        private Dictionary<Type, GameObject> prefabMap = new Dictionary<Type, GameObject>();
        private Dictionary<Type, Transform> poolRootMap = new Dictionary<Type, Transform>();
        
        protected override void Awake()
        {
            base.Awake();
            InitializePools();
        }
        
        private void InitializePools()
        {
            foreach (var settings in poolSettingsList)
            {
                if (settings.prefab == null) continue;
                
                var comp = settings.prefab.GetComponent<IPooledObject>();
                if (comp == null) continue;
                
                Type type = comp.GetType();
                settings.pooledType = type;
                
                if (!pools.ContainsKey(type))
                {
                    Debug.Log($"Creating pool for type: {type}");
                    CreatePool(type, settings);
                }
                
            }
        }
        
        private void CreatePool(Type type, PoolSettings settings)
        {
            var pool = new ObjectPool<GameObject>(
                createFunc: () => CreatePooledObject(settings.prefab, type),
                actionOnGet: (go) => OnGetObject(go),
                actionOnRelease: (go) => OnReleaseObject(go, type),
                actionOnDestroy: (go) => OnDestroyObject(go),
                defaultCapacity: settings.defaultCapacity,
                maxSize: settings.maxSize
            );
            
            pools[type] = pool;
            prefabMap[type] = settings.prefab;
            
            // 创建一个父物体
            var poolRoot = new GameObject($"Pool_{type.Name}");
            poolRoot.transform.SetParent(transform);
            poolRootMap[type] = poolRoot.transform;
            
            // 先初始化出来
            var warmedObjects = new List<GameObject>();
            for (int i = 0; i < settings.defaultCapacity; i++) warmedObjects.Add(pool.Get());
            foreach (var go in warmedObjects)  pool.Release(go);
        }
        
        
        private GameObject CreatePooledObject(GameObject prefab,Type type)
        {
            var go = Instantiate(prefab, poolRootMap[type]);
            go.SetActive(false);
            return go;
        }
        
        private void OnGetObject(GameObject go)
        {
            go.SetActive(true);
            var pooledObject = go.GetComponent<IPooledObject>();
            pooledObject?.OnGetFromPool();
        }
        
        private void OnReleaseObject(GameObject go, Type type)
        {
            var pooledObject = go.GetComponent<IPooledObject>();
            pooledObject?.OnReleaseToPool();
            go.SetActive(false);
            go.transform.SetParent(poolRootMap[type]);
        }
        
        private void OnDestroyObject(GameObject go)
        {
            var pooledObject = go.GetComponent<IPooledObject>();
            pooledObject?.OnDestroyPoolObject();
            Destroy(go);
        }
        
        /// <summary>
        /// 根据T类型获取池化的GameObject
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public GameObject GetGameObject<T>() where T : Component, IPooledObject
        {
            Type type = typeof(T);
            
            if (pools.TryGetValue(type, out var pool))
            {
                var go = pool.Get();
                return go;
            }
            
            Debug.LogError($"No pool found for type {type}. Make sure the prefab is configured in PoolManager.");
            return null;
        }
        
        /// <summary>
        /// 将GameObject返回到池中
        /// </summary>
        /// <param name="go"></param>
        /// <param name="forceDestroy">当不是找不到返回的池时，强制Destory</param>
        public void Release(GameObject go, bool forceDestroy = false)
        {
            var pooledObject = go.GetComponent<IPooledObject>();
            if (pooledObject == null)
            {
                Debug.LogError($"No pool found for GameObject {go.name}. Ensure it implements IPooledObject.");
            }
            
            Type type = pooledObject.GetType();
            
            if (pools.TryGetValue(type, out var pool))
            {
                pool.Release(pooledObject.GameObject);
            }
            else
            {
                Debug.LogError($"No pool found for type {type}");
                if(forceDestroy) Destroy(pooledObject.GameObject);
            }
        }
        
        public void ClearAllPools()
        {
            foreach (var pool in pools.Values) pool.Clear();
        }
        
        protected void OnDestroy()
        {
            ClearAllPools();
        }
        
        
#if UNITY_EDITOR    
        public (int active, int inactive, int total) GetPoolStats<T>() where T : IPooledObject
        {
            Type type = typeof(T);
            
            if (pools.TryGetValue(type, out var pool) && pool is ObjectPool<GameObject> objectPool)
            {
                return (objectPool.CountActive, objectPool.CountInactive, objectPool.CountAll);
            }
            
            return (0, 0, 0);
        }

        public void RefreshPooledObjectsList()
        {
            // 查找项目中所有的Prefab
            var guids = UnityEditor.AssetDatabase.FindAssets("t:Prefab");
            var pooledPrefabs = new HashSet<GameObject>();
            
            foreach (var guid in guids)
            {
                var path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                var prefab = UnityEditor.AssetDatabase.LoadAssetAtPath<GameObject>(path);
                
                if (prefab != null)
                {
                    var pooledComponents = prefab.GetComponents<IPooledObject>();
                    if (pooledComponents.Length > 0)
                    {
                        pooledPrefabs.Add(prefab);
                    }
                }
            }
            
            // 添加新发现的prefabs
            var existingPrefabs = poolSettingsList.Select(s => s.prefab).Where(p => p != null).ToHashSet();
            
            foreach (var prefab in pooledPrefabs)
            {
                if (!existingPrefabs.Contains(prefab))
                {
                    poolSettingsList.Add(new PoolSettings { prefab = prefab });
                }
            }
            
            // 移除
            poolSettingsList.RemoveAll(s => s.prefab != null && !pooledPrefabs.Contains(s.prefab));
            
            // 标记为脏，确保序列化保存
            UnityEditor.EditorUtility.SetDirty(this);
        }
        
        private void OnValidate()
        {
            foreach (var settings in poolSettingsList)
            {
                if (settings.defaultCapacity <= 0) settings.defaultCapacity = 10;
                if (settings.maxSize < settings.defaultCapacity) settings.maxSize = settings.defaultCapacity * 2;
            }
        }
#endif
        
        
    }
}