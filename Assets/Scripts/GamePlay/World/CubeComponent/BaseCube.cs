using System.Collections;
using System.Collections.Generic;
using AILand.System.ObjectPoolSystem;
using UnityEngine;

namespace AILand.GamePlay.World.Cube
{
    public class BaseCube : MonoBehaviour, IPooledObject
    {
        public GameObject GameObject => gameObject;

        public void OnGetFromPool()
        {

        }

        public void OnReleaseToPool()
        {

        }

        public void OnDestroyPoolObject()
        {

        }
    }
}
