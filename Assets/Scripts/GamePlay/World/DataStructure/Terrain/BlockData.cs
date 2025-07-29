
using AILand.System.ObjectPoolSystem;
using AILand.Utils;
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
        public int BlockID => m_blockID;
        
        // 区块宽高
        private int m_width;
        private int m_height;

        // block锚点在世界中的位置
        private Vector3 m_worldPosition;
        public Vector3 WorldPosition => m_worldPosition;
        
        // block在世界中的坐标
        private Vector2Int m_worldIndex;
        public Vector2Int WorldIndex => m_worldIndex;

        // 所有的Cell数据
        private CellData[,] m_cells;
        public CellData[,] Cells => m_cells;
        
        // 该区块的岛屿信息
        private PCGIslandInfo m_islandInfo;

        // 该区块的岛屿是否由玩家创建
        private bool m_isPlayerCreated; // 是否由玩家创建
        public bool IsPlayerCreated => m_isPlayerCreated;
        private Vector3 m_generatorPosition; // creater在区块中的位置
        public Vector3 GeneratorPosition => m_generatorPosition;
 
        private bool m_isCreated; // 岛屿是否已经创建（玩家 or PCG）
        public bool IsCreated => m_isCreated;
        
        
        // 游戏物体实例
        private GameObject m_instanceGo;
        public GameObject InstanceGo
        {
            get
            {
                if (!m_instanceGo) InstantiateBlock();
                return m_instanceGo;
            }
        }
        public Block BlockComponent => m_instanceGo.GetComponent<Block>();

        
        public BlockData(int blockID, int width, int height)
        {
            m_blockID = blockID;
            m_width = width;
            m_height = height;
            m_worldPosition = Util.GetBlockPositionByID(blockID, width, height);
            m_worldIndex = Util.GetBlockIndexByID(blockID);
            m_isCreated = false;
            m_isPlayerCreated = true;
            m_generatorPosition = new Vector3(100, 0, 100); // 默认生成位置
        }


        private void InstantiateBlock()
        {
            m_instanceGo = PoolManager.Instance.GetGameObject<Block>();
            m_instanceGo.transform.SetParent(GameManager.Instance.blockHolder);
            m_instanceGo.GetComponent<Block>().SetBlockData(this);
        }


        /// <summary>
        /// 根据配置创建岛屿数据
        /// </summary>
        /// <param name="islandInfo">配置数据</param>
        /// <returns></returns>
        public bool CreateIsland(PCGIslandInfo islandInfo)
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
                    var cell = new CellData(this, new Vector2Int(x,z), cellPosition);
                    
                    // 计算一下高度
                    var cellCubesType = islandInfo.GetCellCubesType(x, z);
                    
                    
                    if(cellCubesType != null)
                    {
                        // 创建CubeData
                        for (int y = 0; y < cellCubesType.Count; y++)
                        {
                            var cubeData = new CubeData(cell, cellCubesType[y],0, y);
                            cell.Cubes.Add(cubeData);
                        }
                    }
                    
                    
                    
                    m_cells[x, z] = cell;
                }
            }
            
            m_isCreated = true;
            return m_isCreated;
        }


        public void DestoryCube(int x,int y,int z)
        {
            var cell = GetCellData(x, z);
            if (cell == null)
            {
                Debug.LogError($"DestoryCube error : Cell at {x}, {z} does not exist in block {m_blockID}.");
                return;
            }
            cell.DestoryCube(y);
            
            // 需要load旁边的cell
            LoadAroundCells(x, z);
            
        }
        
        public void AddCube(int x, int y, int z, CubeType cubeType)
        {
            var cell = GetCellData(x, z);
            if (cell == null)
            {
                Debug.LogError($"AddCube error : Cell at {x}, {z} does not exist in block {m_blockID}.");
                return;
            }
            cell.AddCube(y, cubeType);
            
            // 需要load旁边的cell
            LoadAroundCells(x, z);
        }
        
        private void LoadAroundCells(int x, int z)
        {
            // 加载周围的Cell
            GetCellData(x - 1, z)?.Load();
            GetCellData(x + 1, z)?.Load();
            GetCellData(x, z - 1)?.Load();
            GetCellData(x, z + 1)?.Load();
        }
        
        
        public CellData GetCellData(int x,int z)
        {
            if (x < 0 || x >= m_width || z < 0 || z >= m_height)
            {
                return null;
            }
            return m_cells[x, z];
        }
    }
}
