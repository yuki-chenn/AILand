using UnityEngine;

namespace AILand.GamePlay.World
{
    [CreateAssetMenu(fileName ="IslandConfig_" ,menuName ="创建岛屿配置",order =0)]
    public class IslandConfigSO : ScriptableObject
    {
        public CellType[,] cellTypes = new CellType[200,200]; 
        
    }
}