
using UnityEngine;

namespace AILand.GamePlay.World
{
    

    /// <summary>
    /// 世界中的一个Block的数据结构
    /// </summary>
    public class BlockData
    {
        // 区块的唯一标识符
        private int m_blockID;
        
        // 区块宽高
        private int m_width;
        private int m_height;

        // block锚点在世界中的位置
        private Vector3 m_worldPosition;
        
        // block在世界中的坐标
        private Vector2Int m_worldIndex;

        // 所有的Cell数据
        private CellData[,] m_cells;
        
        // 该区块的岛屿信息
        private PCGIslandInfo m_islandInfo;

        // 该区块的岛屿是否由玩家创建
        private bool m_isPlayerCreated; // 是否由玩家创建
        private Vector2 m_createrPosition; // creater在区块中的位置

        private bool m_isCreated; // 岛屿是否已经创建（玩家 or PCG）



        public bool createIsland(PCGIslandInfo islandInfo)
        {
            if (m_isCreated)
            {
                Debug.LogError($"Block island at {m_worldIndex} has already been created.");
                return false;
            }

            m_islandInfo = islandInfo;
            m_cells = new CellData[m_width, m_height];
            
            for (int x = 0; x < m_width; x++)
            {
                for (int z = 0; z < m_height; z++)
                {
                    // 垂直的数据
                    var cellPosition = new Vector3(x, 0, z);
                    m_cells[x, z] = new CellData(new Vector2Int(x,z), cellPosition);
                    
                    // 计算一下高度
                    float heightMapValue = islandInfo.heightMap[x, z];
                    int height = islandInfo.heightMapFunc(heightMapValue);
                    
                    // 创建CubeData
                    
                    
                }
            }
            
            
            
            m_isCreated = true;
            return true;
        }


    }
}
