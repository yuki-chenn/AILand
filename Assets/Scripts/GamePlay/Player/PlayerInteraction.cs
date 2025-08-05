using AILand.GamePlay.World;
using AILand.GamePlay.World.Cube;
using AILand.System.EventSystem;
using AILand.Utils;
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

        void Update()
        {
            DetectInteractableProp();

            DetectInteractableCube();

             // 打开背包
            if (Input.GetKeyDown(KeyCode.B))
            {
                EventCenter.Broadcast(EventType.OpenBag, 0);
            }
            
            // 滑动滚轮更改选择
            HandleScrollInput();
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
            
            // 左键点击放置
            if(Input.GetMouseButtonDown(0))
            {
                if (m_cubeFoucs != null)
                {
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
            
                        // 在新位置创建方块
                        WorldManager.Instance.PlaceCube(gridPosition, CubeType.Stone, m_cubeFoucs as BaseCube); 
                    }
                }
            }
            
            
            // 右键点击破坏
            if (Input.GetMouseButtonDown(1))
            {
                if(m_cubeFoucs != null)
                {
                    var cube = m_cubeFoucs as BaseCube;
                    WorldManager.Instance.DestoryCube(cube);
                }
            }
            
            
            
        }
        
        
    }
}