using AILand.GamePlay.World;
using AILand.GamePlay.World.Cube;
using AILand.Utils;
using UnityEngine;

namespace AILand.GamePlay
{
    public class PlayerInteraction : MonoBehaviour
    {
        public float interactRadius = 3f;
        public LayerMask interactLayer;
        
        public LayerMask cubeLayer;
        public float rayStartDistance = 5f;
        public float raycastDistance = 5f;

        private IInteractable m_interactableObject;
        
        
        private Camera playerCamera => GameManager.Instance.mainCamera;



        private IInteractable m_cubeFoucs;

        void Update()
        {
            DetectInteractableAround();

            DetectInteractableCube();
        }
        
        private void DetectInteractableAround()
        {
            m_interactableObject = null;
            
            // 在玩家位置附近的球形范围内检测
            Collider[] hits = Physics.OverlapSphere(transform.position, interactRadius, interactLayer);
            float minDist = float.MaxValue;
            foreach (var hit in hits)
            {
                var interactable = hit.GetComponent<IInteractable>();
                if (interactable != null)
                {
                    float dist = Vector3.Distance(transform.position, hit.transform.position);
                    if (dist < minDist)
                    {
                        minDist = dist;
                        m_interactableObject = interactable;
                    }
                }
            }

            if (m_interactableObject != null)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    m_interactableObject.Interact();
                }
            }
        }

        private void DetectInteractableCube()
        {
            // 从屏幕中心发射射线
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
            Vector3 rayStart = ray.origin + ray.direction * rayStartDistance;
            Ray offsetRay = new Ray(rayStart, ray.direction);
            
            Debug.DrawRay(offsetRay.origin, offsetRay.direction * raycastDistance, Color.red);
            
            RaycastHit hit;
            if (Physics.Raycast(offsetRay, out hit, raycastDistance, cubeLayer))
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
                // 没有射线命中时清除焦点
                if (m_cubeFoucs != null)
                {
                    m_cubeFoucs.OnLostFocus();
                    m_cubeFoucs = null;
                }
            }
            
            // 射线检测玩家右键点击的方块
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