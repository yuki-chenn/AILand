using AILand.GamePlay.Battle.Enemy;
using AILand.System.ObjectPoolSystem;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace AILand.GamePlay.Battle
{
    public class BaseMagic : MonoBehaviour,IPooledObject
    {
        public float moveSpeed = 10f;
        public float lifeTime = 5f;
        public string explosionVfxName;
        public float damage;
        
        private Vector3 m_moveTarget = Vector3.zero;
        private float m_lifeTimer = 0f;
        
        private bool m_isMoving;
        private bool m_isReleased;
        
        
        protected virtual void Move()
        {
            m_isMoving = true;
            m_lifeTimer = lifeTime;
        }

        public virtual void MoveForward(Vector3 forward)
        {
            if(!gameObject.activeSelf) gameObject.SetActive(true);
            m_moveTarget = forward;
            Move();
        }

        protected virtual void Update()
        {
            if (!m_isMoving) return;
            
            var moveDir = (m_moveTarget - transform.position).normalized;
            transform.position += moveDir * (moveSpeed * Time.deltaTime);
            if (moveDir != Vector3.zero)
                transform.rotation = Quaternion.FromToRotation(Vector3.right, moveDir);
           
            m_lifeTimer -= Time.deltaTime;
            
            if(Vector3.Distance(transform.position,m_moveTarget) < 0.001f)
                OnHit();

            if (m_lifeTimer <= 0f) Release();
        }

        
        private void OnCollisionEnter(Collision other)
        {
            Debug.Log($"collide with {other.collider.name}");
            
            // 检测是否为敌人
            if (other.collider.CompareTag("Enemy"))
            {
                var enemy = other.collider.GetComponent<BaseEnemy>(); // 假设敌人实现了IEnemy接口
                if (enemy != null)
                {
                    enemy.TakeDamage(damage);
                }
            }

            OnHit();
        }

        private void ShowExplosionVfx()
        {
            // 从对象池获取特效控制器
            var vfxController = PoolManager.Instance.GetGameObject<VfxController>();
            vfxController.GetComponent<VfxController>().Play(explosionVfxName, transform.position);
        }

        private void OnHit()
        {
            ShowExplosionVfx();
            Release();
        }
        
        private void Release()
        {
            if (m_isReleased) return;
            m_isMoving = false;
            m_lifeTimer = 0;
            m_moveTarget = Vector3.zero;
            PoolManager.Instance.Release(gameObject);
        }

        public GameObject GameObject => gameObject;
        
        public void OnGetFromPool()
        {
            m_isReleased = false;
        }

        public void OnReleaseToPool()
        {
            m_isReleased = true;
        }

        public void OnDestroyPoolObject()
        {
            
        }
    }
}