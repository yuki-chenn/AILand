using UnityEngine;

namespace AILand.GamePlay.World
{
    
    [CreateAssetMenu(fileName ="CubeConfig_" ,menuName ="创建方块配置",order = 0)]
    public class CubeConfigSO : ScriptableObject
    {
        public CubeType cubeType;
        
        public GameObject cubePrefab;
        
        public bool canDestroy = true;

        public bool canPlaceCubeOn = true;

    }
}