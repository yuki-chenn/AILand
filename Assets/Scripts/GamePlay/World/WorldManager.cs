using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AILand.System.SOManager;
using AILand.GamePlay.World.Cube;
using AILand.GamePlay.World.Prop;
using AILand.System.Base;
using AILand.System.EventSystem;
using AILand.Utils;
using UnityEngine;
using EventType = AILand.System.EventSystem.EventType;
using Random = UnityEngine.Random;


namespace AILand.GamePlay.World
{
    /// <summary>
    /// 控制世界的生成、动态加载卸载
    /// </summary>
    public class WorldManager : Singleton<WorldManager>
    {
        [Header("渲染设置")] public int blockLoadRange = 1;
        public int sight = 30;
        public int border = 2;

        private WorldData m_worldData;
        
        private List<CellData> m_loadedCells = new List<CellData>();


        private int m_blockWidth => Constants.BlockWidth;
        private int m_blockHeight => Constants.BlockHeight;

        // 上一次所在的block
        private int m_lastBlockID = int.MinValue;
        private Dictionary<int,bool> m_isLoadLowTerrain = new Dictionary<int, bool>();

        protected override void Awake()
        {
            base.Awake();
            BindListeners();
            InitData();
        }

        private void InitData()
        {
            m_worldData = new WorldData(GameManager.Instance.WFCConfigSO);
        }
        
        private void BindListeners()
        {
            EventCenter.AddListener<int, float[,], int[,]>(EventType.PlayerCreateIsland, PlayerCreateIsland);
        }
        
        private void UnbindListeners()
        {
            EventCenter.RemoveListener<int, float[,], int[,]>(EventType.PlayerCreateIsland, PlayerCreateIsland);
        }
        
        
        
        private void Update()
        {
            // 获取玩家位置
            var playerPosition = GameManager.Instance.player.transform.position;

            // 载入周围的Block
            LoadBlocksAroundPlayer(playerPosition);
            
            // 加载玩家附近区域的方块
            LoadCubesAroundPlayer(playerPosition, Time.frameCount % 60 == 0);
            
            // 加载玩家附近区域的道具
            LoadPropsAroundPlayer(playerPosition, Time.frameCount % 60 == 0);
            
            // 更新低地形纹理
            UpdateLowTerrain(GameManager.Instance.player.transform.position);
        }

        private void OnDestroy()
        {
            UnbindListeners();
        }

        public bool CreateBlock(int blockId)
        {
            if (m_worldData.ContainsBlock(blockId))
            {
                Debug.LogError($"already exist {blockId}");
                return false;
            }

            // 获取当前block的IslandType
            var islandType = m_worldData.GetBlockIslandType(blockId);

            var block = new BlockData(blockId, m_blockWidth, m_blockHeight, islandType);
            m_worldData.AddBlock(blockId, block);

            if(islandType != IslandType.Custom && islandType != IslandType.None && islandType != IslandType.Water)
            {
                // 自然生成的岛屿
                PCGIslandInfo islandInfo = new PCGIslandInfo();
                
                var islandConfig = Util.CloneSO(SOManager.Instance.islandConfigDict[islandType]);
                islandInfo.islandConfig = islandConfig;
                
                // 随机属性设置
                var noiseMap = Util.GetRandomTerrainNoiseMap(m_blockWidth, m_blockHeight, 
                    80, Util.GetRandomInRange(150,250), Util.GetRandomInRange(3,6),Util.GetRandomInRange(4,6));
                islandInfo.shapeConfig = new ShapeConfig(islandConfig.waterThreshold, noiseMap, islandConfig.maxHeight, islandConfig.noiseToHeightCurve);
                
                block.CreateIsland(islandInfo);
            }

            return true;
        }

        public void PlayerCreateIsland(int blockID, float[,] noiseMap, int[,] cellTypeMap)
        {
            var block = m_worldData.GetBlock(blockID);
            if (block == null)
            {
                Debug.LogError($"Block {blockID} does not exist.");
                return;
            }
            
            if(block.IslandType!= IslandType.Custom)
            {
                Debug.LogError($"Block {blockID} is not a custom island type.");
                return;
            }
            
            // 传送玩家
            Vector3 pos = block.WorldPosition + new Vector3(m_blockWidth / 2, 50, m_blockHeight / 2);
            GameManager.Instance.Teleport(pos);
            
            
            PCGIslandInfo islandInfo = new PCGIslandInfo();
            
            var islandConfig = Util.CloneSO(SOManager.Instance.islandConfigDict[block.IslandType]);
            islandConfig.SetPaintCellType(cellTypeMap);
            islandInfo.islandConfig = islandConfig;
            
            islandInfo.shapeConfig = new ShapeConfig(islandConfig.waterThreshold, noiseMap, islandConfig.maxHeight, islandConfig.noiseToHeightCurve);
            var ok = block.CreateIsland(islandInfo);

            if (ok)
            {
                Debug.Log($"Block {blockID} island created successfully by player.");
                StartCoroutine(LoadBlockCoroutine(blockID));
            }
            else
            {
                Debug.LogError($"Failed to create island for Block {blockID} by player.");
            }

        }

        public bool DestroyCube(BaseCube cube)
        {
            if(cube == null)
            {
                Debug.LogWarning("DestoryBlock Cube is null");
                return false;
            }
            
            var cubeTrans = cube.transform;
            
            Debug.Log($"Clicked on cube at position: {cubeTrans.position}");
            int x = Mathf.RoundToInt(cubeTrans.localPosition.x);
            int y = Mathf.RoundToInt(cubeTrans.localPosition.y);
            int z = Mathf.RoundToInt(cubeTrans.localPosition.z);
            
            // TODO : 会有破坏不了的方块
            
            var block = m_worldData.GetBlock(Util.GetBlockIDByWorldPosition(cubeTrans.position, m_blockWidth, m_blockHeight));

            var cubeData = block.GetCellData(x, z)?.GetCubeData(y);
            if(cubeData == null || cubeData.CubeType == CubeType.None)
            {
                // 没有方块
                Debug.LogWarning($"No cube found at position ({x}, {y}, {z}) to destroy.");
                return false;
            }
            // TODO: 范围导致无法破坏
            
            // 方块属性无法破坏
            if (!cubeData.CubeConfig.canDestroy)
            {
                Debug.Log("Cannot destroy this cube.");
                return false;
                
            }
            
            return block.DestroyCube(x, y, z);;
        }

        public bool PlaceCube(Vector3Int worldPosIndex, CubeType cubeType, BaseCube cubeFocus)
        {
            // TODO : 检查是否能够放置方块
            if (!SOManager.Instance.cubeConfigDict[cubeFocus.CubeType].canPlaceCubeOn)
            {
                Debug.Log($"Cannot place cube of type {cubeFocus.CubeType}");
                return false;
            }
            
            // 如果此时玩家和方块碰撞，直接返回
            Vector3 playerPosition = GameManager.Instance.player.transform.position;
            // 玩家两格高
            Vector3Int playerIndex0 = new Vector3Int(
                Mathf.RoundToInt(playerPosition.x),
                Mathf.RoundToInt(playerPosition.y + 1),
                Mathf.RoundToInt(playerPosition.z));
            Vector3Int playerIndex1 = new Vector3Int(
                Mathf.RoundToInt(playerPosition.x),
                Mathf.RoundToInt(playerPosition.y + 2),
                Mathf.RoundToInt(playerPosition.z));
            
            if (playerIndex0.Equals(worldPosIndex) || playerIndex1.Equals(worldPosIndex))
            {
                Debug.LogWarning("Player is standing on the block, cannot place cube.");
                return false;
            }
            
            var block = m_worldData.GetBlock(GameManager.Instance.CurBlockId);
            
            // 根据block将坐标转换为本地坐标
            Vector3Int localPosIndex = worldPosIndex - Vector3Int.RoundToInt(block.WorldPosition);

            
            if(localPosIndex.x < 0 || localPosIndex.x >= m_blockWidth ||
               localPosIndex.y < 0 || localPosIndex.y >= Constants.BuildMaxHeight ||
               localPosIndex.z < 0 || localPosIndex.z >= m_blockHeight)
            {
                // 超出范围
                return false;
            }
            
            
            

            return block.AddCube(localPosIndex.x, localPosIndex.y, localPosIndex.z, cubeType, 0);
        }
        
        public MagicCrystal GetBlockMagicCrystalInstance()
        {
            var block = m_worldData.GetBlock(GameManager.Instance.CurBlockId);
            return block?.MagicCrystalComp;
        }

        #region 载入地图

        public void ForceLoadAroundPosition(Vector3 pos)
        {
            LoadBlocksAroundPlayer(pos);
            LoadCubesAroundPlayer(pos, true);
            LoadPropsAroundPlayer(pos, true);
            UpdateLowTerrain(pos);
        }
        
        // 根据玩家的位置，载入Block
        public void LoadBlocksAroundPlayer(Vector3 playerPosition)
        {

            int curBlockID = Util.GetBlockIDByWorldPosition(playerPosition, m_blockWidth, m_blockHeight);
            if (curBlockID != m_lastBlockID || m_lastBlockID == int.MinValue)
            {
                // TODO:将原先block的所有东西都卸载了
                // for (int i = m_loadedCells.Count - 1; i >= 0; i--)
                // {
                //     var cell = m_loadedCells[i];
                //     cell.Unload();
                //     m_loadedCells.RemoveAt(i);
                // }
                
                Debug.Log($"Player moved to block {curBlockID}, loading blocks around it.");

                List<int> blocksToLoad = new List<int>();
                List<int> blocksToUnload = new List<int>();

                Util.GetAroundBlockID(curBlockID, blockLoadRange, ref blocksToLoad);
                Util.GetAroundBlockID(m_lastBlockID, blockLoadRange, ref blocksToUnload);

                var union = blocksToLoad.Intersect(blocksToUnload).ToList();
                blocksToLoad = blocksToLoad.Except(union).ToList();
                blocksToUnload = blocksToUnload.Except(union).ToList();

                foreach (var blockId in blocksToLoad)
                {
                    // block.Load();
                    if (!m_worldData.ContainsBlock(blockId))
                    {
                        CreateBlock(blockId);
                    }

                    StartCoroutine(LoadBlockCoroutine(blockId));
                }

                foreach (var block in blocksToUnload)
                {
                     //block.Unload();
                }



                m_lastBlockID = curBlockID;
            }
        }
        
        IEnumerator LoadBlockCoroutine(int blockID)
        {
            var block = m_worldData.GetBlock(blockID);

            block.InstanceGo.SetActive(true);
            block.BlockComponent.SetBlockActive(true, block.IsCreated);
            
            if (!block.IsCreated)
            {
                // 岛屿数据还没有生成

            }
            else
            {
                // 岛屿数据已经生成
                
                // 加载显示远处地形
                var playerPos = GameManager.Instance.player.transform.position;
                if (!m_isLoadLowTerrain.GetValueOrDefault(blockID,false)) StartCoroutine(LoadBlockLowTerrainTextureCoroutine(blockID, playerPos, sight));
                
                yield break;
            }
        }
        
        IEnumerator LoadBlockLowTerrainTextureCoroutine(int blockID, Vector3 playerPos, int sight)
        {
            m_isLoadLowTerrain[blockID] = true;
            var block = m_worldData.GetBlock(blockID);
            if (block == null)
            {
                Debug.LogError($"Block {blockID} does not exist.");
                yield break;
            }
            
            Texture2D heightMap = new Texture2D(200, 200, TextureFormat.RGB24, false, true);
            Texture2D colorMap = new Texture2D(200, 200, TextureFormat.RGB24, false, true);
            for (int x = 0; x < m_blockWidth; x++)
            {
                for (int z = 0; z < m_blockHeight; z++)
                {
                    int height = block.Cells[x, z].Height;
                    Color heightColor = new Color(
                        Mathf.Clamp(height, 0F, 5F) / 5F, 
                        Mathf.Clamp(height - 5F, 0F, 10F) / 10F, 
                        Mathf.Clamp(height - 15F, 0F, 35F) / 35F);
                    int cy = Mathf.RoundToInt(playerPos.y);
                    var cubeConfig = block.Cells[x, z].TopCube?.CubeConfig;
                    Color mapColor = cubeConfig == null ? Color.clear :
                        (cy < height - 2 ? cubeConfig.SideColor : cubeConfig.TopColor);
                    heightMap.SetPixel(x, z, heightColor);
                    colorMap.SetPixel(x, z, mapColor);
                }
                yield return 1;
            }

            yield return 1;
            colorMap.Apply();
            
            yield return 1;
            heightMap.Apply();
            
            block.BlockComponent.SetLowTerrainTexture(colorMap, heightMap, playerPos, sight);
            m_isLoadLowTerrain[blockID] = false;
        }

        // 根据玩家的位置，载入周围的Cube
        public void LoadCubesAroundPlayer(Vector3 playerPosition, bool forceAll = false)
        {
            var localIndex = new Vector2Int();
            var blockIndex = Util.GetBlockIndexByWorldPosition(playerPosition, m_blockWidth, m_blockHeight,ref localIndex);
            
            // 获取玩家所在的Block
            var block = m_worldData.GetBlock(blockIndex);
            
            if(!block.IsCreated) return;
            
            // 卸载不在视野内的cell
            for (int i = m_loadedCells.Count - 1; i >= 0; i--)
            {
                var cell = m_loadedCells[i];
                if (!IsCellInSight(cell, localIndex))
                {
                    cell.Unload();
                    m_loadedCells.RemoveAt(i);
                }
            }
            
            
            // 加载在视野内的cell
            for(int dx = -sight; dx <= sight; dx++)
            {
                for(int dz = -sight; dz <= sight; dz++)
                {
                    // 每次只重新刷新边缘的方块,而不全部重渲染
                    if(forceAll || 
                       dx < -sight + border || dx > sight - border ||
                       dz < -sight + border || dz > sight - border )
                    {
                        int sx = localIndex.x + dx;
                        int sz = localIndex.y + dz;
                    
                        if(sx < 0 || sx >= m_blockWidth || sz < 0 || sz >= m_blockHeight)
                        {
                            continue;
                        }
                        
                        var cell = block.Cells[sx, sz];
                        if (cell != null && !m_loadedCells.Contains(cell))
                        {
                            cell.Load();
                            m_loadedCells.Add(cell);
                        }
                    }
                }
            }
            
            
        }

        public void LoadPropsAroundPlayer(Vector3 playerPosition, bool forceAll = false)
        {
            var localIndex = new Vector2Int();
            var blockIndex = Util.GetBlockIndexByWorldPosition(playerPosition, m_blockWidth, m_blockHeight,ref localIndex);
            
            // 获取玩家所在的Block
            var block = m_worldData.GetBlock(blockIndex);
            
            if(!block.IsCreated) return;
            
            // 卸载不在视野内的prop,加载在视野内的prop
            foreach (var prop in block.Props)
            {
                if (!IsPropInSight(prop, localIndex))
                {
                    prop.Unload();
                }
                else
                {
                    prop.Load();
                }
            }
        }
        
        
        private bool IsCellInSight(CellData cell, Vector2Int localIndex)
        {
            // 检查cell是否在视野范围内
            int dx = cell.Index.x - localIndex.x;
            int dz = cell.Index.y - localIndex.y;
            
            return Math.Abs(dx) <= sight && Math.Abs(dz) <= sight;
        }

        private bool IsPropInSight(PropData prop, Vector2Int localIndex)
        {
            // 检查cell是否在视野范围内
            int dx = 0;
            int dz = 0;

            if (!prop.IsLoad)
            {
                dx = prop.Index.x - localIndex.x; 
                dz = prop.Index.z - localIndex.y;
            }
            else
            {
                if(prop.InstanceGo == null)
                {
                    Debug.LogWarning($"Prop {prop.Index} is loaded but InstanceGo is null.");
                    return false;
                }
                dx = Mathf.RoundToInt(prop.InstanceGo.transform.localPosition.x - localIndex.x);
                dz = Mathf.RoundToInt(prop.InstanceGo.transform.localPosition.z - localIndex.y);
            }
            
            return Math.Abs(dx) <= sight && Math.Abs(dz) <= sight;
        }
        // 根据玩家位置修改lowTerrain
        public void UpdateLowTerrain(Vector3 playerPosition)
        {
            var blockId = Util.GetBlockIDByWorldPosition(playerPosition, m_blockWidth, m_blockHeight);
            var block = m_worldData.GetBlock(blockId);
            if (block != null && block.IsCreated)
            {
                block.BlockComponent.UpdateLowTerrain(playerPosition, sight);
                if(Time.frameCount % 300 == 0 && !m_isLoadLowTerrain[blockId])
                {
                    var playerPos = GameManager.Instance.player.transform.position;
                    // 每隔一段时间重新加载低地形纹理
                    StartCoroutine(LoadBlockLowTerrainTextureCoroutine(blockId, playerPos, sight));
                }
            }
        }
        
        #endregion


        #region 路经检查

        
        /// <summary>
        /// 判断有没有会碰撞的方块
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public bool HasCollideCube(Vector3 position)
        {
            Vector2Int localIndex = new Vector2Int();
            var blockIndex = Util.GetBlockIndexByWorldPosition(position, m_blockWidth, m_blockHeight,ref localIndex);

            var block = m_worldData.GetBlock(blockIndex);
            if (block == null)
            {
                Debug.LogWarning($"Block {blockIndex} does not exist.");
                return false;
            }
            
            var cell = block.GetCellData(localIndex.x, localIndex.y);
            
            int y = Mathf.CeilToInt(position.y);
            var cubeData = cell?.GetCubeData(y);
            
            if (cubeData == null || cubeData.CubeType == CubeType.None) return false;
            
            return cubeData.CubeConfig.hasCollision;
        }

        #endregion

        //#region ONGUI

        //private string inputX = "0";
        //private string inputY = "10";
        //private string inputZ = "0";

        //private void OnGUI()
        //{
        //    // 输入一个三维坐标并传送
        //    GUILayout.BeginArea(new Rect(10, 10, 300, 120));
        //    GUILayout.BeginVertical("Box");
    
        //    GUILayout.Label("传送坐标：");
    
        //    GUILayout.BeginHorizontal();
        //    GUILayout.Label("X:", GUILayout.Width(20));
        //    inputX = GUILayout.TextField(inputX, GUILayout.Width(60));
        //    GUILayout.Label("Y:", GUILayout.Width(20));
        //    inputY = GUILayout.TextField(inputY, GUILayout.Width(60));
        //    GUILayout.Label("Z:", GUILayout.Width(20));
        //    inputZ = GUILayout.TextField(inputZ, GUILayout.Width(60));
        //    GUILayout.EndHorizontal();
    
        //    if (GUILayout.Button("传送"))
        //    {
        //        if (float.TryParse(inputX, out float x) && 
        //            float.TryParse(inputY, out float y) && 
        //            float.TryParse(inputZ, out float z))
        //        {
        //            TeleportPlayer(new Vector3(x, y, z));
        //        }
        //        else
        //        {
        //            Debug.LogWarning("请输入有效的数字坐标！");
        //        }
        //    }
    
        //    GUILayout.EndVertical();
        //    GUILayout.EndArea();
        //}

        //private void TeleportPlayer(Vector3 position)
        //{
        //    var playerController = GameManager.Instance.player.GetComponent<CharacterController>();
        //    if (playerController != null)
        //    {
        //        playerController.enabled = false;
        //        GameManager.Instance.player.transform.position = position;
        //        playerController.enabled = true;
        //        Debug.Log($"玩家传送到坐标：{position}");
        //    }
        //    else
        //    {
        //        Debug.LogError("未找到玩家的CharacterController组件！");
        //    }
        //}

        //#endregion


        
    }
}
