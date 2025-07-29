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
        // 所占空间大小，主要是用来判断能否放置
        public Vector3Int size;
        
        public bool canRotate = false;          // 是否可以旋转
        public int fixedHeight = -1;            // 固定高度，-1表示不固定
        public bool outsideIsland = true;       // 是否在岛屿外部，即root必须在外边界上或者在外边界以外
        public bool connectToIsland = false;    // 是否必须连接到岛屿
        public bool canReplace = false;         // 是否可以替换已有方块
        
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