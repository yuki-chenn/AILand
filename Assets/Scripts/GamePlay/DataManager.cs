using System;
using AILand.GamePlay.World;
using AILand.System.Base;
using AILand.Utils;
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
        }

        private void Start()
        {
            PlayerData.AddItem(1,1);
            for (int i = 2; i <= 7; i++)
            {
                PlayerData.AddItem(i,Util.GetRandomInRange(50,100));
            }
            
            PlayerData.AddElementalEnergy(new NormalElement(99999,99999,99999,99999,99999));
        }
    }
}