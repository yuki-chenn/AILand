using AILand.GamePlay.InventorySystem;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace AILand.UI
{
    public class ItemSlotUI : MonoBehaviour
    {
        [Header("槽位设置")]
        public bool canAcceptItems = true;

        private ItemUI currentItem;

        private void Awake()
        {
            currentItem = GetComponentInChildren<ItemUI>();
            if (currentItem == null)
            {
                Debug.LogError($"Current slot {name} does not have an ItemUI component.");
            }
        }
        
        public ItemUI GetCurrentItem()
        {
            return currentItem;
        }

        public void SetItem(ItemUI item)
        {
            currentItem = item;
        }

        public bool IsEmpty()
        {
            return currentItem == null;
        }

        public bool CanAcceptItem(ItemUI item)
        {
            return canAcceptItems;
        }

        public void UpdateItem(ItemData itemData)
        {
            var itemId = itemData.itemID;
            var itemIcon = ItemFactory.GetItemByID(itemId).config.itemIcon;
            currentItem.UpdateItemInfo(itemIcon, itemData.itemCount);
        }
    }
}