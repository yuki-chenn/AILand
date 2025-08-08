using AILand.GamePlay.InventorySystem;
using AILand.GamePlay.World;
using AILand.Utils;
using UnityEngine;

namespace AILand.GamePlay.Player
{
    public class PlayerCharacter : MonoBehaviour
    {
        [Header("水体检测")] 
        public float waterDamage = 50f;
        public float detectWaterRadius = 0.5f;
        public float detectGroundRadius = 1f;
        public LayerMask detectWaterLayer;
        public LayerMask detectGroundLayer;

        [Header("携带手套")]
        public GameObject infiniteGauntletGo;

        private CharacterController m_characterController;
        private PlayerController m_playerController;

        private bool m_isInWater = false;
        public bool IsInWater => m_isInWater;


        private void Awake()
        {
            m_characterController = GetComponent<CharacterController>();
            m_playerController = GetComponent<PlayerController>();
        }

        private void Update()
        {
            DetectInOuterWater();
        }

        private void DetectInOuterWater()
        {
            // 检测玩家是否在外部水域中

            Collider[] waterHits = Physics.OverlapSphere(transform.position, detectWaterRadius, detectWaterLayer);

            if(waterHits.Length > 0)
            {
                m_isInWater = true;
                m_playerController.enableJump = false;
            }
            else
            {
                m_isInWater = false;
                m_playerController.enableJump = true;
            }

            if (m_isInWater)
            {
                Collider[] hits = Physics.OverlapSphere(transform.position + new Vector3(0,1,0), detectGroundRadius, detectGroundLayer);
                if (hits.Length > 0)
                {
                    m_playerController.enableJump = true;
                }
            }

            // 如果玩家在水中，持续掉血
            if (m_isInWater)
            {
                DataManager.Instance.PlayerData.ChangeHp(-waterDamage * Time.deltaTime);
            }

        }
        
        public void ChangeItemOnHand(int itemIndex)
        {
            // 切换手中的物品
            var itemData = DataManager.Instance.PlayerData.GetItemInInventory(0, itemIndex);
            var item = ItemFactory.GetItemByID(itemData.itemID);
            if (item.itemEnum == ItemEnum.InfiniteGauntlet)
            {
                infiniteGauntletGo.SetActive(true);
            }else
            {
                infiniteGauntletGo.SetActive(false);
            }
        }


        public void MoveTo(Vector3 position)
        {
            if (m_characterController != null)
            {
                m_characterController.enabled = false;
                transform.position = position;
                m_characterController.enabled = true;
                Debug.Log($"玩家移动到到坐标：{position}");
            }
            else
            {
                Debug.LogError("未找到玩家的CharacterController组件！");
            }
        }
    }
}