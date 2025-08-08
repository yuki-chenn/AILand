using AILand.System.ObjectPoolSystem;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace AILand.GamePlay.Battle
{
    public class BaseMagic : MonoBehaviour,IPooledObject
    {
        public float moveSpeed = 10f;
        public float lifeTime = 5f;

        private Vector3 m_moveTarget = Vector3.zero;
        private float m_lifeTimer = 0f;


        private bool m_isMoving;
            
        
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

            if (m_lifeTimer <= 0f) OnLifeTimeEnd();
        }
        
        private void OnLifeTimeEnd()
        {
            m_isMoving = false;
            m_lifeTimer = 0;
            m_moveTarget = Vector3.zero;
            PoolManager.Instance.Release(gameObject);
        }

        public GameObject GameObject => gameObject;
        
        public void OnGetFromPool()
        {
            
        }

        public void OnReleaseToPool()
        {
            
        }

        public void OnDestroyPoolObject()
        {
            
        }
    }
}