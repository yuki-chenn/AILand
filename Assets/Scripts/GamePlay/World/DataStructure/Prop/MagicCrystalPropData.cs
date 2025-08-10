using UnityEngine;

namespace AILand.GamePlay.World
{
    public class MagicCrystalPropData : PropData
    {
        // 是否是天然生成的
        private bool m_isNatural;
        public bool IsNatural
        {
            get => m_isNatural;
            set => m_isNatural = value;
        }

        // 是否已经被充能
        private bool m_isCharged;
        public bool IsCharged
        {
            get => m_isCharged;
            set => m_isCharged = value;
        }

        // 是否已激活（已经创建过岛屿）
        private bool m_isActive;
        public bool IsActive
        {
            get => m_isActive;
            set => m_isActive = value;
        }
        
        private ElementalEnergy m_energy;

        public ElementalEnergy Energy
        {
            set => m_energy = value;
            get => m_energy;
        }
        
        public MagicCrystalPropData(BlockData blockData, PropType propType, Vector3Int index, Quaternion rotation) : base(blockData, propType, index, rotation)
        {
        }
    }
}