using System;
using System.Collections.Generic;
using AILand.System.ObjectPoolSystem;
using UnityEngine;

namespace AILand.GamePlay.Battle
{
    public class VfxController : MonoBehaviour, IPooledObject
    {
        [Serializable]
        public class VfxData
        {
            public string name;
            public GameObject vfxObject;
            public float duration = 1f;
        }

        [SerializeField]
        public VfxData[] vfxList;
        private Dictionary<string, VfxData> vfxDict;

        public GameObject GameObject => gameObject;
        
        private bool m_isReleased = false;

        private void Awake()
        {
            InitializeVfxDictionary();
        }

        private void InitializeVfxDictionary()
        {
            vfxDict = new Dictionary<string, VfxData>();
            foreach (var vfx in vfxList)
            {
                if (!string.IsNullOrEmpty(vfx.name))
                    vfxDict[vfx.name] = vfx;
            }
        }

        public void Play(string vfxName, Vector3? position = null, Quaternion? rotation = null)
        {
            if (!vfxDict.TryGetValue(vfxName, out VfxData vfxData))
            {
                Debug.LogWarning($"VFX '{vfxName}' not found!");
                return;
            }
            
            transform.position = position ?? Vector3.zero;
            transform.rotation = rotation ?? Quaternion.identity;

            vfxData.vfxObject.SetActive(true);
            
            // 自动关闭特效
            Invoke("Release", vfxData.duration);
        }

        private void StopVfx()
        {
            // 关闭所有特效
            foreach (var vfx in vfxList)
            {
                vfx.vfxObject.SetActive(false);
            }
        }

        public void Release()
        {
            if (m_isReleased) return;
            StopVfx();
            PoolManager.Instance.Release(gameObject);
        }

        public void OnGetFromPool()
        {
            m_isReleased = false;
        }

        public void OnReleaseToPool()
        {
            transform.position = Vector3.zero;
            transform.rotation = Quaternion.identity;
            StopVfx();
            m_isReleased = true;
        }

        public void OnDestroyPoolObject()
        {
            
        }
    }
}