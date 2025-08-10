using System;
using System.Collections.Generic;
using UnityEngine;

namespace AILand.GamePlay.World
{

    [Serializable]
    public struct StoredElementEnergy
    {
        public EnergyType energyType;
        public int count;

        public StoredElementEnergy(EnergyType type, int count)
        {
            this.energyType = type;
            this.count = count;
        }
    }

    
    [CreateAssetMenu(fileName ="CubeConfig_" ,menuName ="创建方块配置",order = 0)]
    public class CubeConfigSO : ScriptableObject
    {
        public CubeType cubeType;
        
        public GameObject cubePrefab;
        
        public bool canDestroy = true;

        public List<StoredElementEnergy> elementEnergy;

        public bool canPlaceCubeOn = true;
        
        public bool hasCollision = true;

        public Color TopColor;

        public Color SideColor;
    }
}