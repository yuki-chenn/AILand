using System.Collections.Generic;

namespace AILand.GamePlay.InventorySystem
{
    public struct ItemData
    {
        public int itemID;
        public int itemCount;
    }
    
    public class InventoryData
    {
        public Dictionary<int, List<ItemData>> allInventorys = new Dictionary<int, List<ItemData>>();
    }
}