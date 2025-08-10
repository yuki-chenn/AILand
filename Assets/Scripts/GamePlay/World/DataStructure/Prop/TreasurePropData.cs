using UnityEngine;

namespace AILand.GamePlay.World
{
    public class TreasurePropData : PropData
    {
        // 触发召唤怪物
        private bool m_isSummoned = false;
        public bool IsSummoned
        {
            get => m_isSummoned;
            set => m_isSummoned = value;
        }
        
        // 开启
        private bool m_isOpen = false;
        public bool IsOpen
        {
            get => m_isOpen;
            set => m_isOpen = value;
        }
        
        public TreasurePropData(BlockData blockData, PropType propType, Vector3Int index, Quaternion rotation) : base(blockData, propType, index, rotation)
        {
        }
    }
}