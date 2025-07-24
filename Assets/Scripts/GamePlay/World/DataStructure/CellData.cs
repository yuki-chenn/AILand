
using System.Collections.Generic;
using AILand.Utils;
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
        
        public CubeData GetCubeData(int yHeight)
        {
            if(yHeight < 0 || yHeight >= m_cubes.Count)
            {
                return null;
            }
            return m_cubes[yHeight];
        }
        
        public void Load()
        {
            foreach (var cube in m_cubes)
            {
                if(IsInVisible(cube))
                {
                    cube.Unload();
                }else
                {
                    cube.Load();
                }
            }
        }

        public void Unload()
        {
            foreach (var cube in m_cubes)
            {
                cube.Unload();
            }
        }
        
        private bool IsInVisible(CubeData cube)
        {
            int y = cube.YHeight;
            
            bool up = y + 1 < m_cubes.Count && GetCubeData(y + 1) != null;
            bool down = y == 0 || GetCubeData(y - 1) != null;
            bool left = m_blockData.GetCellData(m_index.x - 1, m_index.y)?.GetCubeData(y) != null;
            bool right = m_blockData.GetCellData(m_index.x + 1, m_index.y)?.GetCubeData(y) != null;
            bool front = m_blockData.GetCellData(m_index.x, m_index.y - 1)?.GetCubeData(y) != null;
            bool back = m_blockData.GetCellData(m_index.x, m_index.y + 1)?.GetCubeData(y) != null;
            
            return up && down && left && right && front && back;
        }
        
    }
}
