using System;
using System.Collections.Generic;
using UnityEngine;

namespace AILand.GamePlay.World
{
    [Serializable]
    public struct AdjacencyRule
    {
        // 当前类型
        public IslandType type;
        // 8格 允许出现的类型
        public List<IslandType> forbid8;
        // 4格 允许出现的类型
        public List<IslandType> forbid4;
        // 周围8格的水类型数量
        public int leastWaterTypeCount;
    }


    [CreateAssetMenu(fileName = "WFCConfig_", menuName = "创建波函数坍缩配置", order = 0)]
    public class WFCConfigSO : ScriptableObject
    {
        public List<AdjacencyRule> adjacencyRule;
    }
}