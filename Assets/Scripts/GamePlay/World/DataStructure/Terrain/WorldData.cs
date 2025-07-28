
using System.Collections.Generic;
using System.Numerics;
using AILand.Utils;
using UnityEngine;


namespace AILand.GamePlay.World
{
    public class WorldData
    {
        // 所有block的数据，key为区块的唯一ID
        private Dictionary<int, BlockData> m_blocks;

        public WorldData()
        {
            m_blocks = new Dictionary<int, BlockData>();
        }
        
        public BlockData GetBlock(int blockId)
        {
            return m_blocks.GetValueOrDefault(blockId);
        }

        public BlockData GetBlock(Vector2Int index)
        {
            return GetBlock(Util.GetBlockID(index));
        }

        public bool AddBlock(int blockId, BlockData blockData)
        {
            return m_blocks.TryAdd(blockId, blockData);
        }

        public bool ContainsBlock(int blockId)
        {
            return m_blocks.ContainsKey(blockId);
        }
    }
}
