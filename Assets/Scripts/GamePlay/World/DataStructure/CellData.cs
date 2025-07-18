
using System.Collections.Generic;
using UnityEngine;

namespace AILand.GamePlay.World
{
    public class CellData
    {

        // Block内的坐标
        private Vector2Int m_index;
        
        // Block内的位置
        private Vector3 m_loaalPosition;

        // 所有的方块数据
        private List<CubeData> m_cubes;

        public CellData(Vector2Int index, Vector3 localPosition)
        {
            m_index = index;
            m_loaalPosition = localPosition;
            m_cubes = new List<CubeData>();
        }

    }
}
