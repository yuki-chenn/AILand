using AILand.GamePlay.InventorySystem;
using AILand.GamePlay.Player;
using AILand.GamePlay.World;
using AILand.System.Base;
using AILand.System.EventSystem;
using AILand.Utils;
using UnityEngine;
using EventType = AILand.System.EventSystem.EventType;
using NotImplementedException = System.NotImplementedException;

namespace AILand.GamePlay
{
    public class GameManager : Singleton<GameManager>
    {
        // 配置
        public WFCConfigSO WFCConfigSO;
        public GameObject player;
        public Camera mainCamera;
        public Transform blockHolder;
        
        
        // 数据
        private int m_curSelectItemIndex = 0;
        public int CurSelectItemIndex
        {
            get => m_curSelectItemIndex;
            set
            {
                if (m_curSelectItemIndex != value)
                {
                    m_curSelectItemIndex = value;
                    EventCenter.Broadcast(EventType.SelectInventoryItemChange);
                }
            }
        }

        
        
        public CameraController CameraController => mainCamera.transform.parent.GetComponent<CameraController>();
        public PlayerCharacter PlayerComponent => player.GetComponent<PlayerCharacter>();
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
            CameraController.ChangeCameraPlayer(boatObj.transform);
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
            CameraController.ChangeDefaultCameraPlayer();
            // 移除船的控制方法
            Destroy(boatObj?.GetComponent<BoatController>());
            Debug.Log($"GetOffBoard: 玩家下船成功，船名：{boatObj.name}");
        }


        public BaseItem GetCurrentSelectItem()
        {
            var item = DataManager.Instance.PlayerData.GetItemInInventory(0, m_curSelectItemIndex);
            return ItemFactory.GetItemByID(item.itemID);
        }
    }
}