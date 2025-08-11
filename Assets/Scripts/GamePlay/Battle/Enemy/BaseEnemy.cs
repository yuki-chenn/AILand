using AILand.GamePlay.Player;
using AILand.GamePlay.World;
using AILand.System.CharacterFSM;
using AILand.System.ObjectPoolSystem;
using UnityEngine;

namespace AILand.GamePlay.Battle.Enemy
{
    public class BaseEnemy : MonoBehaviour,IPooledObject
    {
        [Header("速度")]
        public float velocity = 5f;
        
        [Header("冲刺加速度")]
        public float sprintAdittion = 3.5f;
        
        [Header("跳跃参数")]
        public float jumpForce = 18f;
        public float jumpTime = 0.85f;
        
        [Header("重力")]
        public float gravity = 9.8f;
        
        
        [Header("属性")]
        public float maxHp;
        public float attack = 20f;
        public float chaseRange = 10f;
        public float attackRange = 1f;
        public float attackInterval = 1f;
        
        
        private float m_currentHp;
        
        private CFSMSystem m_fsmSystem;
        private CharacterController m_cc;
        private Animator m_animator;
        public Animator Animator => m_animator;
        
        // 移动
        private float m_inputHorizontal;
        private float m_inputVertical;
        private bool m_inputJump;
        
        private bool m_isJumping;
        private bool m_isGrounded;
        
        private float m_jumpElapsedTime = 0;
        
        
        // 寻路
        private PathFinding pathFinding = new PathFinding();
        private Transform m_chaseTarget;
        private Vector3 m_moveTargetPosition;
        private bool m_isMoving = false;

        public bool IsMoving
        {
            get => m_isMoving;
            set => m_isMoving = value;
        }

        private bool m_isChased = false;
        
        public bool IsDead => m_currentHp <= 0;
        public bool ChasePlayer => m_chaseTarget != null;
        
        // 攻击
        private float m_attackTimer = 0f;
        
        // 异常状态
        private bool m_isDie = false;
        private bool m_isSlow = false;
        private GameObject m_vfx;
        private float m_slowTimer = 0f;
        
        protected virtual void Awake()
        {
            m_currentHp = maxHp;
            m_fsmSystem = GetComponent<CFSMSystem>();
            m_cc = GetComponent<CharacterController>();
            m_animator = GetComponentInChildren<Animator>();
        }

        private void Update()
        {
            if(Vector3.Distance(GameManager.Instance.player.transform.position, transform.position) < chaseRange)
            {
                m_chaseTarget = GameManager.Instance.player.transform;
                m_isChased = true;
            }
            else
            {
                m_chaseTarget = null;
            }

            if (!m_isMoving)
            {
                m_inputHorizontal = 0;
                m_inputVertical = 0;
                m_inputJump = false;
            }
            else
            {
                if (!m_isJumping)
                {
                    // 这里根据pathFinding得到的结果输入m_inputHorizontal, m_inputVertical, m_inputJump
                    var res = pathFinding.GetMovement(transform.position, m_moveTargetPosition);
                    m_inputHorizontal = res.direction.x;
                    m_inputVertical = res.direction.z;
                    m_inputJump = res.shouldJump;
                }
                else
                {
                    m_inputJump = false;
                }
            }
            
            if (m_inputJump && m_isGrounded )
            {
                m_isJumping = true;
            }
            HeadHittingDetect();
            
            // timer
            m_attackTimer -= Time.deltaTime;
            if (m_isSlow && m_slowTimer > 0)
            {
                m_slowTimer -= Time.deltaTime;
            }
            if (m_isSlow && m_slowTimer <= 0f)
            {
                ResetSlow();
            }
        }


        private void FixedUpdate()
        {
            if (m_isChased && !m_isMoving) return;
            
            float velocityAdittion = 0;
            if ( m_fsmSystem.CurrentState.StateID == CFSMStateID.EnemyChase )
                velocityAdittion = sprintAdittion;

            if (m_isSlow)
                velocityAdittion -= 1f;

            float directionX = m_inputHorizontal * (velocity + velocityAdittion) * Time.deltaTime;
            float directionZ = m_inputVertical * (velocity + velocityAdittion) * Time.deltaTime;
            float directionY = 0;

            if ( m_isJumping )
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

            // 直接使用世界坐标系的方向
            Vector3 horizontalDirection = new Vector3(directionX, 0, directionZ);

            if (directionX != 0 || directionZ != 0)
            {
                float angle = Mathf.Atan2(directionX, directionZ) * Mathf.Rad2Deg;
                Quaternion rotation = Quaternion.Euler(0, angle, 0);
                transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.15f);
            }

            Vector3 verticalDirection = Vector3.up * directionY;
            Vector3 moviment = verticalDirection + horizontalDirection;
            m_cc.Move( moviment );
            m_isGrounded = m_cc.isGrounded;
        }

        public void SetSlow()
        {
            m_isSlow = true;
            m_slowTimer = 3f;
            if (m_vfx)
            {
                PoolManager.Instance.Release(m_vfx);
                m_vfx = null;
            }
        
            m_vfx = PoolManager.Instance.GetGameObject<VfxController>();
            m_vfx.transform.SetParent(transform);
            m_vfx.GetComponent<VfxController>().Play("Slow",transform.position, Quaternion.identity);
        }

        public void ResetSlow()
        {
            if (!m_isSlow) return;
            m_isSlow = false;
            if (m_vfx)
            {
                m_vfx.GetComponent<VfxController>().Release();
                m_vfx = null;
            }
        }
        
        public void Chase()
        {
            if (m_chaseTarget == null)
            {
                return;
            }
            m_isMoving = true;
            m_moveTargetPosition = m_chaseTarget.position;
        }

        // Animation调用
        public void Attack()
        {
            // 朝向玩家
            if (m_chaseTarget != null)
            {
                Vector3 direction = m_chaseTarget.position - transform.position;
                direction.y = 0;
                if (direction != Vector3.zero)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(direction);
                    transform.rotation = targetRotation;
                }
            }
            
            if (m_attackTimer > 0)
            {
                return;
            }
            
            // 球形检测
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, attackRange,LayerMask.GetMask("Player"));
            if (hitColliders.Length > 0)
            {
                GameManager.Instance.player.GetComponent<PlayerCharacter>().TakeDamageFromEnemy(attack);
            }
            m_animator.SetTrigger("attack");

            m_attackTimer = attackInterval;
        }
        
        public bool CanAttackTarget()
        {
            if(m_chaseTarget == null)
            {
                return false;
            }

            return Vector3.Distance(m_chaseTarget.position, transform.position) < attackRange && 
                   Mathf.Abs(m_chaseTarget.position.y - transform.position.y) < 0.3f;
        }

        public virtual void TakeDamage(float damage)
        {
            m_currentHp -= damage;
            if (m_currentHp <= 0)
            {
                Die();
            }
        }
        
        // 击退效果
        public void KnockBack(Vector3 sourcePosition, float force)
        {
            Vector3 knockbackDirection = (transform.position - sourcePosition).normalized;
            knockbackDirection.y = 0; // 保持在水平面上
            m_cc.Move(knockbackDirection * force);
        }

        protected virtual void Die()
        {
            if (m_isDie) return;
            
            m_isDie = true;
            Debug.Log($"Dead");
            // 全元素加 10
            DataManager.Instance.PlayerData.AddElementalEnergy(new NormalElement(10));
            Invoke("Release",2f);
        }

        public void MoveTo(Vector3 position)
        {
            if (m_cc != null)
            {
                m_cc.enabled = false;
                transform.position = position;
                m_cc.enabled = true;
                Debug.Log($"{gameObject.name} 移动到到坐标：{position}");
            }
            else
            {
                Debug.LogError($"未找到{gameObject.name}的CharacterController组件！");
            }
        }

        private void Release()
        {
            PoolManager.Instance.Release(gameObject);
        }
        
        private void HeadHittingDetect()
        {
            float headHitDistance = 1.1f;
            Vector3 ccCenter = transform.TransformPoint(m_cc.center);
            float hitCalc = m_cc.height / 2f * headHitDistance;

            if (Physics.Raycast(ccCenter, Vector3.up, hitCalc))
            {
                m_jumpElapsedTime = 0;
                m_isJumping = false;
            }
        }

        public GameObject GameObject => gameObject;

        public void OnGetFromPool()
        {
            m_currentHp = maxHp;
            m_fsmSystem.SetCurrentState(m_fsmSystem.initialState);
        }

        public void OnReleaseToPool()
        {
            m_isJumping = false;
            m_isGrounded = true;
            m_jumpElapsedTime = 0;
            m_isMoving = false;
            m_chaseTarget = null;

            m_fsmSystem.SetCurrentState(m_fsmSystem.initialState);
        }

        public void OnDestroyPoolObject()
        {
            
        }
    }
}