using System;
using System.Collections;
using System.Collections.Generic;
using AILand.GamePlay.World;
using AILand.System.EventSystem;
using AILand.Utils;
using UnityEngine;
using EventType = AILand.System.EventSystem.EventType;



/// <summary>
/// 控制世界的生成、动态加载卸载
/// </summary>
public class WorldManager : Singleton<WorldManager>
{
    [Header("渲染设置")]
    public int 每帧加载最多加载的方块数量 = 20;
    public int 视野范围 = 30; 
    
    public List<GameObject> cubePrefabs;
    
    
    private WorldData m_worldData;



    private int m_blockWidth => Constants.BlockWidth;
    private int m_blockHeight => Constants.BlockHeight;
    
    
    protected override void Awake()
    {
        base.Awake();
        
        EventCenter.AddListener<int, float[,]>(EventType.PlayerCreateIsland, PlayerCreateIsland);
        
        m_worldData = new WorldData();
        
        
        // 为了测试我先加载一个Block
        CreateBlock(new Vector3(0, 0, 0));
        
        
    }

    private void OnDestroy()
    {
        EventCenter.RemoveListener<int, float[,]>(EventType.PlayerCreateIsland, PlayerCreateIsland);
    }

    
    public void InitializeFirstBlock()
    {
        
    }
    
    public bool CreateBlock(Vector3 position)
    {
        // 根据position计算需要生成的Block位置
        int blockId = Util.GetBlockIDByWorldPosition(position, m_blockWidth, m_blockHeight);

        if (m_worldData.ContainsBlock(blockId))
        {
            Debug.LogError($"already exist { blockId }");
            return false;
        }
        
        // TODO 暂时的
        var block = new BlockData(blockId, position, m_blockWidth, m_blockHeight);
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
        
        if(ok)
        {
            Debug.Log($"Block {blockID} island created successfully by player.");
            StartCoroutine(LoadBlockCoroutine(blockID));
        }
        else
        {
            Debug.LogError($"Failed to create island for Block {blockID} by player.");
        }
        
    }







    #region 载入地图

    IEnumerator LoadBlockCoroutine(int blockID)
    {
        int loadCount = 0;
        
        if (!m_worldData.ContainsBlock(blockID) || !m_worldData.GetBlock(blockID).IsCreated)
        {
            Debug.LogError($"Block {blockID} didn't exists or haven't created.");
            yield break;
        }

        var block = m_worldData.GetBlock(blockID);
        var blockCells = block.Cells;

        for (int x = 0; x < m_blockWidth; x++)
        {
            for (int z = 0; z < m_blockHeight; z++)
            {
                var cell = blockCells[x, z];
                if (cell == null) continue;
                
                for(int y=0;y<cell.Cubes.Count;y++)
                {
                    if(loadCount >= 每帧加载最多加载的方块数量)
                    {
                        yield return null; 
                        loadCount = 0; 
                    }
                    
                    var cube = cell.Cubes[y];
                    if (cube == null || cube.CubeType == CubeType.None) continue;

                    // 生成Cube
                    var cubeInstance = Instantiate(cubePrefabs[(int)cube.CubeType - 1],
                        block.WorldPosition + cell.LocalPosition + new Vector3(0, y, 0), 
                        Quaternion.identity);
                    
                    cubeInstance.name = $"Cube_{blockID}_{x}_{y}_{z}";

                    loadCount++;
                }
                
            }
        }



        yield return null;
    }

    #endregion


    
}
