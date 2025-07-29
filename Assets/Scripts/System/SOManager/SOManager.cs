using System;
using System.Collections.Generic;
using AILand.GamePlay.World;
using AILand.System.Base;
using AILand.Utils;
using UnityEngine;

namespace AILand.System.SOManager
{
    // 定义通用接口
    public interface IKeyValuePair<TKey, TValue>
    {
        TKey key { get; }
        TValue value { get; }
    }

    [Serializable]
    public struct HeightDistribution : IKeyValuePair<CellType, HeightDistributionConfigSO>
    {
        public CellType cellType;
        public HeightDistributionConfigSO so;
        
        CellType IKeyValuePair<CellType, HeightDistributionConfigSO>.key => cellType;
        HeightDistributionConfigSO IKeyValuePair<CellType, HeightDistributionConfigSO>.value => so;
    }

    [Serializable]
    public struct IslandConfig : IKeyValuePair<IslandType, IslandConfigSO>
    {
        public IslandType islandType;
        public IslandConfigSO so;
        
        IslandType IKeyValuePair<IslandType, IslandConfigSO>.key => islandType;
        IslandConfigSO IKeyValuePair<IslandType, IslandConfigSO>.value => so;
    }

    [Serializable]
    public struct CubeConfig : IKeyValuePair<CubeType, CubeConfigSO>
    {
        public CubeType cubeType;
        public CubeConfigSO so;
        
        CubeType IKeyValuePair<CubeType, CubeConfigSO>.key => cubeType;
        CubeConfigSO IKeyValuePair<CubeType, CubeConfigSO>.value => so;
    }
    
    [Serializable]
    public struct CubePreset : IKeyValuePair<CubePresetType, CubePresetSO>
    {
        public CubePresetType presetType;
        public CubePresetSO so;
        
        CubePresetType IKeyValuePair<CubePresetType, CubePresetSO>.key => presetType;
        CubePresetSO IKeyValuePair<CubePresetType, CubePresetSO>.value => so;
    }

    public class SOManager : Singleton<SOManager>
    {
        [Header("每个Cell对应的高度分布配置")]
        public List<HeightDistribution> heightDistributions = new List<HeightDistribution>();

        [Header("每个岛屿类型对应的配置")]
        public List<IslandConfig> islandConfigs = new List<IslandConfig>();

        [Header("每个方块类型对应的配置")]
        public List<CubeConfig> cubeConfigs = new List<CubeConfig>();
        
        [Header("Preset配置")]
        public List<CubePreset> cubePresets = new List<CubePreset>();

        public Dictionary<CellType, HeightDistributionConfigSO> heightDistributionDict =
            new Dictionary<CellType, HeightDistributionConfigSO>();
        
        public Dictionary<IslandType, IslandConfigSO> islandConfigDict =
            new Dictionary<IslandType, IslandConfigSO>();
        
        public Dictionary<CubeType, CubeConfigSO> cubeConfigDict =
            new Dictionary<CubeType, CubeConfigSO>();
        
        public Dictionary<CubePresetType, CubePresetSO> cubePresetDict =
            new Dictionary<CubePresetType, CubePresetSO>();

        protected override void Awake()
        {
            base.Awake();

            RegisterDictionary(heightDistributions, heightDistributionDict, "HeightDistributionConfigSO");
            RegisterDictionary(islandConfigs, islandConfigDict, "IslandConfigSO");
            RegisterDictionary(cubeConfigs, cubeConfigDict, "CubeConfigSO");
            RegisterDictionary(cubePresets, cubePresetDict, "CubePresetSO");
        }

        private void RegisterDictionary<T, TKey, TValue>(List<T> list, Dictionary<TKey, TValue> dictionary, string typeName)
            where T : IKeyValuePair<TKey, TValue>
        {
            foreach (var item in list)
            {
                if (!dictionary.ContainsKey(item.key))
                {
                    dictionary.Add(item.key, item.value);
                }
                else
                {
                    Debug.LogError($"{typeName} for {item.key} already exists!");
                }
            }
        }
    }
}