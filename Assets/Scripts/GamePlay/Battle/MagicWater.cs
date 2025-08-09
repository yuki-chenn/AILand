using AILand.GamePlay.Battle.Enemy;
using AILand.System.ObjectPoolSystem;
using UnityEngine;
using NotImplementedException = System.NotImplementedException;

namespace AILand.GamePlay.Battle
{
    public class MagicWater : BaseMagic
    {
        public GameObject GameObject { get; }


        protected override void OnCollisionEnter(Collision other)
        {
            base.OnCollisionEnter(other);
            // 减速敌人
            if (other.collider.CompareTag("Enemy"))
            {
                var enemy = other.collider.GetComponent<BaseEnemy>();
                if (enemy != null)
                {
                    enemy.SetSlow();
                }
            }
        }
    }
    
}