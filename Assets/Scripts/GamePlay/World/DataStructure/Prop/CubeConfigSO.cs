using UnityEngine;

namespace AILand.GamePlay.World
{
    
    [CreateAssetMenu(fileName ="PropConfig_" ,menuName ="创建Prop配置",order = 0)]
    public class PropConfigSO : ScriptableObject
    {
        public PropType propType;
        
        public GameObject propPrefab;
    }
}