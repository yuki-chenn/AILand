using System.Collections;
using System.Collections.Generic;
using AILand.System.ObjectPoolSystem;
using UnityEngine;

public class Cube : MonoBehaviour, IPooledObject
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
