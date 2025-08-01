using AILand.System.SOManager;
using AILand.GamePlay.World.Cube;
using AILand.System.ObjectPoolSystem;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace AILand.GamePlay.World
{
    public class CubeData
    {
        // 所在的Cell数据
        private CellData m_cellData;
        public CellData CellData => m_cellData;
        
        // 方块的种类
        private CubeType m_cubeType;
        public CubeType CubeType => m_cubeType;
        private CubeConfigSO m_cubeConfig;
        public CubeConfigSO CubeConfig => m_cubeConfig;
        
        // 方块的旋转
        private int m_rotation;
        
        
        // 方块所在的高度
        private int m_YHeight;
        public int YHeight => m_YHeight;
        
        public Vector3 LocalPosition => new Vector3(m_cellData.LocalPosition.x, m_YHeight, m_cellData.LocalPosition.z);
        
        // 游戏物体实例
        private GameObject m_instanceGo;

        public CubeData(CellData cell, CubeType type,int rotation, int yHeight)
        {
            m_cellData = cell;
            
            m_cubeType = type;
            if(type != CubeType.None) m_cubeConfig = SOManager.Instance.cubeConfigDict[type];

            m_rotation = rotation;
            
            m_YHeight = yHeight;
        }

        private bool m_isLoad = false;

        public void Load()
        {
            if(m_isLoad || m_cubeType == CubeType.None) return;
            if (m_cubeConfig == null)
            {
                Debug.LogError(
                    $"Load CubeData error: CubeConfig for type {m_cubeType} has not been set or does not exist.");
                return;
            }
            var type = m_cubeConfig.cubePrefab.GetComponent<BaseCube>().GetType();
            m_instanceGo = PoolManager.Instance.GetGameObject(type);
            
            m_instanceGo.transform.SetParent(m_cellData.BlockData.BlockComponent.cubeHolder);
            m_instanceGo.transform.localPosition = LocalPosition;
            m_instanceGo.transform.localRotation = Quaternion.Euler(0, m_rotation * 90, 0);
            m_instanceGo.name = $"{m_cellData.BlockData.BlockID}_{m_cellData.Index.x}_{m_cellData.Index.y}_{m_YHeight}_{m_cubeType}";
            
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
                Debug.LogWarning($"try to release a null instance of CubeData at {m_cellData.BlockData.BlockID}_{m_cellData.Index.x}_{m_cellData.Index.y}_{m_YHeight}_{m_cubeType}");
            }
        }

        // 更改方块类型
        public void Change(CubeType cubeType, int rotation, bool needLoad = true)
        {
            if (m_cubeType == cubeType) return;
            Unload();
            m_cubeType = cubeType;
            if (cubeType != CubeType.None)
            {
                m_cubeConfig = SOManager.Instance.cubeConfigDict[cubeType];
                m_rotation = rotation;
                if(needLoad) Load();
            }
            else
            {
                m_rotation = 0;
                m_cubeConfig = null;
                m_instanceGo = null;
            }
        }
    }

}