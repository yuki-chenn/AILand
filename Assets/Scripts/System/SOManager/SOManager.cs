using System.Collections.Generic;
using AILand.GamePlay.World;
using AILand.System.Base;
using UnityEngine;

namespace System.SOManager
{
    [Serializable]
    public struct HeightDistribution
    {
        public CellType key;
        public HeightDistributionConfigSO value;
    }

    [Serializable]
    public struct IslandConfig
    {
        public IslandType key;
        public IslandConfigSO value;
    }



    public class SOManager : Singleton<SOManager>
    {
        [Header("每个Cell对应的高度分布配置")]
        public List<HeightDistribution> heightDistributions = new List<HeightDistribution>();

        [Header("每个岛屿类型对应的配置")]
        public List<IslandConfig> islandConfigs = new List<IslandConfig>();


        public Dictionary<CellType, HeightDistributionConfigSO> heightDistributionDict =
            new Dictionary<CellType, HeightDistributionConfigSO>();
        
        public Dictionary<IslandType, IslandConfigSO> islandConfigDict =
            new Dictionary<IslandType, IslandConfigSO>();

        protected override void Awake()
        {
            base.Awake();

            foreach(var kv in heightDistributions)
            {
                if (!heightDistributionDict.ContainsKey(kv.key))
                {
                    heightDistributionDict.Add(kv.key, kv.value);
                }
                else
                {
                    Debug.LogError($"HeightDistributionConfigSO for {kv.key} already exists!");
                }
            }

            foreach (var kv in islandConfigs)
            {
                if (!islandConfigDict.ContainsKey(kv.key))
                {
                    islandConfigDict.Add(kv.key, kv.value);
                }
                else
                {
                    Debug.LogError($"IslandConfigSO for {kv.key} already exists!");
                }
            }

        }

    }
}