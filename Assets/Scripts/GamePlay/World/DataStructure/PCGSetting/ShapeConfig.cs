using System;

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
        public Func<float,int> heightMapFunc;
        
        public ShapeConfig(float threshold, float[,] heightMap, int generateMaxHeight, Func<float, int> heightMapFunc)
        {
            this.threshold = threshold;
            this.heightMap = heightMap ?? throw new ArgumentNullException(nameof(heightMap), "HeightMap cannot be null.");
            this.generateMaxHeight = generateMaxHeight;
            this.heightMapFunc = heightMapFunc ?? throw new ArgumentNullException(nameof(heightMapFunc), "HeightMap function cannot be null.");
        }

        public int GetHeight(int x, int z)
        {
            if (heightMap == null)
            {
                throw new NullReferenceException("HeightMap is not initialized.");
            }
            
            var heightValue = heightMap[x, z];
            return heightValue < threshold ? 0 : heightMapFunc(heightValue);
        }
    }
}