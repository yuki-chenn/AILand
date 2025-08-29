using System;
using AILand.GamePlay.InventorySystem;
using AILand.GamePlay.Player;
using AILand.GamePlay.World;
using AILand.GamePlay.World.Cube;
using AILand.GamePlay.World.Prop;
using AILand.System.EventSystem;
using AILand.System.SOManager;
using AILand.Utils;
using GamePlay.InventorySystem;
using UnityEngine;
using EventType = AILand.System.EventSystem.EventType;

namespace AILand.GamePlay
{
    public class PlayerInteraction : MonoBehaviour
    {
        public float propInteractRadius = 3f;
        public LayerMask propInteractLayer;
        
        public LayerMask cubeLayer;
        public float rayStartDistance = 5f;
        public float raycastDistance = 5f;

        private IInteractable m_interactableProp;
        
        private Camera playerCamera => GameManager.Instance.mainCamera;

        private Ray m_ray
        {
            get
            {
                Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
                Vector3 rayStart = ray.origin + ray.direction * rayStartDistance;
                Ray offsetRay = new Ray(rayStart, ray.direction);
                return offsetRay;
            }
        }

        private IInteractable m_cubeFoucs;
        
        // 当前选中的物品
        private BaseItem m_currentSelectItem => GameManager.Instance.GetCurrentSelectItem();
        private ItemType m_currentSelectItemType => m_currentSelectItem == null ? ItemType.None : m_currentSelectItem.config.itemType;

        // 鼠标长按充能
        private bool m_isCharge = false;
        private float m_chargeTimer = 0f;

        private PlayerController m_playerController => GetComponent<PlayerController>();
        private PlayerCharacter m_playerCharacter => GetComponent<PlayerCharacter>();
        
        
        void Update()
        {
            if (GameManager.Instance.IsShowUI) return;
            
            // 滑动滚轮更改选择
            HandleScrollInput();
            
            // 检测周围交互的道具
            DetectInteractableProp();

            // 检测充能
            DetectCrystalCharge();
            
            // 检测方块交互
            DetectInteractableCube();
            
            // 检测方块放置
            DetectPlaceCube();
            
            // 检测手套功能
            DetectInfiniteGauntlet();

            // 检测数字键选择元素
            DetectElementSelect();
            
             // 打开背包
            if (Input.GetKeyDown(KeyCode.B))
            {
                EventCenter.Broadcast(EventType.OpenBag, 0);
            }
            
            if (Input.GetKeyDown(KeyCode.H))
            {
                EventCenter.Broadcast(EventType.SwitchShowInputHint);
            }
            
            // 暂停游戏
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                EventCenter.Broadcast(EventType.PauseGame);
                GameManager.Instance.PauseGame();
            }
            
        }

        
        
        private void DetectCrystalCharge()
        {
            if (m_currentSelectItemType != ItemType.InfiniteGauntlet) return;
            // 长按鼠标左键为水晶充能
            if(m_interactableProp is MagicCrystal)
            {
                // 检测鼠标按下
                if (Input.GetKeyDown(KeyCode.E))
                {
                    m_isCharge = true;
                }
                
                // 检测鼠标松开
                if (Input.GetKeyUp(KeyCode.E))
                {
                    m_isCharge = false;
                    SetChargeLine(false);
                }
            }
            else
            {
                m_isCharge = false;
            }

            if (m_isCharge && m_interactableProp is MagicCrystal crystal)
            {
                if (m_chargeTimer > 0)
                {
                    m_chargeTimer -= Time.deltaTime;
                    return;
                }
                // 获取当前选中的元素
                var element = Util.GetSelectedEnergyType();

                SetChargeLine(true);
                
                // 充能水晶
                Debug.Log($"charging crystal with element: {element}");
                crystal.Charge(element);
                DataManager.Instance.PlayerData.ConsumeElementalEnergy(element, 1);
                m_chargeTimer = 0.1f; // 每0.2秒充能一次
            }
            else
            {
                SetChargeLine(false);
            }
        }

        private void SetChargeLine(bool active)
        {
            var line = m_playerCharacter.infiniteGauntletGo.transform.Find("Line").gameObject;
            
            line.SetActive(active);
            if (!active) return;
            
            var lineRenderer = line.GetComponent<LineRenderer>();
            if (lineRenderer == null || !(m_interactableProp is MagicCrystal crystal)) 
            {
                line.SetActive(false);
                return;
            }
            
            // 设置线条的起点和终点
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, m_playerCharacter.infiniteGauntletGo.transform.position);
            lineRenderer.SetPosition(1, crystal.transform.position);
    
            // 根据当前选中的元素类型设置颜色
            var element = Util.GetSelectedEnergyType();
            Color elementColor = Constants.energyColors[element];
            Gradient gradient = new Gradient();
            gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(elementColor, 0.0f), new GradientColorKey(elementColor, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
            );
            lineRenderer.colorGradient = gradient;
        }

        private void HandleScrollInput()
        {
            float scroll = Input.GetAxis("Mouse ScrollWheel");
            var curSelectIndex = GameManager.Instance.CurSelectItemIndex;
            if (scroll > 0f) // 向上滚动
            {
                GameManager.Instance.CurSelectItemIndex = (curSelectIndex - 1 + 10) % 10;
            }
            else if (scroll < 0f) // 向下滚动
            {
                GameManager.Instance.CurSelectItemIndex = (curSelectIndex + 1 ) % 10;
            }
            
            if(scroll != 0f)
            {
                m_interactableProp = null;
                m_cubeFoucs = null;
            }
        }

        private void DetectElementSelect()
        {
            // 数字键选择元素
            for (int i = 0; i < 5; i++)
            {
                if (Input.GetKeyDown(KeyCode.Alpha1 + i))
                {
                    GameManager.Instance.CurSelectedElementIndex = i;
                    break;
                }
            }
        }
        
        private void DetectInteractableProp()
        {
            // 在玩家位置附近的球形范围内检测
            Collider[] hits = Physics.OverlapSphere(transform.position, propInteractRadius, propInteractLayer);
            
            float minDist = float.MaxValue;
            IInteractable interactableObj = null;
            
            foreach (var hit in hits)
            {
                var it = hit.GetComponent<IInteractable>();
                if (it != null)
                {
                    float dist = Vector3.Distance(transform.position, hit.transform.position);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        interactableObj = it;
                    }
                }
            }

            // 焦点
            if (m_interactableProp != interactableObj)
            {
                m_interactableProp?.OnLostFocus();
                m_interactableProp = interactableObj;
                m_interactableProp?.OnFocus();
            }

            // 交互
            if (m_interactableProp != null)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    m_interactableProp.Interact();
                }
            }
        }

        private void DetectInteractableCube()
        {
            // 从屏幕中心发射射线
            Debug.DrawRay(m_ray.origin, m_ray.direction * raycastDistance, Color.red);
            
            RaycastHit hit;
            if (Physics.Raycast(m_ray, out hit, raycastDistance, cubeLayer))
            {
                var cubeFocus = hit.collider.GetComponent<IInteractable>();
                if (m_cubeFoucs != cubeFocus)
                {
                    m_cubeFoucs?.OnLostFocus();
                    m_cubeFoucs = cubeFocus;
                    m_cubeFoucs?.OnFocus();
                }
            }
            else
            {
                if (m_cubeFoucs != null)
                {
                    m_cubeFoucs.OnLostFocus();
                    m_cubeFoucs = null;
                }
            }
        }
        
        private void DetectPlaceCube()
        {
            if (m_currentSelectItemType != ItemType.PlacedCube) return;
            // 左键点击放置
            if(Input.GetMouseButtonDown(0))
            {
                if (m_cubeFoucs != null)
                {
                    RaycastHit hit;
                    if (Physics.Raycast(m_ray, out hit, raycastDistance, cubeLayer))
                    {
                        // 获取击中面的法向量
                        Vector3 hitNormal = hit.normal;
            
                        // 根据法向量计算新方块的位置
                        Vector3 newCubePosition = hit.point + hitNormal * 0.5f;
            
                        // 将位置转换为整数网格坐标
                        Vector3Int gridPosition = new Vector3Int(
                            Mathf.RoundToInt(newCubePosition.x),
                            Mathf.RoundToInt(newCubePosition.y),
                            Mathf.RoundToInt(newCubePosition.z)
                        );
                        
                        Debug.Log($"Placing cube at grid position: {gridPosition}");
            
                        CubeType placeCubeType = m_currentSelectItem.PlaceCubeType;
                        // 在新位置创建方块
                        var ok = WorldManager.Instance.PlaceCube(gridPosition, placeCubeType, m_cubeFoucs as BaseCube);
                        if (ok)
                        {
                            DataManager.Instance.PlayerData.ConsumeItem(0,m_currentSelectItem.config.itemID,1);
                        }
                    }
                }
            }
        }

        private void DetectInfiniteGauntlet()
        {
            if (m_currentSelectItemType != ItemType.InfiniteGauntlet) return;
            
            // 右键点击破坏
            if (Input.GetMouseButtonDown(1))
            {
                if(m_cubeFoucs != null)
                {
                    var cube = m_cubeFoucs as BaseCube;
                    if(cube == null) return;
                    var ok = WorldManager.Instance.DestroyCube(cube);
                    if (ok)
                    {
                        // 增加能量
                        var storedEnergy = SOManager.Instance.cubeConfigDict[cube.CubeType].elementEnergy;
                        DataManager.Instance.PlayerData.AddElementalEnergy(storedEnergy);
                    }
                }
            }

            // 左键点击攻击
            if (Input.GetMouseButtonDown(0))
            {
                var curSelectElement = Util.GetSelectedEnergyType();
                m_playerController.UseSkill(curSelectElement);
            }
            
        }
        
        
    }
}