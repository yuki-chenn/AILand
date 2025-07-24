using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AILand.System.Base;
using AILand.System.EventSystem;
using AILand.Utils;
using UnityEngine;
using EventType = AILand.System.EventSystem.EventType;



namespace AILand.GamePlay.World
{
    /// <summary>
    /// 控制世界的生成、动态加载卸载
    /// </summary>
    public class WorldManager : Singleton<WorldManager>
    {
        [Header("渲染设置")] public int blockLoadRange = 1;
        public int 每帧加载最多加载的方块数量 = 20;
        public int sight = 30;

        public List<GameObject> cubePrefabs;
        
        
        private WorldData m_worldData;
        private List<CellData> m_loadedCells = new List<CellData>();
        



        private int m_blockWidth => Constants.BlockWidth;
        private int m_blockHeight => Constants.BlockHeight;


        // 上一次所在的block
        private int m_lastBlockID = int.MinValue;

        protected override void Awake()
        {
            base.Awake();

            EventCenter.AddListener<int, float[,]>(EventType.PlayerCreateIsland, PlayerCreateIsland);

            m_worldData = new WorldData();
        }

        private void OnDestroy()
        {
            EventCenter.RemoveListener<int, float[,]>(EventType.PlayerCreateIsland, PlayerCreateIsland);
        }

        public bool CreateBlock(int blockId)
        {
            if (m_worldData.ContainsBlock(blockId))
            {
                Debug.LogError($"already exist {blockId}");
                return false;
            }

            // TODO 考虑玩家创建还是自然生成
            var block = new BlockData(blockId, m_blockWidth, m_blockHeight);
            m_worldData.AddBlock(blockId, block);

            return true;
        }

        public void PlayerCreateIsland(int blockID, float[,] noiseMap)
        {
            // TODO：传送玩家
            var playerController = GameManager.Instance.player.GetComponent<CharacterController>();
            if (playerController != null)
            {
                playerController.enabled = false;
                GameManager.Instance.player.transform.position = new Vector3(100, 100, 100);
                playerController.enabled = true;
            }
            
            var block = m_worldData.GetBlock(blockID);
            if (block == null)
            {
                Debug.LogError($"Block {blockID} does not exist.");
                return;
            }


            PCGIslandInfo islandInfo = new PCGIslandInfo(
                threshold: 0.3f,
                heightMap: noiseMap,
                generateMaxHeight: 64,
                heightMapFunc: x => Mathf.RoundToInt(3.15f * x - 9.53f * x * x + 71.05f * x * x * x)
            );

            islandInfo.cubeDistributions = new List<CubeDistribution>
            {
                new CubeDistribution { lowerBound = 5, cubeType = CubeType.Sand },
                new CubeDistribution { lowerBound = 20, cubeType = CubeType.Dirt },
                new CubeDistribution { lowerBound = 50, cubeType = CubeType.Stone },
                new CubeDistribution { lowerBound = 64, cubeType = CubeType.Snow }
            };

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


        private void Update()
        {
            // 获取玩家位置
            var playerPosition = GameManager.Instance.player.transform.position;

            // 载入周围的Block
            LoadBlocksAroundPlayer(playerPosition);
            
            // 加载玩家附近区域的方块
            LoadCubesAroundPlayer(playerPosition);
            
        }


        #region 载入地图

        // 根据玩家的位置，载入Block
        public void LoadBlocksAroundPlayer(Vector3 playerPosition)
        {

            int curBlockID = Util.GetBlockIDByWorldPosition(playerPosition, m_blockWidth, m_blockHeight);
            if (curBlockID != m_lastBlockID || m_lastBlockID == int.MinValue)
            {
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
                    // block.Unload();
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
                StartCoroutine(LoadBlockLowTerrainTextureCoroutine(blockID));
                
                yield break;
            }
        }
        
        IEnumerator LoadBlockLowTerrainTextureCoroutine(int blockID)
        {
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
                    Color heightColor = new Color((height & 7) / 7.0f, ((height >> 3) & 7) / 7.0f,
                        ((height >> 6) & 7) / 7.0f);
                    if(height > 0) Debug.Log($"height:{height},{heightColor.ToString()}");
                    Color mapColor = block.Cells[x, z].TopCube?.CubeType switch
                    {
                        CubeType.Sand => Color.yellow,
                        CubeType.Dirt => new Color(0.545f, 0.271f, 0.075f),
                        CubeType.Stone => Color.gray,
                        CubeType.Snow => Color.white,
                        _ => Color.clear
                    };
                    heightMap.SetPixel(z, m_blockHeight - 1 - x, heightColor);
                    colorMap.SetPixel(z, m_blockHeight - 1 - x, mapColor);
                }
                yield return 1;
            }

            yield return 1;
            colorMap.Apply();
            
            yield return 1;
            heightMap.Apply();
            
            block.BlockComponent.SetLowTerrainTexture(colorMap, heightMap);
        }

        // 根据玩家的位置，载入周围的Cube
        public void LoadCubesAroundPlayer(Vector3 playerPosition)
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
                if (!isCellInSight(cell, localIndex))
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
                    int sx = localIndex.x + dx;
                    int sz = localIndex.y + dz;
                    
                    var cell = block.Cells[sx, sz];
                    if (cell != null && !m_loadedCells.Contains(cell))
                    {
                        cell.Load();
                        m_loadedCells.Add(cell);
                    }
                }
            }
            
            
        }
        
        private bool isCellInSight(CellData cell, Vector2Int localIndex)
        {
            // 检查cell是否在视野范围内
            int dx = cell.Index.x - localIndex.x;
            int dz = cell.Index.y - localIndex.y;
            
            return Math.Abs(dx) <= sight && Math.Abs(dz) <= sight;
        }

        
        #endregion



    }
}
