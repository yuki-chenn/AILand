using System;
using System.Collections.Generic;
using System.SOManager;
using UnityEngine;

namespace AILand.GamePlay.World
{
    public class PCGIslandInfo
    {
        // 运行时生成的
        public ShapeConfig shapeConfig;
        
        // 主要是地形的方块分布
        public IslandConfigSO islandConfigSO;


        public List<CubeType> GetCellCubesType(int x, int z)
        {
            if(islandConfigSO == null)
            {
                Debug.LogError("IslandConfigSO is not set.");
                return null;
            }
            
            CellType cellType = islandConfigSO.cellTypes[x, z];
            
            // 根据cellType获取对应的HeightDistributionConfigSO
            var hdConfig = SOManager.Instance.heightDistributionDict[cellType];
            if (hdConfig == null)
            {
                Debug.LogError($"HeightDistributionConfigSO not found for CellType: {cellType}");
                return null;
            }
            
            // 获取高度
            int height = shapeConfig.GetHeight(x, z);

            return hdConfig.GetCellCubesType(height);
        }
    }
}