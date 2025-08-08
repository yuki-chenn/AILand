using AILand.GamePlay.World.Prop;
using AILand.System.ObjectPoolSystem;
using AILand.System.SOManager;
using UnityEngine;

namespace AILand.GamePlay.World
{
    public class PropData
    {
        private BlockData m_blockData;
        
        private PropType m_propType;
        public PropType PropType => m_propType;
        private PropConfigSO m_propConfig;
        
        private Vector3Int m_index;
        public Vector3Int Index => m_index;
        
        private Vector3 m_localPosition;
        
        private Quaternion m_rotation;

        private GameObject m_instanceGo;
        private bool m_isLoad = false;
        public GameObject InstanceGo => m_instanceGo;
        public bool IsLoad => m_isLoad;
        
        public PropData(BlockData blockData, PropType propType, Vector3Int index, Quaternion rotation)
        {
            m_blockData = blockData;
            m_propType = propType;
            m_propConfig = SOManager.Instance.propConfigDict[propType];
            m_index = index;
            m_localPosition = index;
            m_rotation = rotation;
        }
        
        
        public void Load()
        {
            if(m_isLoad) return;
            
            if (m_propConfig == null)
            {
                Debug.LogError(
                    $"Load CubeData error: CubeConfig for type {m_propConfig} has not been set or does not exist.");
                return;
            }
            
            var type = m_propConfig.propPrefab.GetComponent<BaseProp>().GetType();
            m_instanceGo = PoolManager.Instance.GetGameObject(type);
            
            m_instanceGo.SetActive(false);
            m_instanceGo.transform.SetParent(m_blockData.BlockComponent.propHolder);
            m_instanceGo.transform.localPosition = m_localPosition;
            m_instanceGo.transform.localRotation = m_rotation;
            m_instanceGo.name = $"{m_blockData.BlockID}_{m_index.x}_{m_index.z}_{m_index.y}_{m_propType}";
            m_instanceGo.SetActive(true);
            
            m_isLoad = true;
        }
        
        public void Unload()
        {
            if (!m_isLoad) return;
            if (m_instanceGo != null)
            {
                PoolManager.Instance.Release(m_instanceGo);
                m_instanceGo = null;
                m_isLoad = false;
            }
            else
            {
                Debug.LogWarning($"try to release a null instance of CubeData at {m_blockData.BlockID}_{m_index.x}_{m_index.z}_{m_index.y}_{m_propType}");
            }
        }
    }
}