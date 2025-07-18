
using System.Collections.Generic;


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
            if (m_blocks.TryGetValue(blockId, out BlockData blockData))
            {
                return blockData;
            }

            return null;
        }

        public bool AddBlock(int blockId, BlockData blockData)
        {
            if (m_blocks.ContainsKey(blockId))
            {
                return false;
            }

            m_blocks[blockId] = blockData;
            return true;
        }

        public bool ContainsBlock(int blockId)
        {
            return m_blocks.ContainsKey(blockId);
        }
    }
}
