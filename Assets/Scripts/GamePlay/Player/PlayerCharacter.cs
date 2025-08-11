using AILand.GamePlay.Battle;
using AILand.GamePlay.Battle.Enemy;
using AILand.GamePlay.InventorySystem;
using AILand.System.EventSystem;
using AILand.System.ObjectPoolSystem;
using AILand.Utils;
using UnityEngine;
using EventType = AILand.System.EventSystem.EventType;

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

        [Header("携带手套和武器")]
        public GameObject infiniteGauntletGo;
        public GameObject bigSword;

        private CharacterController m_characterController;
        private PlayerController m_playerController;

        private bool m_isInWater = false;
        public bool IsInWater => m_isInWater;

        
        // 护盾
        private bool m_isShielded = false;
        private GameObject m_vfxShield;
        private float m_shieldTimer = 0f;

        private void Awake()
        {
            m_characterController = GetComponent<CharacterController>();
            m_playerController = GetComponent<PlayerController>();
            EventCenter.AddListener(EventType.RefreshBagInventory, OnInventoryChange);
        }

        private void OnDestroy()
        {
            EventCenter.RemoveListener(EventType.RefreshBagInventory, OnInventoryChange);
        }

        private void Update()
        {
            DetectInOuterWater();

            if (m_isShielded && m_shieldTimer > 0f)
            {
                m_shieldTimer -= Time.deltaTime;
            }
            
            if(m_isShielded && m_shieldTimer <= 0f)
            {
                ResetShield();
            }
        }
        
        private void OnDrawGizmos()
        {
            // 设置Gizmos颜色
            UnityEngine.Gizmos.color = Color.red;
    
            // 计算碰撞盒的中心和大小
            Vector3 boxCenter = transform.position + transform.forward * 1.5f + new Vector3(0, 1, 0);
            Vector3 boxSize = new Vector3(3f, 1.6f, 3f); // 注意：Gizmos.DrawWireCube使用全尺寸，不是半尺寸
    
            // 绘制线框立方体
            UnityEngine.Gizmos.DrawWireCube(boxCenter, boxSize);
        }

        public void SwordAttack()
        {
            // 特效
            var vfx = PoolManager.Instance.GetGameObject<VfxController>();
            vfx.GetComponent<VfxController>().Play("SwordPower",bigSword.transform.position);
            
            // 方块碰撞
            Collider[] hits = Physics.OverlapBox(transform.position + transform.forward * 1.5f + new Vector3(0,1,0), 
                new Vector3(1.5f,0.8f,1.5f), Quaternion.identity, 
                LayerMask.GetMask("Enemy"));

            foreach (var enemy in hits)
            {
                if (enemy.transform.CompareTag("Enemy"))
                {
                    var enemyComponent = enemy.GetComponent<BaseEnemy>();
                    if (enemyComponent != null)
                    {
                        // 造成伤害
                        enemyComponent.TakeDamage(100f);
                        // 击退
                        enemyComponent.KnockBack(transform.position, 5f);
                    }
                }
            }
            
            
        }
        
        public void TakeDamageFromEnemy(float damage)
        {
            if (m_isShielded)
            {
                // 护盾抵挡伤害
                Debug.Log("护盾抵挡了伤害");
                ResetShield();
                return;
            }
            DataManager.Instance.PlayerData.ChangeHp(-damage);
        }
        
        public void SetShield()
        {
            m_isShielded = true;
            m_shieldTimer = 5f;
            if (m_vfxShield)
            {
                PoolManager.Instance.Release(m_vfxShield);
                m_vfxShield = null;
            }
        
            m_vfxShield = PoolManager.Instance.GetGameObject<VfxController>();
            m_vfxShield.transform.SetParent(transform);
            m_vfxShield.GetComponent<VfxController>().Play("Defense",transform.position, Quaternion.identity);
        }
        
        public void ResetShield()
        {
            if (!m_isShielded) return;
            m_isShielded = false;
            m_shieldTimer = 0f;
            
            if (m_vfxShield)
            {
                m_vfxShield.GetComponent<VfxController>().Release();
                m_vfxShield = null;
            }
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

        private void OnInventoryChange()
        {
            var itemIndex = GameManager.Instance.CurSelectItemIndex;
            ChangeItemOnHand(itemIndex);
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

        public void ChangeElementOnInfiniteGauntlet()
        {
            foreach(Transform child in infiniteGauntletGo.transform)
            {
                ParticleSystem ps = child.GetComponent<ParticleSystem>();
                if (ps != null)
                {
                    var colorOverLifetime = ps.colorOverLifetime;
            
                    // 设置单一颜色
                    var type = Util.GetSelectedEnergyType();
                    colorOverLifetime.color = Constants.energyColors[type];
                }
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