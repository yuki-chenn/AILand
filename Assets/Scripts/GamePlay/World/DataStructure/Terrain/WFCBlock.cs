using AILand.GamePlay.World;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;
using AILand.Utils;

public class WFCBlock
{
    private WFCConfigSO m_config;

    private Dictionary<int, List<IslandType>> m_probableType;
    private Dictionary<int, IslandType> m_generatedTypes;
    private Dictionary<IslandType, AdjacencyRule> m_adjacencyRuleDic;

    private readonly List<IslandType> allType = Enum.GetValues(typeof(IslandType)).Cast<IslandType>().ToList();

    public WFCBlock(WFCConfigSO wfcConfig, int seed = -1)
    {
        m_config = wfcConfig;
        m_probableType = new Dictionary<int, List<IslandType>>();
        m_generatedTypes = new Dictionary<int, IslandType>();

        InitAdjacencyRules();
    }

    private void InitAdjacencyRules()
    {
        m_adjacencyRuleDic = new Dictionary<IslandType, AdjacencyRule>();
        foreach (var rule in m_config.adjacencyRule)
        {
            if(m_adjacencyRuleDic.ContainsKey(rule.type))
            {
                Debug.LogWarning($"Duplicate adjacency rule for type {rule.type}. Overwriting existing rule.");
            }
            m_adjacencyRuleDic[rule.type] = rule;
        }
    }

    public IslandType Generate(int blockId,int range)
    {
        if (!m_generatedTypes.ContainsKey(blockId))
        {
            // 波函数坍缩
            
            // 确定坍缩范围
            List<int> collapsIds = new List<int>();

            var blockIndex = Util.GetBlockIndexByID(blockId);
            for (int dx = -range; dx <= range; dx++)
            {
                for (int dz = -range; dz <= range; dz++)
                {
                    int id = Util.GetBlockID(blockIndex + new Vector2Int(dx, dz));
                    if(!m_generatedTypes.ContainsKey(id)) collapsIds.Add(id);
                }
            }

            // 初始化可能性
            foreach (var id in collapsIds)
            {
                if (!m_probableType.ContainsKey(id))
                {
                    // 初始化所有类型
                    if(id == Constants.FirstBlockID)
                    {
                        // 如果id包含了玩家的初始岛屿id，则默认只有Custom类型
                        m_probableType[id] = new List<IslandType> { IslandType.Plain };
                    }
                    else
                    {
                        m_probableType[id] = new List<IslandType>(allType);
                        m_probableType[id].Remove(IslandType.None);
                    }
                }
            }

            // 循环坍缩，直到全部位置都坍缩为单一类型
            while(collapsIds.Count > 0)
            {
                // 从可能性列表中选择熵最小的位置
                var coId = FindMinEntropyPosition(collapsIds);
                if(m_generatedTypes.ContainsKey(coId) && m_generatedTypes[coId] != IslandType.None)
                {
                    Debug.LogError($"Block {coId} has already been collapsed to a specific type.");
                }
                if (coId == -1)
                {
                    Debug.LogError("No valid position found for collapse.");
                    break;
                }
                // 坍缩该位置
                CollapsePosition(coId);
                // 从待坍缩列表中移除
                collapsIds.Remove(coId);
                // 传播约束到相邻位置
                PropagateConstraints(coId, collapsIds);
            }

        }


        return m_generatedTypes[blockId];
    }

    /// <summary>
    /// 找到熵最小的位置
    /// </summary>
    private int FindMinEntropyPosition(in List<int> blockIds)
    {
        int minEntropy = int.MaxValue;

        foreach (var id in blockIds)
        {
            if(m_generatedTypes.ContainsKey(id) && m_generatedTypes[id] != IslandType.None)
            {
                // 已经坍缩为单一类型，跳过
                continue;
            }

            int entropy = m_probableType[id].Count;
            minEntropy = Math.Min(minEntropy, entropy);
        }

        if (minEntropy == int.MaxValue) return -1;

        var candidates = new List<int>();
        foreach (var id in blockIds)
        {
            if(m_probableType[id].Count == minEntropy) candidates.Add(id);
        }

        // 随机选择一个候选位置
        return Util.GetRandomElement(candidates);
    }

    /// <summary>
    /// 坍缩指定位置
    /// </summary>
    private void CollapsePosition(int blockId)
    {
        var possibleTypes = m_probableType[blockId];

        if (possibleTypes.Count == 0)
        {
            Debug.LogError($"No possible types for position {blockId}");
            return;
        }

        // 随机选择一个类型
        var type = Util.GetRandomElement(possibleTypes);

        // 设置为单一类型
        m_probableType[blockId] = new List<IslandType> { type };
        m_generatedTypes[blockId] = type;
        Debug.Log($"Block {blockId} collapsed to type {type}");
    }

    /// <summary>
    /// 传播约束到相邻位置
    /// </summary>
    private void PropagateConstraints(int curBlockId,in List<int> allBlockId)
    {
        var neighbors = GetNeighborsInList(curBlockId, allBlockId);
        foreach (int neighborId in neighbors) UpdateNeighborProbableType(neighborId,curBlockId);
    }

    /// <summary>
    /// 获取指定位置的所有邻居
    /// </summary>
    private List<int> GetNeighborsInList(int blockId, in List<int> allBlockId)
    {
        var neighbors = new List<int>();
        var index = Util.GetBlockIndexByID(blockId);

        foreach (var id in allBlockId)
        {
            var neighborIndex = Util.GetBlockIndexByID(id);
            if (Mathf.Abs(index.x - neighborIndex.x) <= 1 && Mathf.Abs(index.y - neighborIndex.y) <= 1 && id != blockId)
            {
                neighbors.Add(id);
            }
        }

        return neighbors;
    }

    /// <summary>
    /// 获取周围8个邻居的blockId列表
    /// </summary>
    /// <param name="blockId"></param>
    /// <returns></returns>
    private List<int> GetNeighbors(int blockId)
    {
        var neighbors = new List<int>();
        var index = Util.GetBlockIndexByID(blockId);

        for(int dx = -1; dx <= 1; dx++)
        {
            for (int dz = -1; dz <= 1; dz++)
            {
                if (dx == 0 && dz == 0) continue; // 跳过自身
                int neighborId = Util.GetBlockID(index + new Vector2Int(dx, dz));
                neighbors.Add(neighborId);
            }
        }

        return neighbors;
    }

    /// <summary>
    /// 根据disseminateBlockId更新邻居updateBlockId的probableType
    /// </summary>
    /// <param name="updateBlockId">需要更新的blockId</param>
    /// <param name="disseminateBlockId">坍缩的blockId</param>
    private void UpdateNeighborProbableType(int updateBlockId,int disseminateBlockId)
    {
        if(!m_generatedTypes.ContainsKey(disseminateBlockId))
        {
            Debug.LogWarning($"UpdateNeighborProbableType : Disseminate block {disseminateBlockId} has not been generated yet.");
            return;
        }

        if (m_generatedTypes.ContainsKey(updateBlockId) && m_generatedTypes[updateBlockId] != IslandType.None)
        {
            Debug.LogWarning($"UpdateNeighborProbableType : Update block {updateBlockId} has already been generated with type {m_generatedTypes[updateBlockId]}.");
            return;
        }

        IslandType disseminateType = m_generatedTypes[disseminateBlockId];
        // 1. 8 邻接关系
        var forbid8 = m_adjacencyRuleDic[disseminateType].forbid8;
        foreach(var type in forbid8) m_probableType[updateBlockId].Remove(type);
        // 2. 4 邻接关系
        if (Is4Adjacent(updateBlockId, disseminateBlockId))
        {
            var forbid4 = m_adjacencyRuleDic[disseminateType].forbid4;
            foreach (var type in forbid4) m_probableType[updateBlockId].Remove(type);
        }
        // 3. leastWaterTypeCount
        var neighbors = GetNeighbors(updateBlockId);
        foreach (var neighborId in neighbors)
        {
            if (m_generatedTypes.ContainsKey(neighborId) && m_generatedTypes[neighborId] != IslandType.None)
            {
                var type = m_generatedTypes[neighborId];
                var least = m_adjacencyRuleDic[type].leastWaterTypeCount;
                int notWaterCount = CountNotWaterTypeInNeighbors(neighborId);
                if(8 - notWaterCount <= least)
                {
                    // 可能是water的数量已经小于等于least，water
                    m_probableType[updateBlockId] = new List<IslandType> { IslandType.Water };
                    break;
                }
            }
        }
    }

    /// <summary>
    /// 判断两个位置之间是否是4邻接
    /// </summary>
    private bool Is4Adjacent(int id1, int id2)
    {
        var index1 = Util.GetBlockIndexByID(id1);
        var index2 = Util.GetBlockIndexByID(id2);

        int dx = Math.Abs(index1.x - index2.x);
        int dz = Math.Abs(index1.y - index2.y);

        return dx + dz == 1;
    }

    private int CountNotWaterTypeInNeighbors(int blockId)
    {
        int count = 0;
        var neighbors = GetNeighbors(blockId);
        foreach (var neighborId in neighbors)
        {
            if (m_generatedTypes.ContainsKey(neighborId) && m_generatedTypes[neighborId] != IslandType.Water)
            {
                count++;
            }
        }
        return count;
    }
}
