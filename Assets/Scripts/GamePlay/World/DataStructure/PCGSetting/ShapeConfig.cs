using System;
using UnityEngine;

namespace AILand.GamePlay.World
{
    public class ShapeConfig
    {
        // 高度阈值
        public float threshold;
        
        // 地形图 0-1
        public float[,] heightMap;
        
        // 生成时最大高度
        public int generateMaxHeight;
        
        // 将heightMap转换为高度的函数
        public AnimationCurve noiseToHeightCurve;
        
        public ShapeConfig(float threshold, float[,] heightMap, int generateMaxHeight, AnimationCurve noiseToHeightCurve)
        {
            this.threshold = threshold;
            this.heightMap = heightMap ?? throw new ArgumentNullException(nameof(heightMap), "HeightMap cannot be null.");
            this.generateMaxHeight = generateMaxHeight;
            this.noiseToHeightCurve = noiseToHeightCurve ?? AnimationCurve.Linear(0f, 0f, 1f, generateMaxHeight);
        }

        public int GetHeight(int x, int z)
        {
            if (heightMap == null)
            {
                throw new NullReferenceException("HeightMap is not initialized.");
            }
            
            var heightValue = heightMap[x, z];
            return heightValue < threshold ? 0 : MapNoiseToHeight(heightValue);
        }
        
        /// <summary>
        /// 将噪声值映射为高度
        /// </summary>
        /// <param name="noiseValue">噪声值 (通常在0-1之间)</param>
        /// <returns>映射后的高度值</returns>
        public int MapNoiseToHeight(float noiseValue)
        {
            noiseValue = Mathf.Clamp01(noiseValue);
    
            // 使用贝塞尔曲线计算高度值
            float heightFloat = noiseToHeightCurve.Evaluate(noiseValue);
            heightFloat = Mathf.Clamp(heightFloat, 0, generateMaxHeight );
                
            return Mathf.RoundToInt(heightFloat);
        }

        /// <summary>
        /// 批量映射噪声值数组到高度数组
        /// </summary>
        /// <param name="noiseValues">噪声值数组</param>
        /// <returns>高度值数组</returns>
        public int[,] MapNoiseArrayToHeight(float[,] noiseValues)
        {
            int width = noiseValues.GetLength(0);
            int height = noiseValues.GetLength(1);
            int[,] heightMap = new int[width, height];
    
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    heightMap[x, y] = MapNoiseToHeight(noiseValues[x, y]);
                }
            }
    
            return heightMap;
        }
    }
}