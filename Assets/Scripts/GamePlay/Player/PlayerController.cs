using System.Collections;
using AILand.GamePlay.Battle;
using AILand.GamePlay.World;
using AILand.System.EventSystem;
using AILand.System.ObjectPoolSystem;
using UnityEngine;
using EventType = AILand.System.EventSystem.EventType;
using NotImplementedException = System.NotImplementedException;

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
        
        [Header("技能释放点")]
        public Transform firePoint;
        
        
        private float m_jumpElapsedTime = 0;

        // 状态
        private bool m_isJumping;
        private bool m_isRunning;
        private bool m_isUsingSkill;
        
        // 检测状态的参数，在cc调用之后判断
        private bool m_isGrounded;
        private bool m_isWalking;

        // 输入
        private float m_inputHorizontal;
        private float m_inputVertical;
        private bool m_inputJump;
        private bool m_inputRun;
        
        private Animator m_animator;
        private CharacterController m_cc;
        
        private PlayerCharacter m_playerCharacter => GetComponent<PlayerCharacter>();

        
        private Ray m_ray
        {
            get
            {
                Ray ray = GameManager.Instance.mainCamera.ScreenPointToRay(new Vector3(Screen.width / 2f, Screen.height / 2f, 0));
                Vector3 rayStart = ray.origin + ray.direction * 6;
                Ray offsetRay = new Ray(rayStart, ray.direction);
                return offsetRay;
            }
        }
        
        void Start()
        {
            m_cc = GetComponent<CharacterController>();
            m_animator = GetComponent<Animator>();
        }

        void Update()
        {
            
            
            m_inputHorizontal = Input.GetAxis("Horizontal");
            m_inputVertical = Input.GetAxis("Vertical");
            m_inputJump = Input.GetAxis("Jump") == 1f;
            m_inputRun = Input.GetAxis("Fire3") == 1f;
            
            if (m_isUsingSkill || GameManager.Instance.IsShowUI)
            {
                // 使用技能时不可以移动
                m_inputHorizontal = 0;
                m_inputVertical = 0;
                m_inputJump = false;
                m_inputRun = false;
            }

            if (m_animator)
            {
                if (m_isGrounded)
                {
                    m_animator.SetBool("walk", m_isWalking);
                    
                    m_isRunning = m_isWalking && m_inputRun;
                    m_animator.SetBool("run", m_isRunning );
                }
                m_animator.SetBool("air", !m_isGrounded );
                m_animator.SetBool("water", m_playerCharacter.IsInWater);
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
            if ( m_isRunning )
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

            directionY -= gravity * Time.deltaTime;

            Vector3 forward = GameManager.Instance.mainCamera.transform.forward;
            Vector3 right = GameManager.Instance.mainCamera.transform.right;

            forward.y = 0;
            right.y = 0;

            forward.Normalize();
            right.Normalize();

            forward *= directionZ;
            right *= directionX;

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
            m_isWalking = m_cc.velocity.magnitude > minimumSpeed;
            if (!m_isGrounded) m_isUsingSkill = false;
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
        

        public void UseSkill(EnergyType curSelectElement)
        {
            // 在水中空中不能释放技能
            if (m_isUsingSkill || m_playerCharacter.IsInWater || !m_isGrounded) return;
            m_isUsingSkill = true;
            
            switch (curSelectElement)
            {
                case EnergyType.Metal:
                    SkillSword();
                    break;
                case EnergyType.Wood:
                    SkillRestore();
                    break;
                case EnergyType.Earth:
                    SkillDefense();
                    break;
                case EnergyType.Fire:
                case EnergyType.Water:
                    SkillCast(curSelectElement);
                    break;
            }
            // 扣除元素
            DataManager.Instance.PlayerData.ConsumeElementalEnergy(curSelectElement, 1);
        }

        public void SkillSword()
        {
            Debug.Log($"我砍");
            m_animator.SetTrigger("skillSword");
        }

        public void SkillCast(EnergyType curSelectElement)
        {
            Debug.Log($"我施法 {curSelectElement}");
            m_animator.SetTrigger("skillCast");
            
            // 获取子弹
            GameObject projectileGo = null;
            switch (curSelectElement)
            {
                case EnergyType.Water:
                    projectileGo = PoolManager.Instance.GetGameObject<MagicWater>();
                    break;
                case EnergyType.Fire:
                    projectileGo = PoolManager.Instance.GetGameObject<MagicFire>();
                    break;
                default:
                    Debug.LogError($"no projectile for {curSelectElement}");
                    break;
            }
            projectileGo.transform.position = firePoint.position;
            projectileGo.SetActive(false);
            
            // 获取目标位置
            Debug.DrawRay(m_ray.origin, m_ray.direction * 100, Color.red,2f);
            Vector3 targetPoint;
            if (Physics.Raycast(m_ray, out RaycastHit hitInfo))
                targetPoint = hitInfo.point;
            else
                targetPoint = m_ray.GetPoint(100f);  // 最多 100
            
            Debug.Log($"targetPoint: {targetPoint}");
            
            // 朝向目标
            Vector3 direction = (targetPoint - firePoint.position).normalized;
            direction.y = 0;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = targetRotation;
            
            StartCoroutine(Shoot(projectileGo, targetPoint));
        }

        public void SkillDefense()
        {
            Debug.Log($"我防");
            m_animator.SetTrigger("skillDefense");
        }
        
        public void SkillRestore()
        {
            Debug.Log($"我恢");
            m_animator.SetTrigger("skillRestore");
            DataManager.Instance.PlayerData.ChangeHp(20);
        }

        IEnumerator Shoot(GameObject projectileGo,Vector3 targetPoint)
        {
            yield return new WaitForSeconds(0.3f);
            projectileGo.GetComponent<BaseMagic>().MoveForward(targetPoint);
        }
        
        public void Defense()
        {
            m_playerCharacter.SetShield();
        }

        public void Restore()
        {
            var vfx = PoolManager.Instance.GetGameObject<VfxController>();
            vfx.transform.SetParent(transform);
            vfx.GetComponent<VfxController>().Play("Restore",transform.position,transform.rotation);
        }

        public void SummonSword()
        {
            m_playerCharacter.bigSword.SetActive(true);
        }

        public void EndSkill()
        {
            m_isUsingSkill = false;
            m_playerCharacter.bigSword.SetActive(false);
        }
        
    }
}