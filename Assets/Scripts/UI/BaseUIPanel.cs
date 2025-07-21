using System;
using AILand.System.EventSystem;
using UnityEngine;
using EventType = AILand.System.EventSystem.EventType;

namespace AILand.UI
{
    public class BaseUIPanel : MonoBehaviour
    {
        
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