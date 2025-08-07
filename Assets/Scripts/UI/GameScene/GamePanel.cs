using AILand.GamePlay;
using AILand.GamePlay.InventorySystem;
using AILand.System.EventSystem;
using UnityEngine;
using UnityEngine.UI;
using EventType = AILand.System.EventSystem.EventType;

namespace AILand.UI
{
    public class GamePanel : BaseUIPanel
    {
        private Transform m_transInventory10;
        private GameObject[] m_goItems = new GameObject[10];
        private Transform[] m_transItemSlots = new Transform[10];

        private Slider m_sliderHp;

        private ElementEnergyUI m_elementEnergyUI;

        protected override void BindUI()
        {
            m_transInventory10 = transform.Find("Inventory10Grid");
            for (int i = 0; i < 10; i++)
            {
                m_transItemSlots[i] = m_transInventory10.GetChild(i);
                m_goItems[i] = m_transItemSlots[i].Find("Item").gameObject;
            }

            m_sliderHp = transform.Find("SliderHpBar").GetComponent<Slider>();

            m_elementEnergyUI = transform.Find("ElementEnergy").GetComponent<ElementEnergyUI>();
        }
        
        protected override void Start()
        {
            base.Start();
            Update10Inventory();
            UpdateHpSlider();
            m_elementEnergyUI.UpdateSelectElement();
        }

        

        protected override void BindListeners()
        {
            EventCenter.AddListener(EventType.OnShowUIPanel, Hide);
            EventCenter.AddListener(EventType.OnHideUIPanel, Show);
            EventCenter.AddListener(EventType.RefreshBagInventory, Update10Inventory);
            EventCenter.AddListener(EventType.SelectInventoryItemChange,Update10Inventory);
            EventCenter.AddListener<int[]>(EventType.RefreshElementEnergy, UpdateElementEnergy);
            EventCenter.AddListener(EventType.RefreshPlayerHp, UpdateHpSlider);
            EventCenter.AddListener(EventType.SelectElementChange, m_elementEnergyUI.UpdateSelectElement);
        }

        protected override void UnbindListeners()
        {
            EventCenter.RemoveListener(EventType.OnShowUIPanel, Hide);
            EventCenter.RemoveListener(EventType.OnHideUIPanel, Show);
            EventCenter.RemoveListener(EventType.RefreshBagInventory, Update10Inventory);
            EventCenter.RemoveListener(EventType.SelectInventoryItemChange,Update10Inventory);
            EventCenter.RemoveListener<int[]>(EventType.RefreshElementEnergy, UpdateElementEnergy);
            EventCenter.RemoveListener(EventType.RefreshPlayerHp, UpdateHpSlider);
            EventCenter.RemoveListener(EventType.SelectElementChange, m_elementEnergyUI.UpdateSelectElement);
        }

        protected override void OnEnable()
        {
            // 不需要停止camera
        }
        
        protected override void OnDisable()
        {
            // 不需要停止camera
        }

        private void UpdateElementEnergy(int[] delta)
        {
            m_elementEnergyUI.UpdateEnergy(DataManager.Instance.PlayerData.GetElementalEnergy().NormalElement);
            m_elementEnergyUI.UpdateEnergyAdd(delta);
        }


        private void Update10Inventory()
        {
            var inventories = DataManager.Instance.PlayerData.GetItemsInInventory(0);
            var selectIndex = GameManager.Instance.CurSelectItemIndex;
            for(int i = 0; i < 10; i++)
            {
                m_transItemSlots[i].Find("Select").gameObject.SetActive(i == selectIndex);
                
                var item = inventories[i];
                var itemIcon = ItemFactory.GetItemByID(item.itemID).config.itemIcon;
                m_goItems[i].SetActive(item.itemID != 0 && item.itemCount > 0);
                m_goItems[i].GetComponentInChildren<Image>().sprite = itemIcon;
                m_goItems[i].GetComponentInChildren<Text>().text = item.itemCount.ToString();
            }
        }

        private void UpdateHpSlider()
        {
            var curHp = DataManager.Instance.PlayerData.CurrentHp;
            var maxHp = DataManager.Instance.PlayerData.MaxHp;
            if (m_sliderHp != null)
            {
                m_sliderHp.value = curHp / maxHp;
            }
        }
        
        
    }
}