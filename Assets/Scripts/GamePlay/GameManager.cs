using AILand.GamePlay.Player;
using AILand.GamePlay.World;
using AILand.System.Base;
using AILand.Utils;
using UnityEngine;

namespace AILand.GamePlay
{
    public class GameManager : Singleton<GameManager>
    {
        public WFCConfigSO WFCConfigSO;


        public GameObject player;
        public Camera mainCamera;
        public PlayerCharacter PlayerComponent => player.GetComponent<PlayerCharacter>();
        
        public Transform blockHolder;


        private CameraController cameraController => mainCamera.transform.parent.GetComponent<CameraController>();
        
        public int CurBlockId
        {
            get
            {
                var pos = player.transform.position;
                return Util.GetBlockIDByWorldPosition(pos, Constants.BlockWidth, Constants.BlockHeight);
            }
        }
        
        /// <summary>
        /// 玩家和船交互后上船
        /// </summary>
        /// /// <param name="boatObj"></param>
        public bool GetOnBoard(GameObject boatObj)
        {
            if(boatObj == null)
            {
                Debug.LogError("GetOnBoard: boatObj is null");
                return false;
            }
            // 把物体拿出来
            boatObj.transform.SetParent(transform);
            // 暂时不显示玩家
            player.SetActive(false);
            // 将Camera跟随船
            cameraController.ChangeCameraPlayer(boatObj.transform);
            // 给船一个控制方法
            boatObj.AddComponent<BoatController>().playerTransform = player.transform;
            Debug.Log($"GetOnBoard: 玩家上船成功，船名：{boatObj.name}");
            return true;
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
            // 将玩家移动到船的左侧
            PlayerComponent.MoveTo(boatObj.transform.position + boatObj.transform.right * -2f);
            // 恢复玩家显示
            player.SetActive(true);
            // 将Camera跟随玩家
            cameraController.ChangeDefaultCameraPlayer();
            // 移除船的控制方法
            Destroy(boatObj?.GetComponent<BoatController>());
            Debug.Log($"GetOffBoard: 玩家下船成功，船名：{boatObj.name}");
        }
        
        
        
    }
}