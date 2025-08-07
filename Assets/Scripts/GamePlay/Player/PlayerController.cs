using UnityEngine;

namespace AILand.GamePlay.Player
{
    public class PlayerController : MonoBehaviour
    {
        [Header("速度")]
        public float velocity = 5f;
        
        [Header("冲刺加速度")]
        public float sprintAdittion = 3.5f;
        
        [Header("跳跃参数")]
        public bool enableJump = true;
        public float jumpForce = 18f;
        public float jumpTime = 0.85f;
        
        [Header("重力")]
        public float gravity = 9.8f;
        
        
        private float m_jumpElapsedTime = 0;

        // Player states
        private bool m_isJumping = false;
        private bool m_isSprinting = false;

        // Inputs
        private float m_inputHorizontal;
        private float m_inputVertical;
        private bool m_inputJump;
        private bool m_inputRun;

        private bool m_isGrounded;
        private bool m_isWalk;

        private Animator m_animator;
        private CharacterController m_cc;
        
        void Start()
        {
            m_cc = GetComponent<CharacterController>();
            m_animator = GetComponent<Animator>();
            if (m_animator == null)
                Debug.LogWarning("Hey buddy, you don't have the Animator component in your player. Without it, the animations won't work.");
        }

        void Update()
        {
            m_inputHorizontal = Input.GetAxis("Horizontal");
            m_inputVertical = Input.GetAxis("Vertical");
            m_inputJump = Input.GetAxis("Jump") == 1f;
            m_inputRun = Input.GetAxis("Fire3") == 1f;

            if (m_animator)
            {
                if (m_isGrounded)
                {
                    m_animator.SetBool("walk", m_isWalk);
                    
                    m_isSprinting = m_isWalk && m_inputRun;
                    m_animator.SetBool("run", m_isSprinting );
                }
                m_animator.SetBool("air", m_isGrounded == false );
            }
            
            if (enableJump && m_inputJump && m_isGrounded )
            {
                m_isJumping = true;
            }

            HeadHittingDetect();

        }
        
        private void FixedUpdate()
        {
            float velocityAdittion = 0;
            if ( m_isSprinting )
                velocityAdittion = sprintAdittion;

            float directionX = m_inputHorizontal * (velocity + velocityAdittion) * Time.deltaTime;
            float directionZ = m_inputVertical * (velocity + velocityAdittion) * Time.deltaTime;
            float directionY = 0;

            if ( enableJump && m_isJumping )
            {

                directionY = Mathf.SmoothStep(jumpForce, jumpForce * 0.30f, m_jumpElapsedTime / jumpTime) * Time.deltaTime;

                m_jumpElapsedTime += Time.deltaTime;
                if (m_jumpElapsedTime >= jumpTime)
                {
                    m_isJumping = false;
                    m_jumpElapsedTime = 0;
                }
            }

            directionY = directionY - gravity * Time.deltaTime;

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
            
            
            Vector3 verticalDirection = Vector3.up * directionY;
            Vector3 horizontalDirection = forward + right;

            Vector3 moviment = verticalDirection + horizontalDirection;
            m_cc.Move( moviment );
            
            
            // 状态变化
            float minimumSpeed = 0.9f;
            m_isGrounded = m_cc.isGrounded;
            m_isWalk = m_cc.velocity.magnitude > minimumSpeed;
        }


        void HeadHittingDetect()
        {
            if (!enableJump) return;

            float headHitDistance = 1.1f;
            Vector3 ccCenter = transform.TransformPoint(m_cc.center);
            float hitCalc = m_cc.height / 2f * headHitDistance;

            if (Physics.Raycast(ccCenter, Vector3.up, hitCalc))
            {
                m_jumpElapsedTime = 0;
                m_isJumping = false;
            }
        }
    }
}