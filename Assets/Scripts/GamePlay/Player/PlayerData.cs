using System.Collections.Generic;
using AILand.GamePlay.InventorySystem;
using AILand.GamePlay.World;
using AILand.GamePlay.World.Prop;
using UnityEngine;

namespace GamePlay.Player
{
    public class PlayerData
    {
        // 玩家收集的元素能量
        private ElementalEnergy m_elementalEnergy;
        
        // 玩家持有的道具
        private InventoryData m_inventoryData;
        private int m_inventoryCounter = 0;

        public PlayerData()
        {
            m_elementalEnergy = new ElementalEnergy();
            m_inventoryData = new InventoryData();
        }

        public int AddInventory()
        {
            m_inventoryData.allInventorys[m_inventoryCounter] = new List<ItemData>(30);
            for (int i = 0; i < 30; i++)
            {
                m_inventoryData.allInventorys[m_inventoryCounter].Add(new ItemData());
            }
            return m_inventoryCounter++;
        }

        public bool AddItem(int id, int count, int inventoryID = 0)
        {
            if(id == 0 || count <= 0)
            {
                Debug.LogWarning("Invalid item ID or count.");
                return false;
            }
            
            if (!m_inventoryData.allInventorys.ContainsKey(inventoryID))
            {
                m_inventoryData.allInventorys[inventoryID] = new List<ItemData>();
                Debug.LogWarning($"Inventory ID {inventoryID} does not exist, creating a new inventory.");
            }

            var itemList = m_inventoryData.allInventorys[inventoryID];

            // 先尝试找到相同ID的物品进行合并
            int existingIndex = itemList.FindIndex(i => i.itemID == id && i.itemID != 0);
            if (existingIndex >= 0)
            {
                var existingItem = itemList[existingIndex];
                existingItem.itemCount += count;
                itemList[existingIndex] = existingItem;
                Debug.Log($"Item {id} added successfully in {inventoryID}.");
                return true;
            }

            // 找到第一个空位（itemID为0）
            int emptyIndex = itemList.FindIndex(i => i.itemID == 0);
            if (emptyIndex >= 0)
            {
                var emptySlot = itemList[emptyIndex];
                emptySlot.itemID = id;
                emptySlot.itemCount = count;
                itemList[emptyIndex] = emptySlot;
                Debug.Log($"Item {id} added successfully in {inventoryID}.");
                return true;
            }

            // 背包已满
            Debug.Log($"Inventory {inventoryID} is full, cannot add item {id}.");
            return false;
        }
        
        public List<ItemData> GetItemsInInventory(int inventoryID = 0)
        {
            if (m_inventoryData.allInventorys.ContainsKey(inventoryID))
            {
                return m_inventoryData.allInventorys[inventoryID];
            }
            else
            {
                Debug.LogWarning($"Inventory ID {inventoryID} does not exist.");
                return new List<ItemData>();
            }
        }

        public void SwitchItemInInventory(int inventoryId, int from, int to)
        {
            // 检查背包ID是否存在
            if (!m_inventoryData.allInventorys.ContainsKey(inventoryId))
            {
                Debug.LogWarning($"Inventory ID {inventoryId} does not exist.");
                return;
            }
            
            var dataList = m_inventoryData.allInventorys[inventoryId];
            if (dataList == null || from < 0 || to < 0 || from >= dataList.Count || to >= dataList.Count)
            {
                Debug.LogWarning($"Invalid inventory ID {inventoryId} or index out of range.");
                return;
            }
            (dataList[from], dataList[to]) = (dataList[to], dataList[from]);
        }
    }
}