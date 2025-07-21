using System;
using UnityEngine;

namespace AILand.GamePlay
{
    public class GameManager : Singleton<GameManager>
    {
        public GameObject player;


        protected override void Awake()
        {
            base.Awake();
        }


        private void Start()
        {
            WorldManager.Instance.InitializeFirstBlock();
        }
        
        
        
        
        
        
        
    }
}