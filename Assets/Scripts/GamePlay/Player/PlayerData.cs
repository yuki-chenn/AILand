using System.Collections.Generic;
using AILand.GamePlay.World;
using AILand.GamePlay.World.Prop;

namespace GamePlay.Player
{
    public class PlayerData
    {
        // 玩家收集的元素能量
        private ElementalEnergy m_elementalEnergy;
        
        // 玩家持有的道具
        private List<BaseProp> m_inventory;
        
    }
}