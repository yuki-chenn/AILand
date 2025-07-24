
using System.Collections.Generic;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace AILand.GamePlay.World
{
    public class CellData
    {
        
        private BlockData m_blockData;
        public BlockData BlockData => m_blockData;

        // Block内的坐标
        private Vector2Int m_index;
        public Vector2Int Index => m_index;
        
        // Block内的位置
        private Vector3 m_localPosition;
        public Vector3 LocalPosition => m_localPosition;

        // 所有的方块数据
        private List<CubeData> m_cubes;
        public List<CubeData> Cubes => m_cubes;
        
        public CubeData TopCube => m_cubes.Count > 0 ? m_cubes[m_cubes.Count - 1] : null;

        public int Height => m_cubes.Count;

        public CellData(BlockData blockData, Vector2Int index, Vector3 localPosition)
        {
            m_blockData = blockData;
            m_index = index;
            m_localPosition = localPosition;
            m_cubes = new List<CubeData>();
        }
        
        public void Load()
        {
            foreach (var cube in m_cubes)
            {
                cube.Load();
            }
        }

        public void Unload()
        {
            foreach (var cube in m_cubes)
            {
                cube.Unload();
            }
        }
    }
}
