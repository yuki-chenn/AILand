using System;
using UnityEngine;

namespace AILand.GamePlay.Player
{
    public class PlayerCharacter : MonoBehaviour
    {
        private CharacterController m_controller;

        private void Awake()
        {
            m_controller = GetComponent<CharacterController>();
        }


        public void MoveTo(Vector3 position)
        {
            if (m_controller != null)
            {
                m_controller.enabled = false;
                transform.position = position;
                m_controller.enabled = true;
                Debug.Log($"玩家移动到到坐标：{position}");
            }
            else
            {
                Debug.LogError("未找到玩家的CharacterController组件！");
            }
        }
    }
}