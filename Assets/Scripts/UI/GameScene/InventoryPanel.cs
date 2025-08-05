using AILand.GamePlay;
using AILand.System.EventSystem;
using Unity.VisualScripting;

namespace AILand.UI
{
    public class InventoryPanel : BaseUIPanel
    {
        private InventoryUI m_inventory;

        private bool m_showChest;
        private int m_chestInventoryId;

        protected override void Start()
        {
            base.Start();
            Hide();
        }

        protected override void BindUI()
        {
            m_inventory = transform.Find("InventoryGrid").GetComponent<InventoryUI>();
        }
        
        protected override void BindListeners()
        {
            EventCenter.AddListener<int>(EventType.OpenBag, ShowInventory);
            EventCenter.AddListener<int>(EventType.OpenChest, ShowChestInventory);
            EventCenter.AddListener<int,int>(EventType.SwitchItemInInventory,SwitchItemInInventory);
        }

        protected override void UnbindListeners()
        {
            EventCenter.RemoveListener<int>(EventType.OpenBag, ShowInventory);
            EventCenter.RemoveListener<int>(EventType.OpenChest, ShowChestInventory);
            EventCenter.RemoveListener<int,int>(EventType.SwitchItemInInventory,SwitchItemInInventory);
        }

        private void ShowInventory(int inventoryId)
        {
            m_inventory.UpdateInventory(inventoryId);
            m_showChest = false;
            Show();
        }

        private void ShowChestInventory(int inventoryId)
        {
            // TODO:
        }
        
        private void SwitchItemInInventory(int from, int to)
        {
            if (!m_showChest)
            {
                DataManager.Instance.PlayerData.SwitchItemInInventory(0, from, to);
            }
            
        }

    }

}