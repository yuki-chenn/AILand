using System;
using System.Collections.Generic;

namespace AILand.GamePlay.World
{
    
    /// <summary>
    /// 小于等于lowerBound的高度对应的Cube类型
    /// </summary>
    public struct CubeDistribution
    {
        public CubeType cubeType;
        public int lowerBound; // 小于等于该值的高度
    }
    
    public class PCGIslandInfo
    {
        // 高度阈值
        public float threshold;
        
        // 地形图
        public float[,] heightMap;
        
        // 生成时最大高度
        public int generateMaxHeight;
        
        // 将heightMap转换为高度的函数
        public Func<float,int> heightMapFunc;
        
        // Cube分布
        public List<CubeDistribution> cubeDistributions;

        private CubeType[] m_cubesType;
        public CubeType[] CubesType
        {
            get
            {
                if (m_cubesType == null) InitCubesType();
                return m_cubesType;
            }
        }
        
        
        
        public PCGIslandInfo(float threshold, float[,] heightMap, int generateMaxHeight, Func<float,int> heightMapFunc)
        {
            this.threshold = threshold;
            this.heightMap = heightMap;
            this.generateMaxHeight = generateMaxHeight;
            this.heightMapFunc = heightMapFunc;
        }

        private void InitCubesType()
        {
            if(cubeDistributions == null || cubeDistributions.Count == 0)
            {
                throw new Exception("Cube distributions are not initialized.");
            }
            
            cubeDistributions.Sort((a, b) => a.lowerBound.CompareTo(b.lowerBound));
            
            m_cubesType = new CubeType[generateMaxHeight];
            int currentIndex = 0;
            for (int y = 0; y < generateMaxHeight; y++)
            {
                while(currentIndex < cubeDistributions.Count && y > cubeDistributions[currentIndex].lowerBound)
                {
                    currentIndex++;
                }
                
                if (currentIndex < cubeDistributions.Count)
                {
                    m_cubesType[y] = cubeDistributions[currentIndex].cubeType;
                }
                else
                {
                    m_cubesType[y] = CubeType.None;
                }
            }
        }


    }
}