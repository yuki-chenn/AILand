
using System.Collections.Generic;
using System.Numerics;
using AILand.Utils;
using UnityEngine;


namespace AILand.GamePlay.World
{
    public class WorldData
    {
        // 预先生成的block的IslandType
        private WFCBlock m_WFCBlock;
        private WFCConfigSO m_WFCConfig;
        private Dictionary<int, IslandType> m_islandTypeDic;
        
        
        // 所有block的数据，key为区块的唯一ID
        private Dictionary<int, BlockData> m_blockDic;
        
        

        public WorldData(WFCConfigSO WFCConfig)
        {
            // WFC Block type初始化
            m_WFCConfig = WFCConfig;
            m_islandTypeDic = new Dictionary<int, IslandType>();
            m_WFCBlock = new WFCBlock(m_WFCConfig);

            // block数据初始化
            m_blockDic = new Dictionary<int, BlockData>();
        }

        public IslandType GetBlockIslandType(int blockId)
        {
            if (!m_islandTypeDic.ContainsKey(blockId))
            {
                m_islandTypeDic[blockId] = m_WFCBlock.Generate(blockId, WorldManager.Instance.blockLoadRange + 1); ;
            }


            return m_islandTypeDic[blockId];
        }
        
        public BlockData GetBlock(int blockId)
        {
            return m_blockDic.GetValueOrDefault(blockId);
        }

        public BlockData GetBlock(Vector2Int index)
        {
            return GetBlock(Util.GetBlockID(index));
        }

        public bool AddBlock(int blockId, BlockData blockData)
        {
            return m_blockDic.TryAdd(blockId, blockData);
        }

        public bool ContainsBlock(int blockId)
        {
            return m_blockDic.ContainsKey(blockId);
        }
    }
}
