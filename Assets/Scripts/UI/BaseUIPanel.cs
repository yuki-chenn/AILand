using System;
using AILand.System.EventSystem;
using UnityEngine;
using EventType = AILand.System.EventSystem.EventType;

namespace AILand.UI
{
    public abstract class BaseUIPanel : MonoBehaviour
    {
        protected virtual void Awake()
        {
            BindUI();
            BindListeners();
        }

        protected virtual void Start()
        {
            
        }

        protected void Update()
        {
            
        }

        protected virtual void OnDestroy()
        {
            UnbindListeners();
        }

        protected virtual void BindUI()
        {
            
        }
        
        protected virtual void BindListeners()
        {
            
        }
        
        protected virtual void UnbindListeners()
        {
            
        }
        
        protected virtual void Show()
        {
            gameObject.SetActive(true);
        }
        
        protected virtual void Hide()
        {
            gameObject.SetActive(false);
        }
        
        protected virtual void OnEnable()
        {
            EventCenter.Broadcast(EventType.OnShowUIPanel);
        }
        
        protected virtual void OnDisable()
        {
            EventCenter.Broadcast(EventType.OnHideUIPanel);
        }
    }
}