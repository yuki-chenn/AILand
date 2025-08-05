using System;
using AILand.System.Base;
using GamePlay.Player;

namespace AILand.GamePlay
{
    public class DataManager : Singleton<DataManager>
    {
        public PlayerData PlayerData = new PlayerData();

        private void Start()
        {
            PlayerData.AddInventory();
            PlayerData.AddItem(1,1);
            PlayerData.AddItem(2,100);
        }
    }
}