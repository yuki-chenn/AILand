using AILand.GamePlay.InventorySystem;
using AILand.GamePlay.Player;
using AILand.GamePlay.World;
using AILand.System.Base;
using AILand.System.EventSystem;
using AILand.Utils;
using UnityEngine;
using EventType = AILand.System.EventSystem.EventType;

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
                    PlayerComponent.ChangeItemOnHand(m_curSelectItemIndex);
                    EventCenter.Broadcast(EventType.SelectInventoryItemChange);
                }
            }
        }
        
        private int m_curSelectedElementIndex = 0;
        public int CurSelectedElementIndex
        {
            get => m_curSelectedElementIndex;
            set
            {
                if (m_curSelectedElementIndex != value)
                {
                    m_curSelectedElementIndex = value;
                    PlayerComponent.ChangeElementOnInfiniteGauntlet();
                    EventCenter.Broadcast(EventType.SelectElementChange);
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


        #region UI
        // UI
        private bool m_isShowUI;
        public bool IsShowUI => m_isShowUI;


        protected override void Awake()
        {
            base.Awake();
            EventCenter.AddListener(EventType.OnShowUIPanel, OnUIShow);
            EventCenter.AddListener(EventType.OnHideUIPanel, OnUIHide);
        }

        private void OnDestroy()
        {
            EventCenter.RemoveListener(EventType.OnShowUIPanel, OnUIShow);
            EventCenter.RemoveListener(EventType.OnHideUIPanel, OnUIHide);
        }
        
        
        private void OnUIShow()
        {
            m_isShowUI = true;
        }
        
        private void OnUIHide()
        {
            m_isShowUI = false;
        }

        #endregion


        public void PlayerDie()
        {
            if (DataManager.Instance.PlayerData.CurrentHp > 0) return;

            Debug.Log($"PlayerDie");
            PlayerRebirth();
        }

        public void PlayerRebirth()
        {
            Debug.Log($"PlayerRebirth");
            // 传送玩家到上次的存档点
            Teleport(DataManager.Instance.PlayerData.RebirthPosition);
            // 重置玩家状态
            DataManager.Instance.PlayerData.RestoreAllHp();
        }

        public void Teleport(Vector3 pos)
        {
            if (PlayerComponent == null)
            {
                Debug.LogError("Teleport: player is null");
                return;
            }
            // 需要先load一下
            WorldManager.Instance.ForceLoadAroundPosition(pos);
            
            PlayerComponent.MoveTo(pos);
            Debug.Log($"Teleport: 玩家传送到坐标：{pos}");
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