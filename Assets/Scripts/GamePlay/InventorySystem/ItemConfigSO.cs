using AILand.GamePlay.InventorySystem;
using UnityEngine;

namespace AILand.GamePlay.World
{
    
    [CreateAssetMenu(fileName ="ItemConfig_" ,menuName ="创建item配置",order =0)]
    public class ItemConfigSO : ScriptableObject
    {
       public int itemID;
       public string itemName;
       public ItemEnum itemEnum;
       public Sprite itemIcon;
    }
}