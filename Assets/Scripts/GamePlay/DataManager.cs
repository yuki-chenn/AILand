using System;
using AILand.System.Base;
using GamePlay.Player;

namespace AILand.GamePlay
{
    public class DataManager : Singleton<DataManager>
    {
        public PlayerData PlayerData = new PlayerData();

        protected override void Awake()
        {
            base.Awake();
            PlayerData.AddInventory();
            PlayerData.AddItem(1,1);
            PlayerData.AddItem(2,100);
            PlayerData.AddElementalEnergy(10, 20, 30, 40, 50);
        }
    }
}