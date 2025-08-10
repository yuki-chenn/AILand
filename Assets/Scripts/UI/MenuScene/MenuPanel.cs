using System;
using System.Collections;
using System.Collections.Generic;
using AILand.System.EventSystem;
using UnityEngine;
using UnityEngine.UI;
using EventType = AILand.System.EventSystem.EventType;

namespace AILand.UI
{
    public class MenuPanel : BaseUIPanel
    {
        private Button m_btnStartGame;
        private Button m_btnExitGame;
        

        protected override void BindUI()
        {
            m_btnStartGame = transform.Find("Btns/BtnStartGame").GetComponent<Button>();
            m_btnExitGame = transform.Find("Btns/BtnExitGame").GetComponent<Button>();

            m_btnStartGame.onClick.AddListener(OnClickStartGameBtn);
            m_btnExitGame.onClick.AddListener(OnClickExitGameBtn);
        }

        
        private void OnClickStartGameBtn()
        {
            Debug.Log($"start game!!!");
            EventCenter.Broadcast(EventType.StartGame);
            Hide();
        }
        
        private void OnClickExitGameBtn()
        {
            Debug.Log($"exit game!!!");
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        
    }
}
