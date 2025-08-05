using UnityEngine;
using System.Collections.Generic;
using AILand.GamePlay;
using AILand.System.EventSystem;
using EventType = AILand.System.EventSystem.EventType;
using NotImplementedException = System.NotImplementedException;

namespace AILand.UI
{
    public class InventoryUI : MonoBehaviour
    {
        [Header("背包设置")]
        public GameObject itemSlotPrefab;
        public int slotCount = 30;

        private List<ItemSlotUI> itemSlots = new List<ItemSlotUI>();

        private void Awake()
        {
            InitializeSlots();
        }

        private void InitializeSlots()
        {
            // 如果已经有槽位，先获取它们
            for (int i = 0; i < transform.childCount; i++)
            {
                var slot = transform.GetChild(i).GetComponent<ItemSlotUI>();
                if (slot != null)
                {
                    itemSlots.Add(slot);
                }
            }

            // 如果槽位不够，创建新的
            while (itemSlots.Count < slotCount)
            {
                var newSlot = Instantiate(itemSlotPrefab, transform);
                var slotComponent = newSlot.GetComponent<ItemSlotUI>();
                if (slotComponent == null)
                {
                    slotComponent = newSlot.AddComponent<ItemSlotUI>();
                }
                itemSlots.Add(slotComponent);
            }
        }

        public bool AddItem(GameObject itemPrefab)
        {
            // 找到第一个空槽位
            foreach (var slot in itemSlots)
            {
                if (slot.IsEmpty())
                {
                    var newItem = Instantiate(itemPrefab, slot.transform);
                    var itemUI = newItem.GetComponent<ItemUI>();
                    if (itemUI == null)
                    {
                        itemUI = newItem.AddComponent<ItemUI>();
                    }
                    
                    slot.SetItem(itemUI);
                    return true;
                }
            }
        
            return false; // 背包已满
        }
        
        public void RemoveItem(ItemUI item)
        {
            foreach (var slot in itemSlots)
            {
                if (slot.GetCurrentItem() == item)
                {
                    slot.SetItem(null);
                    Destroy(item.gameObject);
                    break;
                }
            }
        }
        
        public int GetEmptySlotCount()
        {
            int count = 0;
            foreach (var slot in itemSlots)
            {
                if (slot.IsEmpty())
                {
                    count++;
                }
            }
            return count;
        }

        public void UpdateInventory(int inventoryId)
        {
            if(itemSlots.Count == 0)
            {
                InitializeSlots();
            }
            
            var inventoryData = DataManager.Instance.PlayerData.GetItemsInInventory(inventoryId);
            if (inventoryData == null || inventoryData.Count != slotCount)
            {
                Debug.LogError($"Inventory ID {inventoryId} does not exist or data error.");
                return;
            }

            for (int i = 0; i < inventoryData.Count; i++)
            {
                var itemData = inventoryData[i];
                itemSlots[i].UpdateItem(itemData);
            }
            
            
            
        }
    }
}