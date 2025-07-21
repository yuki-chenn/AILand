using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AILand.System.EventSystem;
using AILand.Utils;
using UnityEngine;
using EventType = AILand.System.EventSystem.EventType;


/// <summary>
/// 控制世界的生成、动态加载卸载
/// </summary>
namespace AILand.GamePlay.World
{
    public class WorldManager : Singleton<WorldManager>
    {
        [Header("渲染设置")] public int blockLoadRange = 1;
        public int 每帧加载最多加载的方块数量 = 20;
        public int 视野范围 = 30;

        public GameObject blockPrefab;

        public List<GameObject> cubePrefabs;


        private WorldData m_worldData;



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

            if (!block.IsCreated)
            {
                if (block.IsPlayerCreated)
                {
                    block.BlockComponent.ShowGeneratePlatform(true);
                }
                else
                {
                    // TODO 生成PCG岛屿数据

                }
            }
            else
            {
                block.BlockComponent.ShowGeneratePlatform(false);
                // 加载显示
                
                yield break;
            }
        }

        #endregion



    }
}
