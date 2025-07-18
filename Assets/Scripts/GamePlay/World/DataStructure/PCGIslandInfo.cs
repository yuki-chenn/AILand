using System;

namespace AILand.GamePlay.World
{
    public class PCGIslandInfo
    {
        // 高度阈值
        public float threshold;
        
        // 地形图
        public float[,] heightMap;
        
        // 生成时最大高度
        public float generateMaxHeight;
        
        // 将heightMap转换为高度的函数
        public Func<float,int> heightMapFunc;
        
        // 方块类型
        public CubeType[] CubeTypes;
        
        // 方块分布 [a,b,c,d] 表示 0-a : 方块类型0, a-b : 方块类型1, b-c : 方块类型2, c-d : 方块类型3, d-1 : 方块类型4 ，以此类推
        public int[] distribution;
        
        
        
        public PCGIslandInfo(float threshold, float[,] heightMap, float generateMaxHeight, Func<float,int> heightMapFunc)
        {
            this.threshold = threshold;
            this.heightMap = heightMap;
            this.generateMaxHeight = generateMaxHeight;
            this.heightMapFunc = heightMapFunc;
        }
        
    }
}