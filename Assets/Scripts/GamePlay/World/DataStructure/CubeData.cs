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
        
        // 方块所在的高度
        private int m_YHeight;
        public int YHeight => m_YHeight;
        
        public Vector3 LocalPosition => new Vector3(m_cellData.LocalPosition.x, m_YHeight, m_cellData.LocalPosition.z);
        
        // 游戏物体实例
        private GameObject m_instanceGo;
        public Cube CubeComponent => m_instanceGo.GetComponent<Cube>();

        public CubeData(CellData cell, CubeType type, int yHeight)
        {
            m_cellData = cell;
            m_cubeType = type;
            m_YHeight = yHeight;
        }

        private bool m_isLoad = false;

        public void Load()
        {
            if(m_isLoad) return;
            m_instanceGo = PoolManager.Instance.GetGameObject<Cube>();
            m_instanceGo.transform.SetParent(m_cellData.BlockData.BlockComponent.cubeHolder);
            m_instanceGo.transform.localPosition = LocalPosition;
            m_instanceGo.transform.localRotation = Quaternion.identity;
            m_instanceGo.name = $"{m_cellData.BlockData.BlockID}_{m_cellData.Index.x}_{m_cellData.Index.y}_{m_YHeight}_{m_cubeType}";
            var mat = m_instanceGo.GetComponent<MeshRenderer>().material;
            switch (m_cubeType)
            {
                case CubeType.Sand:
                    mat.color = Color.yellow;
                    break;
                case CubeType.Stone:
                    mat.color = Color.gray;
                    break;
                case CubeType.Dirt:
                    mat.color = new Color(0.545f, 0.271f, 0.075f);
                    break;
                case CubeType.Snow:
                    mat.color = Color.white;
                    break;
                default:
                    throw new NotImplementedException($"CubeType {m_cubeType} is not implemented.");
            }
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
    }

}