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

    private WorldData m_worldData;



    private int m_blockWidth => Constants.BlockWidth;
    private int m_blockHeight => Constants.BlockHeight;
    
    
    protected override void Awake()
    {
        base.Awake();
        
        EventCenter.AddListener<int, float[,]>(EventType.PlayerCreateIsland, PlayerCreateIsland);
        
        m_worldData = new WorldData();
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
        
        // TODO
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
        
        
        
    }

    
    
    
    
    
    
}
