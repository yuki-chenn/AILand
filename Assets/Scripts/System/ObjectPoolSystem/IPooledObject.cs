using UnityEngine;
using UnityEngine.Pool;

namespace AILand.System.ObjectPoolSystem
{
    public interface IPooledObject
    {
        // 获取对象所在的GameObject
        GameObject GameObject { get; }
        
        // 当从池中获取时调用
        void OnGetFromPool();
        
        // 当返回池中时调用
        void OnReleaseToPool();
        
        // 当对象被销毁时调用
        void OnDestroyPoolObject();
    }
}