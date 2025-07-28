using System.Collections;
using System.Collections.Generic;
using AILand.System.ObjectPoolSystem;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace AILand.GamePlay.World.Cube
{
    public class BaseCube : MonoBehaviour, IPooledObject, IInteractable
    {
        // override
        public GameObject GameObject => gameObject;
        
        
        
        
        private Outline m_focusOutline;


        protected virtual void Awake()
        {
            m_focusOutline = GetComponent<Outline>();
        }
        


        #region 对象池接口

        public void OnGetFromPool()
        {
            m_focusOutline.enabled = false;
        }

        public void OnReleaseToPool()
        {
            
        }

        public void OnDestroyPoolObject()
        {

        }

        #endregion


        #region 交互接口

        public void OnFocus()
        {
            if (m_focusOutline)
            {
                m_focusOutline.enabled = true;
                m_focusOutline.ForceUpdateMaterialProperties();
            }
            
        }

        public void OnLostFocus()
        {
            if(m_focusOutline) m_focusOutline.enabled = false;
        }

        public void Interact()
        {
            
        }

        #endregion
    }
}
