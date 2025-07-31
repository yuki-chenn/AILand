using System;
using AILand.GamePlay.Player;
using AILand.System.Base;
using GamePlay.Player;
using UnityEngine;

namespace AILand.GamePlay
{
    public class GameManager : Singleton<GameManager>
    {
        public GameObject player;
        public Camera mainCamera;
        
        
        public Transform blockHolder;


        private CameraController cameraController => mainCamera.transform.parent.GetComponent<CameraController>();

        
        /// <summary>
        /// 玩家和船交互后上船
        /// </summary>
        /// /// <param name="boatObj"></param>
        public void GetOnBoard(GameObject boatObj)
        {
            if(boatObj == null)
            {
                Debug.LogError("GetOnBoard: boatObj is null");
                return;
            }
            // 把物体拿出来
            boatObj.transform.SetParent(transform);
            // 暂时不显示玩家
            player.SetActive(false);
            // 将Camera跟随船
            cameraController.ChangeCameraPlayer(boatObj.transform);
            // 给船一个控制方法
            boatObj.AddComponent<BoatController>().playerTransform = player.transform;
        }
        
        /// <summary>
        ///  玩家下船
        /// </summary>
        /// <param name="boatObj"></param>
        public void GetOffBoard(GameObject boatObj)
        {
            if(boatObj == null)
            {
                Debug.LogError("GetOnBoard: boatObj is null");
            }
            // 恢复玩家显示
            player.SetActive(true);
            // 将Camera跟随玩家
            cameraController.ChangeDefaultCameraPlayer();
            // 移除船的控制方法
            Destroy(boatObj?.GetComponent<BoatController>());
        }
        
        
        
    }
}