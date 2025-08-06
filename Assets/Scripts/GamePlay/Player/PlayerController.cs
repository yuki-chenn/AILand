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
        
        
        float jumpElapsedTime = 0;

        // Player states
        bool isJumping = false;
        bool isSprinting = false;

        // Inputs
        float inputHorizontal;
        float inputVertical;
        bool inputJump;
        bool inputSprint;

        Animator animator;
        CharacterController cc;
        void Start()
        {
            cc = GetComponent<CharacterController>();
            animator = GetComponent<Animator>();
            if (animator == null)
                Debug.LogWarning("Hey buddy, you don't have the Animator component in your player. Without it, the animations won't work.");
        }

        void Update()
        {
            inputHorizontal = Input.GetAxis("Horizontal");
            inputVertical = Input.GetAxis("Vertical");
            inputJump = Input.GetAxis("Jump") == 1f;
            inputSprint = Input.GetAxis("Fire3") == 1f;

            if ( cc.isGrounded && animator != null )
            {
                
                float minimumSpeed = 0.9f;
                animator.SetBool("run", cc.velocity.magnitude > minimumSpeed );

                isSprinting = cc.velocity.magnitude > minimumSpeed && inputSprint;
                animator.SetBool("sprint", isSprinting );

            }

            if( animator != null )
                animator.SetBool("air", cc.isGrounded == false );

            if (enableJump && inputJump && cc.isGrounded )
            {
                isJumping = true;
            }

            HeadHittingDetect();

        }
        
        private void FixedUpdate()
        {
            float velocityAdittion = 0;
            if ( isSprinting )
                velocityAdittion = sprintAdittion;

            float directionX = inputHorizontal * (velocity + velocityAdittion) * Time.deltaTime;
            float directionZ = inputVertical * (velocity + velocityAdittion) * Time.deltaTime;
            float directionY = 0;

            if ( enableJump && isJumping )
            {

                directionY = Mathf.SmoothStep(jumpForce, jumpForce * 0.30f, jumpElapsedTime / jumpTime) * Time.deltaTime;

                jumpElapsedTime += Time.deltaTime;
                if (jumpElapsedTime >= jumpTime)
                {
                    isJumping = false;
                    jumpElapsedTime = 0;
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
            cc.Move( moviment );

        }


        void HeadHittingDetect()
        {
            if (!enableJump) return;

            float headHitDistance = 1.1f;
            Vector3 ccCenter = transform.TransformPoint(cc.center);
            float hitCalc = cc.height / 2f * headHitDistance;

            if (Physics.Raycast(ccCenter, Vector3.up, hitCalc))
            {
                jumpElapsedTime = 0;
                isJumping = false;
            }
        }
    }
}