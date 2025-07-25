using System;
using System.Collections.Generic;
using UnityEngine;

namespace AILand.GamePlay.World
{
    [Serializable]
    public struct Distribution
    {
        public CubeType cubeType;   // 方块类型
        public float weight;        // 权重值
        public float disturb;       // 扰动值，用于生成时的随机扰动
    }
    
    
    [CreateAssetMenu(fileName ="HeightDistributionConfig_" ,menuName ="创建高度方块分布配置",order = 1)]
    public class HeightDistributionConfigSO : ScriptableObject
    {
        public CellType cellType;
        
        public List<Distribution> distributions;
    }
}