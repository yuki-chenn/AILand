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
        public int border = 2;

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
                generateMaxHeight: 8,
                heightMapFunc: x => Mathf.RoundToInt(10 * x - 2)
            );

            islandInfo.cubeDistributions = new List<CubeDistribution>
            {
                new CubeDistribution { lowerBound = 2, cubeType = CubeType.Sand },
                new CubeDistribution { lowerBound = 4, cubeType = CubeType.Dirt },
                new CubeDistribution { lowerBound = 7, cubeType = CubeType.Stone },
                new CubeDistribution { lowerBound = 8, cubeType = CubeType.Snow }
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
            LoadCubesAroundPlayer(playerPosition, Time.frameCount % 60 == 0);
            
            // 更新低地形纹理
            UpdateLowTerrain(GameManager.Instance.player.transform.position);

            // 按下alt键呼出鼠标
            if (Input.GetKeyDown(KeyCode.LeftAlt) || Input.GetKeyDown(KeyCode.RightAlt))
            {
                Cursor.lockState = Cursor.lockState == CursorLockMode.Locked ? CursorLockMode.None : CursorLockMode.Locked;
                Cursor.visible = !Cursor.visible;
            }
            
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
                    Color heightColor = new Color(
                        Mathf.Clamp(height, 0F, 5F) / 5F, 
                        Mathf.Clamp(height - 5F, 0F, 10F) / 10F, 
                        Mathf.Clamp(height - 15F, 0F, 35F) / 35F);
                    // TODO: 测试用
                    Color mapColor = block.Cells[x, z].TopCube?.CubeType switch
                    {
                        CubeType.Sand => Color.yellow,
                        CubeType.Dirt => new Color(0.545f, 0.271f, 0.075f),
                        CubeType.Stone => Color.gray,
                        CubeType.Snow => Color.white,
                        _ => Color.clear
                    };
                    heightMap.SetPixel(x, z, heightColor);
                    colorMap.SetPixel(x, z, mapColor);
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
        
        private bool isCellInSight(CellData cell, Vector2Int localIndex)
        {
            // 检查cell是否在视野范围内
            int dx = cell.Index.x - localIndex.x;
            int dz = cell.Index.y - localIndex.y;
            
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
            }
        }
        
        #endregion



        #region ONGUI

        private string inputX = "0";
        private string inputY = "10";
        private string inputZ = "0";

        private void OnGUI()
        {
            // 输入一个三维坐标并传送
            GUILayout.BeginArea(new Rect(10, 10, 300, 120));
            GUILayout.BeginVertical("Box");
    
            GUILayout.Label("传送坐标：");
    
            GUILayout.BeginHorizontal();
            GUILayout.Label("X:", GUILayout.Width(20));
            inputX = GUILayout.TextField(inputX, GUILayout.Width(60));
            GUILayout.Label("Y:", GUILayout.Width(20));
            inputY = GUILayout.TextField(inputY, GUILayout.Width(60));
            GUILayout.Label("Z:", GUILayout.Width(20));
            inputZ = GUILayout.TextField(inputZ, GUILayout.Width(60));
            GUILayout.EndHorizontal();
    
            if (GUILayout.Button("传送"))
            {
                if (float.TryParse(inputX, out float x) && 
                    float.TryParse(inputY, out float y) && 
                    float.TryParse(inputZ, out float z))
                {
                    TeleportPlayer(new Vector3(x, y, z));
                }
                else
                {
                    Debug.LogWarning("请输入有效的数字坐标！");
                }
            }
    
            GUILayout.EndVertical();
            GUILayout.EndArea();
        }

        private void TeleportPlayer(Vector3 position)
        {
            var playerController = GameManager.Instance.player.GetComponent<CharacterController>();
            if (playerController != null)
            {
                playerController.enabled = false;
                GameManager.Instance.player.transform.position = position;
                playerController.enabled = true;
                Debug.Log($"玩家传送到坐标：{position}");
            }
            else
            {
                Debug.LogError("未找到玩家的CharacterController组件！");
            }
        }

        #endregion
        
        
    }
}
