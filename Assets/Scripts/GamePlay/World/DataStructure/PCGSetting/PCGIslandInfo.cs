using System;
using System.Collections.Generic;
using AILand.System.SOManager;
using UnityEngine;

namespace AILand.GamePlay.World
{
    public class PCGIslandInfo
    {
        // 运行时生成的
        public ShapeConfig shapeConfig;
        
        // 主要是地形的方块分布
        public IslandConfigSO islandConfig;


        public List<CubeType> GetCellCubesType(int x, int z)
        {
            if(islandConfig == null)
            {
                Debug.LogError("IslandConfigSO is not set.");
                return null;
            }
            // 获取高度
            int height = shapeConfig.GetHeight(x, z);
            
            CellType cellType = islandConfig.GetCellType(x, z, height);
            
            // 根据cellType获取对应的HeightDistributionConfigSO
            var hdConfig = SOManager.Instance.heightDistributionDict[cellType];
            if (hdConfig == null)
            {
                Debug.LogError($"HeightDistributionConfigSO not found for CellType: {cellType}");
                return null;
            }

            return hdConfig.GetCellCubesType(height);
        }
    }
}