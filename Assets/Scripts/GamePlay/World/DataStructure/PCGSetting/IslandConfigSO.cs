using System;
using System.Collections.Generic;
using UnityEngine;

namespace AILand.GamePlay.World
{
    
    /// <summary>
    /// 表示多少高度以下强制设置为某个CellType
    /// </summary>
    [Serializable]
    public struct ExtraLimit
    {
        public CellType cellType;
        public int heightLimit;
    }
    
    [Serializable]
    public struct PresetSetting
    {
        public CubePresetType presetType;
        public int fixedCount;
        public Vector2Int rangeCount;
    }
    
    
    [CreateAssetMenu(fileName ="IslandConfig_" ,menuName ="创建岛屿配置",order =0)]
    public class IslandConfigSO : ScriptableObject
    {
        [SerializeField] private int width = 200;
        [SerializeField] private int height = 200;
        [SerializeField][HideInInspector] private CellType[] cellTypesArray;
        
        // preset的设置
        public List<PresetSetting> presetSettings = new List<PresetSetting>();
        
        
        
        // 动态属性 和 高度有关的
        [Header("额外的高度配置")] [Tooltip("当高度小于此值时，强制设置为对应的CellType")]
        public ExtraLimit extraLimitBelow;
        

        public int Width => width;
        public int Height => height;

        // 二维数组属性访问器
        public CellType GetCellType(int x, int y, int _height = -1)
        {
            if(_height != -1 && _height <= extraLimitBelow.heightLimit)
            {
                return extraLimitBelow.cellType;
            }
            
            if (x < 0 || x >= width || y < 0 || y >= height) return CellType.None;
            if (cellTypesArray == null) InitializeArray();
            return cellTypesArray[y * width + x];
        }

        public void SetCellType(int x, int y, CellType cellType)
        {
            if (x < 0 || x >= width || y < 0 || y >= height) return;
            if (cellTypesArray == null) InitializeArray();
            cellTypesArray[y * width + x] = cellType;
        }

        public void ResizeArray(int newWidth, int newHeight)
        {
            var newArray = new CellType[newWidth * newHeight];
            
            if (cellTypesArray != null)
            {
                // 复制旧数据
                for (int y = 0; y < Mathf.Min(height, newHeight); y++)
                {
                    for (int x = 0; x < Mathf.Min(width, newWidth); x++)
                    {
                        newArray[y * newWidth + x] = cellTypesArray[y * width + x];
                    }
                }
            }

            width = newWidth;
            height = newHeight;
            cellTypesArray = newArray;
        }

        private void InitializeArray()
        {
            cellTypesArray = new CellType[width * height];
        }

        private void OnValidate()
        {
            if (cellTypesArray == null || cellTypesArray.Length != width * height)
            {
                InitializeArray();
            }
        }
    }
}