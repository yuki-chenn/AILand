using System;
using System.Collections;
using System.Collections.Generic;
using AILand.System.EventSystem;
using UnityEngine;
using EventType = AILand.System.EventSystem.EventType;

namespace AILand.GamePlay.Player   
{
    public class CameraController : MonoBehaviour
    {

        public Transform player;
        public float sensitivity = 5f;
        public bool canZoom = true;
        public Vector2 cameraLimit = new Vector2(-45, 40);

        private bool isUIOpen = false;

        private float m_mouseX;
        private float m_mouseY;
        private float m_offsetDistanceY;


        private void Awake()
        {
            EventCenter.AddListener(EventType.OnShowUIPanel, OnShowUIPanel);
            EventCenter.AddListener(EventType.OnHideUIPanel, OnHideUIPanel);
        }

        void Start()
        {
            m_offsetDistanceY = transform.position.y;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            if (isUIOpen) return;

            transform.position = player.position + new Vector3(0, m_offsetDistanceY, 0);

            if (canZoom && Input.GetAxis("Mouse ScrollWheel") != 0)
                Camera.main.fieldOfView -= Input.GetAxis("Mouse ScrollWheel") * sensitivity * 2;

            m_mouseX += Input.GetAxis("Mouse X") * sensitivity;
            m_mouseY += Input.GetAxis("Mouse Y") * sensitivity;

            m_mouseY = Mathf.Clamp(m_mouseY, cameraLimit.x, cameraLimit.y);

            transform.rotation = Quaternion.Euler(-m_mouseY, m_mouseX, 0);
        }

        private void OnDestroy()
        {
            EventCenter.RemoveListener(EventType.OnShowUIPanel, OnShowUIPanel);
            EventCenter.RemoveListener(EventType.OnHideUIPanel, OnHideUIPanel);
        }


        private void OnShowUIPanel()
        {
            isUIOpen = true;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        private void OnHideUIPanel()
        {
            isUIOpen = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
