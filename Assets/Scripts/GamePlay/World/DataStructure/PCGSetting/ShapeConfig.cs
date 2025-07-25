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
    }
}