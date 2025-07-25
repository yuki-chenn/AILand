using System;
using System.Collections.Generic;
using UnityEngine;

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
        
        public ShapeConfig shapeConfig;
        
        public ScriptableObject IslandConfigSO;
        
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