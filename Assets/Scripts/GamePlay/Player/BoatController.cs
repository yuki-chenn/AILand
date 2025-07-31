using UnityEngine;

namespace AILand.GamePlay.Player
{
    public class BoatController : MonoBehaviour
    {
        public Transform playerTransform;
        
        public float velocity = 5f;
        
        public float sprintAddition = 3.5f;
        
        // 船的冲刺
        private bool m_isSprinting = false;

        // Inputs
        private float m_inputHorizontal;
        private float m_inputVertical;
        private bool m_inputSprint;
        
        private CharacterController m_boatController;
        void Start()
        {
            m_boatController = GetComponent<CharacterController>();
        }
        
        void Update()
        {
            m_inputHorizontal = Input.GetAxis("Horizontal");
            m_inputVertical = Input.GetAxis("Vertical");
            m_inputSprint = Mathf.Approximately(Input.GetAxis("Fire3"), 1f);
            
            // 加速
            m_isSprinting = m_boatController.velocity.magnitude > 0.9f && m_inputSprint;
            
            
            
            // 让玩家跟着一起移动
            if (playerTransform) playerTransform.position = transform.position;
        }

        
        private void FixedUpdate()
        {
            float velocityAdittion = 0;
            if ( m_isSprinting )
                velocityAdittion = sprintAddition;

            // 方向
            float directionX = m_inputHorizontal * (velocity + velocityAdittion) * Time.deltaTime;
            float directionZ = m_inputVertical * (velocity + velocityAdittion) * Time.deltaTime;
            float directionY = 0;
            
            directionY = directionY - 100 * Time.deltaTime;
            
            // 相机相对位置
            Vector3 forward = Camera.main.transform.forward;
            Vector3 right = Camera.main.transform.right;

            forward.y = 0;
            right.y = 0;

            forward.Normalize();
            right.Normalize();
            
            forward = forward * directionZ;
            right = right * directionX;

            if (directionX != 0 || directionZ != 0)
            {
                float angle = Mathf.Atan2(forward.x + right.x, forward.z + right.z) * Mathf.Rad2Deg;
                Quaternion rotation = Quaternion.Euler(0, angle, 0);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.15f);
            }

            
            // 移动
            Vector3 verticalDirection = Vector3.up * directionY;
            Vector3 horizontalDirection = forward + right;

            Vector3 moviment = verticalDirection + horizontalDirection;
            m_boatController.Move( moviment );

        }
    }
}