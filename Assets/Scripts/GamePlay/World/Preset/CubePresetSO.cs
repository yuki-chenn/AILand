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
    }
    
    [CreateAssetMenu(fileName = "CubePreset_", menuName = "创建方块组合预设")]
    public class CubePresetSO : ScriptableObject
    {
        // 所占空间大小，主要是用来判断能否放置
        public Vector3Int size;

        // 是否可旋转
        public bool canRotate = false;
        
        public List<PresetCubeData> cubes = new List<PresetCubeData>();
        
        // 获取指定位置的方块数据
        public PresetCubeData? GetCubeAt(Vector3Int position)
        {
            foreach (var cube in cubes)
            {
                if (cube.position == position)
                    return cube;
            }
            return null;
        }

        // 添加方块到预设
        public void AddCube(Vector3Int position, CubeType type)
        {
            var cubeData = new PresetCubeData
            {
                position = position,
                cubeType = type,
            };
            cubes.Add(cubeData);
        }
        
    }
}