using UnityEngine;

namespace AILand.GamePlay
{
    public class PlayerInteraction : MonoBehaviour
    {
        public float interactRadius = 3f;
        public LayerMask interactLayer;

        private IInteractable currentTarget;
        

        void Update()
        {
            currentTarget = null;

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
                        currentTarget = interactable;
                    }
                }
            }

            if (currentTarget != null)
            {
                if (Input.GetKeyDown(KeyCode.F))
                {
                    currentTarget.Interact();
                }
            }
        }
        
    }
}