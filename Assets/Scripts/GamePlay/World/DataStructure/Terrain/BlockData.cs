
using System.Collections.Generic;
using AILand.System.ObjectPoolSystem;
using AILand.System.SOManager;
using AILand.Utils;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

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

        // IslandType
        private IslandType m_islandType;
        public IslandType IslandType => m_islandType;

        // 所有的Cell数据
        private CellData[,] m_cells;
        public CellData[,] Cells => m_cells;
        
        // 所有Prop数据
        private List<PropData> m_props = new List<PropData>();
        public List<PropData> Props => m_props;
        
        // 该区块的岛屿信息
        private PCGIslandInfo m_islandInfo;

        // 该区块的岛屿是否由玩家创建
        private bool m_isPlayerCreated; // 是否由玩家创建
        public bool IsPlayerCreated => m_isPlayerCreated;
        private Vector3 m_generatorPosition; // creater在区块中的位置
        public Vector3 GeneratorPosition => m_generatorPosition;
 
        private bool m_isCreated; // 岛屿是否已经创建（玩家 or PCG）
        public bool IsCreated => m_isCreated;
        
        // 数据集合
        public Dictionary<CellWater, List<CellData>> m_cellWaterDic = new();
        
        
        
        
        
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

        
        public BlockData(int blockID, int width, int height, IslandType islandType)
        {
            m_blockID = blockID;
            m_width = width;
            m_height = height;
            m_worldPosition = Util.GetBlockPositionByID(blockID, width, height);
            m_worldIndex = Util.GetBlockIndexByID(blockID);

            m_islandType = islandType;
            
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
            
            // 地形创建
            m_islandInfo = islandInfo;
            m_cells = new CellData[m_width, m_height];
            
            for (int x = 0; x < m_width; x++)
            {
                for (int z = 0; z < m_height; z++)
                {
                    // 垂直的数据
                    var cellPosition = new Vector3(x, 0, z);
                    var cell = new CellData(this, new Vector2Int(x,z), cellPosition);
                    
                    var cellCubesType = islandInfo.GetCellCubesType(x, z);
                    
                    if(cellCubesType != null)
                    {
                        // 初始化水域状态
                        if (cellCubesType.Count == 0) cell.CellWater = CellWater.InnerWater;
                        else cell.CellWater = CellWater.None;
                        
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
            // 初始化外部水域信息
            InitOuterWater();
            // 初始化Dic数据
            m_cellWaterDic.Clear();
            m_cellWaterDic[CellWater.InnerWater] = new List<CellData>();
            m_cellWaterDic[CellWater.OuterWater] = new List<CellData>();
            m_cellWaterDic[CellWater.None] = new List<CellData>();
            m_cellWaterDic[CellWater.BorderWater] = new List<CellData>();
            for (int x = 0; x < m_width; x++)
            {
                for (int z = 0; z < m_height; z++)
                {
                    var cell = GetCellData(x, z);
                    if(cell == null) continue;
                    m_cellWaterDic[cell.CellWater].Add(cell);
                }
            }
            
            // preset创建
            foreach (var presetSetting in islandInfo.islandConfig.presetSettings)
            {
                int count = presetSetting.fixedCount <= 0
                    ? Util.GetRandomInRange(presetSetting.rangeCount.x, presetSetting.rangeCount.y)
                    : presetSetting.fixedCount;
                for (int i = 0; i < count; i++)
                {
                    AddPreset(presetSetting.presetType);
                }
            }
            
            
            m_isCreated = true;
            return m_isCreated;
        }

        


        public void DestoryCube(int x,int y,int z,bool needLoad=true)
        {
            var cell = GetCellData(x, z);
            if (cell == null)
            {
                Debug.LogError($"DestoryCube error : Cell at {x}, {z} does not exist in block {m_blockID}.");
                return;
            }
            cell.DestoryCube(y, needLoad);
            
            // 需要load旁边的cell
            if(needLoad) LoadAroundCells(x, z);
            
        }
        
        public void AddCube(int x, int y, int z, CubeType cubeType,int rotation,bool needLoad=true)
        {
            var cell = GetCellData(x, z);
            if (cell == null)
            {
                Debug.LogError($"AddCube error : Cell at {x}, {z} does not exist in block {m_blockID}.");
                return;
            }
            cell.AddCube(y, cubeType, rotation, needLoad);
            
            // 需要load旁边的cell
            if(needLoad) LoadAroundCells(x, z);
        }

        public void AddProp(PropType propType, Vector3Int index, Quaternion rotation)
        {
            PropData propData = new PropData(this, propType, index, rotation);
            m_props.Add(propData);
        }
        
        private void InitOuterWater()
        {
            Queue<Vector2Int> queue = new Queue<Vector2Int>();
            bool[,] vis = new bool[m_width, m_height];
             
            queue.Enqueue(new Vector2Int(0, 0));
            vis[0, 0] = true;
            while (queue.Count != 0)
            {
                Vector2Int current = queue.Dequeue();
                var curCell = GetCellData(current.x, current.y);
                if(curCell.CellWater == CellWater.InnerWater) curCell.CellWater = CellWater.OuterWater; // 外部水域
                
                for(int dx=-1;dx<=1;dx++)
                {
                    for(int dz=-1;dz<=1;dz++)
                    {
                        int sx = current.x + dx;
                        int sz = current.y + dz;
                        
                        if (sx < 0 || sx >= m_width || sz < 0 || sz >= m_height || vis[sx,sz]) continue;
                        
                        var cell = GetCellData(sx, sz);
                        if (cell == null)
                        {
                            vis[sx, sz] = true;
                            continue;
                        }
                        
                        if (cell.CellWater == CellWater.InnerWater)
                        {
                            queue.Enqueue(new Vector2Int(sx, sz));
                            vis[sx, sz] = true;
                        }
                        else if (cell.CellWater == CellWater.None)
                        {
                            GetCellData(current.x, current.y).CellWater = CellWater.BorderWater; // 边界水域
                        }
                    }
                }
            }
            
        }
        
        private void LoadAroundCells(int x, int z)
        {
            // 加载周围的Cell
            GetCellData(x - 1, z)?.Load();
            GetCellData(x + 1, z)?.Load();
            GetCellData(x, z - 1)?.Load();
            GetCellData(x, z + 1)?.Load();
        }

        private bool AddPreset(CubePresetType presetType)
        {
            
            CubePresetSO preset = SOManager.Instance.cubePresetDict[presetType];
            if (preset == null)
            {
                Debug.LogError($"Preset {presetType} not found.");
                return false;
            }
            
            // 先找到一个能放的位置
            int ty = -1;
            CellData tcell = null;
            List<CellData> cellList = new List<CellData>(m_cellWaterDic[preset.cellWater]);
            Util.ShuffleList(cellList);
            foreach (var cell in cellList)
            {
                ty = CanPlacePreset(cell, preset);
                if (ty != -1)
                {
                    tcell = cell;
                    break;
                }
            }

            if (ty == -1 || tcell == null)
            {
                Debug.LogWarning($"AddPreset error: No suitable position found for preset {presetType} in block {m_blockID}.");
                return false;
            }
            else
            {
                PlacePreset(new Vector3Int(tcell.Index.x, ty, tcell.Index.y), preset);
                Debug.Log($"Successfully placed preset {presetType} at {tcell.Index.x}, {ty}, {tcell.Index.y} in block {m_blockID}.");
            }

            return true;
        }

        private void PlacePreset(Vector3Int rootIndex, CubePresetSO preset)
        {
            // cube
            foreach (var cube in preset.cubes)
            {
                var index = cube.position + rootIndex;
                var worldCube = GetCellData(index.x, index.z)?.GetCubeData(index.y);
                if (worldCube != null && worldCube.CubeType != CubeType.None) 
                    DestoryCube(index.x, index.y, index.z, false);
                AddCube(index.x, index.y, index.z, cube.cubeType, cube.rotation, false);
            }
            
            // prop
            foreach (var prop in preset.props)
            {
                AddProp(prop.propType, prop.position + rootIndex, prop.rotation);
            }
            
        }

        private int CanPlacePreset(CellData cell, CubePresetSO preset)
        {
            // TODO: 这里还没处理能否旋转的情况，这个有点复杂
            
            int x = cell.Index.x;
            int z = cell.Index.y;
            
            List<int> availableHeights = new List<int>();
            for (int y = 0; y < Constants.BuildMaxHeight; y++)
            {
                // 判断fixHeight
                if(preset.fixedHeight != -1 && preset.fixedHeight != y)
                {
                    continue; // 如果有固定高度，跳过
                }
                
                // 判断超不超边界
                var minPoint = preset.minPoint + new Vector3Int(x, y, z);
                var maxPoint = preset.maxPoint + new Vector3Int(x, y, z);
                if (minPoint.x < 0 || minPoint.z < 0 || maxPoint.x >= m_width || maxPoint.z >= m_height)
                {
                    continue; // 如果超出边界，跳过
                }
                if(minPoint.y < 0 || maxPoint.y >= Constants.BuildMaxHeight)
                {
                    continue; // 如果超出高度，跳过
                }
                
                
                //判断connect
                if (preset.connectToIsland && !IsConnectToIsland(x, y, z, preset.connectedCubeTypes))
                {
                    continue; // 如果需要连接到岛屿，且不连接，则跳过
                }
                
                // 判断canReplace
                if (!preset.canReplace && IsPresetOverlapIsland(new Vector3Int(x, y, z), preset))
                {
                    continue;
                }
                
                availableHeights.Add(y);
            }
            
            if (availableHeights.Count == 0)
            {
                // 没有可用高度
                return -1;
            }

            return Util.GetRandomElement(availableHeights);
        }
        
        private bool IsConnectToIsland(int x, int y, int z, List<CubeType> connectCubetypes)
        {
            bool specifyCubetype = !connectCubetypes.Contains(CubeType.None);
            
            // 判断cell的周围6格有没有方块
            var cubeL = GetCellData(x - 1, z)?.GetCubeData(y);
            var cubeR = GetCellData(x + 1, z)?.GetCubeData(y);
            var cubeF = GetCellData(x, z - 1)?.GetCubeData(y);
            var cubeB = GetCellData(x, z + 1)?.GetCubeData(y);
            var cubeU = GetCellData(x, z)?.GetCubeData(y + 1);
            var cubeD = GetCellData(x, z)?.GetCubeData(y - 1);

            bool cubeTypeFlag = true;
            
            if (specifyCubetype)
            {
                cubeTypeFlag = (cubeL != null && connectCubetypes.Contains(cubeL.CubeType)) ||
                               (cubeR != null && connectCubetypes.Contains(cubeR.CubeType)) ||
                               (cubeF != null && connectCubetypes.Contains(cubeF.CubeType)) ||
                               (cubeB != null && connectCubetypes.Contains(cubeB.CubeType)) ||
                               (cubeU != null && connectCubetypes.Contains(cubeU.CubeType)) ||
                               (cubeD != null && connectCubetypes.Contains(cubeD.CubeType));
            }
            
            
            return cubeTypeFlag && 
                   ((cubeL != null && cubeL.CubeType != CubeType.None) ||
                    (cubeR != null && cubeR.CubeType != CubeType.None) ||
                    (cubeF != null && cubeF.CubeType != CubeType.None) ||
                    (cubeB != null && cubeB.CubeType != CubeType.None) ||
                    (cubeU != null && cubeU.CubeType != CubeType.None) ||
                    (cubeD != null && cubeD.CubeType != CubeType.None));
        }

        private bool IsPresetOverlapIsland(Vector3Int cellIndex, CubePresetSO preset)
        {
            List<Vector3Int> presetCubesIndexes = new List<Vector3Int>();
            foreach (var cube in preset.cubes)
            {
                presetCubesIndexes.Add(cube.position + cellIndex);
            }

            foreach (var index in presetCubesIndexes)
            {
                var worldCube = GetCellData(index.x, index.z)?.GetCubeData(index.y);
                if(worldCube != null && worldCube.CubeType != CubeType.None)
                {
                    // 有方块重叠
                    return true;
                }
            }

            return false;
        }
        
        public CellData GetCellData(int x,int z)
        {
            if (m_cells == null) return null;
            if (x < 0 || x >= m_width || z < 0 || z >= m_height)
            {
                return null;
            }
            return m_cells[x, z];
        }
    }
}
