using System;
using System.Collections.Generic;
using UnityEngine;

namespace AILand.GamePlay.World
{
    [Serializable]
    public struct Distribution
    {
        public CubeType cubeType;   // 方块类型
        public int count;           // 方块数量，如果方块数量 > 0，则表示一定会有该数量的方块。此时不考虑权重值，也不算做权重。
        public float weight;        // 权重值
    }
    
    
    [CreateAssetMenu(fileName ="HeightDistributionConfig_" ,menuName ="创建高度方块分布配置",order = 1)]
    public class HeightDistributionConfigSO : ScriptableObject
    {
        public CellType cellType;
        
        public List<Distribution> distributions;
        
        [Header("预计算的数据（不要更改）")]
        [SerializeField] private float[] cumulativeWeights;
        [SerializeField] private int fixedCount;
        [SerializeField] private Dictionary<int, List<CubeType>> heightCache = new Dictionary<int, List<CubeType>>();

        // 参数更改时自动更新
        private void OnValidate()
        {
#if UNITY_EDITOR
            // 延迟调用，避免在序列化过程中修改数据
            UnityEditor.EditorApplication.delayCall += () => {
                if (this != null) // 确保对象未被销毁
                {
                    PreCalculateWeights();
                }
            };
#endif
        }
        
        // 在编辑器中调用，预计算权重
        [ContextMenu("预计算权重表")]
        private void PreCalculateWeights()
        {
            if (distributions == null || distributions.Count == 0) return;
            
            cumulativeWeights = new float[distributions.Count];
            float totalWeight = 0f;
            fixedCount = 0;
            
            for(int i=0;i<distributions.Count;i++)
            {
                var dist = distributions[i];
                if (dist.count > 0)
                {
                    fixedCount += dist.count; // 累加固定数量
                    cumulativeWeights[i] = -1f; // 固定数量的分布不参与权重计算
                }
                else
                {
                    totalWeight += dist.weight;
                    cumulativeWeights[i] = totalWeight;
                }
            }
            
            // 归一化
            for(int i=0;i<cumulativeWeights.Length;i++)
            {
                if (cumulativeWeights[i] >= 0f)
                {
                    cumulativeWeights[i] /= totalWeight; // 归一化
                }
            }

            // 标记为已修改，保存到资源文件
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(this);
#endif
        }

        public List<CubeType> GetCellCubesType(int height)
        {
            if(heightCache.ContainsKey(height)) return heightCache[height];
            
            List<CubeType> cellCubes = new List<CubeType>();
            
            if(height <= fixedCount)
            {
                var count = 0;
                // 如果高度小于固定数量，则直接返回固定数量的方块
                foreach(var dis in distributions)
                {
                    if(count >= height) break;
                    if (dis.count > 0)
                    {
                        for (int j = 0; j < dis.count; j++)
                        {
                            if(count >= height) break;
                            cellCubes.Add(dis.cubeType);
                            count++;
                        }
                    }
                }
            }
            else
            {
                int remainingHeight = height - fixedCount;
                int hi = 0;
                for (int i = 0; i < distributions.Count; i++)
                {
                    var rate = 1.0f * hi / remainingHeight;
                    while (rate < cumulativeWeights[i] + Mathf.Epsilon && cumulativeWeights[i] > 0 && hi < remainingHeight)
                    {
                        cellCubes.Add(distributions[i].cubeType);
                        hi++;
                        rate = 1.0f * hi / remainingHeight;
                    }
                
                    if (cumulativeWeights[i] < Mathf.Epsilon)
                    {
                        // 说明这部分是固定权重
                        for (int j = 0; j < distributions[i].count; j++)
                        {
                            cellCubes.Add(distributions[i].cubeType);
                        }
                    }
                }
            }
            
            heightCache[height] = cellCubes;
            
            return cellCubes;
        }
    }
}