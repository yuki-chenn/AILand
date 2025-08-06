using System.Collections.Generic;
using AILand.GamePlay;
using AILand.GamePlay.InventorySystem;
using AILand.GamePlay.World;
using AILand.System.EventSystem;
using UnityEngine;
using EventType = AILand.System.EventSystem.EventType;

namespace GamePlay.Player
{
    public class PlayerData
    {
        // 血量
        private float m_maxHp;
        public float MaxHp => m_maxHp;
        private float m_currentHp;
        public float CurrentHp => m_currentHp;



        // 玩家收集的元素能量
        private ElementalEnergy m_elementalEnergy;
        
        // 玩家持有的道具
        private InventoryData m_inventoryData;
        private int m_inventoryCounter = 0;

        public PlayerData()
        {
            m_maxHp = 100f; // 初始最大血量
            RestoreAllHp();
            m_elementalEnergy = new ElementalEnergy();
            m_inventoryData = new InventoryData();
        }

        #region HP相关

        public void ChangeHp(float delta)
        {
            m_currentHp += delta;
            var isDie = m_currentHp <= 0;
            m_currentHp = Mathf.Clamp(m_currentHp, 0, m_maxHp);
            BroadcastHpChange();
            if (isDie) GameManager.Instance.PlayerDie();
        }

        public void RestoreAllHp()
        {
            ChangeHp(m_maxHp);
        }

        private void BroadcastHpChange()
        {
            // 广播血量变化事件
            EventCenter.Broadcast(EventType.RefreshPlayerHp);
        }

        #endregion


        #region Elemental Energy

        public ElementalEnergy GetElementalEnergy()
        {
            return m_elementalEnergy;
        }

        public void AddElementalEnergy(EnergyType type, int count)
        {
            if (count <= 0)
            {
                Debug.LogWarning("Cannot add non-positive amount of elemental energy.");
                return;
            }
            int[] delta = new int[5];
            var normalElement = m_elementalEnergy.NormalElement;
            switch (type)
            {
                case EnergyType.Metal:
                    normalElement.Metal += count;
                    delta[0] = count;
                    break;
                case EnergyType.Wood:
                    normalElement.Wood += count;
                    delta[1] = count;
                    break;
                case EnergyType.Water:
                    normalElement.Water += count;
                    delta[2] = count;
                    break;
                case EnergyType.Fire:
                    normalElement.Fire += count;
                    delta[3] = count;
                    break;
                case EnergyType.Earth:
                    normalElement.Earth += count;
                    delta[4] = count;
                    break;
                default:
                    Debug.LogWarning("Unknown energy type.");
                    return;
            }
            m_elementalEnergy.NormalElement = normalElement;
            BroadcastElementEnergyChange(delta);
        }

        public void AddElementalEnergy(NormalElement element)
        {
            var normalElement = m_elementalEnergy.NormalElement;
            normalElement.Metal += element[0];
            normalElement.Wood += element[1];
            normalElement.Water += element[2];
            normalElement.Fire += element[3];
            normalElement.Earth += element[4];
            m_elementalEnergy.NormalElement = normalElement;
            int[] delta = new int[5] { element.Metal, element.Wood, element.Water, element.Fire, element.Earth };
            BroadcastElementEnergyChange(delta);
        }
        
        public void AddElementalEnergy(List<StoredElementEnergy> storedEnergy)
        {
            NormalElement normalEnergy = new NormalElement(0);
            foreach (var energy in storedEnergy)
            {
                switch (energy.energyType)
                {
                    case EnergyType.Metal:
                        normalEnergy[0] += energy.count;
                        break;
                    case EnergyType.Wood:
                        normalEnergy[1] += energy.count;
                        break;
                    case EnergyType.Water:
                        normalEnergy[2] += energy.count;
                        break;
                    case EnergyType.Fire:
                        normalEnergy[3] += energy.count;
                        break;
                    case EnergyType.Earth:
                        normalEnergy[4] += energy.count;
                        break;
                }
            }

            AddElementalEnergy(normalEnergy);
        }

        private void BroadcastElementEnergyChange(int[] delta)
        {
            // 广播能量变化事件
            EventCenter.Broadcast(EventType.RefreshElementEnergy,delta);
        }

        #endregion


        #region Inventory

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
                BroadcastInventoryChange();
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
                BroadcastInventoryChange();
                return true;
            }

            // 背包已满
            Debug.Log($"Inventory {inventoryID} is full, cannot add item {id}.");
            return false;
        }
        
        public bool ConsumeItem(int inventoryId, int itemId, int count=1)
        {
            if (count <= 0)
            {
                Debug.LogWarning("Cannot consume non-positive amount of items.");
                return false;
            }
            
            if (!m_inventoryData.allInventorys.ContainsKey(inventoryId))
            {
                Debug.LogError($"Inventory ID {inventoryId} does not exist.");
                return false;
            }

            var itemList = m_inventoryData.allInventorys[inventoryId];
            int index = itemList.FindIndex(i => i.itemID == itemId);
            if (index < 0)
            {
                Debug.LogWarning($"Item ID {itemId} not found in inventory {inventoryId}.");
                return false;
            }

            var item = itemList[index];
            if (item.itemCount < count)
            {
                Debug.LogWarning($"Not enough items to consume. Available: {item.itemCount}, Required: {count}");
                return false;
            }

            item.itemCount -= count;
            if (item.itemCount <= 0)
            {
                item.itemID = 0; // 清空物品
            }
            itemList[index] = item;

            BroadcastInventoryChange();
            return true;
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
        
        public ItemData GetItemInInventory(int inventoryId, int index)
        {
            if (!m_inventoryData.allInventorys.ContainsKey(inventoryId))
            {
                Debug.LogError($"Inventory ID {inventoryId} does not exist.");
                return new ItemData{itemID = -1,itemCount = 0};
            }
            
            var dataList = m_inventoryData.allInventorys[inventoryId];
            if (dataList == null || index < 0 || index >= dataList.Count)
            {
                Debug.LogError($"Invalid inventory ID {inventoryId} or index out of range.");
                return new ItemData{itemID = -1,itemCount = 0};
            }
            return dataList[index];
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
            BroadcastInventoryChange();
        }
        
        private void BroadcastInventoryChange()
        {
            // 广播背包变化事件
            EventCenter.Broadcast(EventType.RefreshBagInventory);
        }

        #endregion


        
    }
}