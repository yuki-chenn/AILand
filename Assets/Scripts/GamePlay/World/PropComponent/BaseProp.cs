using AILand.System.ObjectPoolSystem;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace AILand.GamePlay.World.Prop
{
    public class BaseProp : MonoBehaviour, IPooledObject, IInteractable
    {
        public GameObject GameObject => gameObject;
        public virtual PropType PropType { get; }
        
        private Outline m_focusOutline;


        protected virtual void Awake()
        {
            m_focusOutline = GetComponent<Outline>();
        }
        
        
        
        #region 对象池接口

        public void OnGetFromPool()
        {
            if(m_focusOutline) m_focusOutline.enabled = false;
        }

        public void OnReleaseToPool()
        {
            transform.rotation = Quaternion.identity;
        }

        public void OnDestroyPoolObject()
        {

        }

        #endregion


        #region 交互接口

        public virtual void OnFocus()
        {
            if (m_focusOutline)
            {
                m_focusOutline.enabled = true;
                m_focusOutline.ForceUpdateMaterialProperties();
            }
            
        }

        public virtual void OnLostFocus()
        {
            if(m_focusOutline) m_focusOutline.enabled = false;
        }

        public virtual void Interact()
        {
            
        }

        #endregion
    }
}