using System;
using System.Collections.Generic;
using AILand.GamePlay.World;
using UnityEditor;
using UnityEngine;

namespace AILand.GamePlay.World
{
    
    [Serializable]
    public struct PresetCubeData
    {
        public Vector3Int position;  // 相对于预设中心的位置
        public CubeType cubeType;    // 方块类型
        public int rotation; // 旋转角度，0, 1, 2, 3
    }
    
    [CreateAssetMenu(fileName = "CubePreset_", menuName = "创建方块组合预设")]
    public class CubePresetSO : ScriptableObject
    {
        // 所占空间，主要是用来判断能否放置
        public Vector3Int minPoint = Vector3Int.zero;  // 左下角点
        public Vector3Int maxPoint = Vector3Int.zero;  // 右上角点
        
        
        public bool canRotate = false;          // 是否可以旋转
        public int fixedHeight = -1;            // 固定高度，-1表示不固定
        public bool connectToIsland = false;    // root是否连接到地面
        public CellWater cellWater ;            // root所在的点的水域类型
        public bool canReplace = false;         // 是否可以替换已有方块
        public List<CubeType> connectedCubeTypes; // 连接的Cell类型，主要是用来判断能否放置
        
        
        public List<PresetCubeData> cubes = new List<PresetCubeData>();
        
        
        public PresetCubeData? GetCubeAt(Vector3Int position)
        {
            foreach (var cube in cubes)
            {
                if (cube.position == position)
                    return cube;
            }
            return null;
        }

        
        public void AddCube(Vector3Int position, CubeType type, int rotation)
        {
            // 检查位置是否已经存在方块
            if (GetCubeAt(position) != null)
            {
                Debug.LogWarning($"位置 {position} 已经存在方块，无法添加");
                return;
            }
            
            var cubeData = new PresetCubeData
            {
                position = position,
                cubeType = type,
                rotation = rotation
            };
            cubes.Add(cubeData);
        }
        
    }
}