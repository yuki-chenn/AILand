using System.Collections.Generic;
using AILand.GamePlay.World;
using AILand.System.Base;
using UnityEngine;

namespace System.SOManager
{
    public class SOManager : Singleton<SOManager>
    {
        [Header("每个Cell对应的高度分布配置")]
        public Dictionary<CellType, HeightDistributionConfigSO> heightDistributionDict =
            new Dictionary<CellType, HeightDistributionConfigSO>();
        
        [Header("每个岛屿类型对应的配置")]
        public Dictionary<IslandType, IslandConfigSO> islandConfigDict =
            new Dictionary<IslandType, IslandConfigSO>();
        
    }
}