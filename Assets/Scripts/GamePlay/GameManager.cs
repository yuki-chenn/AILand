using System;
using AILand.System.Base;
using UnityEngine;

namespace AILand.GamePlay
{
    public class GameManager : Singleton<GameManager>
    {
        public GameObject player;
        public Camera mainCamera;
        
        public Transform blockHolder;
    }
}