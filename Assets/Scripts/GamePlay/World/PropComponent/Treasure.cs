using AILand.System.ObjectPoolSystem;
using AILand.Utils;
using System;
using AILand.GamePlay.Battle;
using AILand.GamePlay.Battle.Enemy;
using UnityEngine;

namespace AILand.GamePlay.World.Prop
{
    public class Treasure : BaseProp
    {
        public override PropType PropType => PropType.Treasure;

        [Header("敌人数量范围")]
        public Vector2Int enemyCountRange;

        [Header("触发召唤敌人范围")]
        public float triggerRadius = 20f;

        [Header("敌人召唤出来的位置范围")]
        public float summonRadius = 10f;


        // 触发召唤怪物
        private bool m_isSummoned = false;
        
        // 开启
        private bool m_isOpen = false;
        
        private void Update()
        {

            if (!m_isSummoned)
            {
                if(Vector3.Distance(GameManager.Instance.player.transform.position, transform.position) < triggerRadius)
                {
                    // 玩家靠近宝箱时，召唤敌人
                    SummonEnemy();
                }
            }

        }
        
        private void SummonEnemy()
        {
            int count = Util.GetRandomInRange(enemyCountRange.x, enemyCountRange.y);
            for(int i = 0; i < count; i++)
            {
                var enemyType = Util.GetRandomElement(Constants.EnemyTypes);
                var enemyInstance = PoolManager.Instance.GetGameObject(enemyType);
                // 随机放在宝箱周围
                var summonPos = RandomSummonPos(transform.position, summonRadius);
                enemyInstance.GetComponent<BaseEnemy>().MoveTo(summonPos);
            }

            m_isSummoned = true;
        }

        private Vector3 RandomSummonPos(Vector3 centerPos,float radius)
        {
            float r = Util.GetRandomInRange(3, radius);
            float angle = Util.GetRandomInRange(0f, 360f);
            float x = centerPos.x + r * Mathf.Cos(angle * Mathf.Deg2Rad);
            float z = centerPos.z + r * Mathf.Sin(angle * Mathf.Deg2Rad);
            return new Vector3(x, centerPos.y + 5, z);
        }
        
        public override void Interact()
        {
            if (m_isOpen) return;
            
            m_isOpen = true;

            // TODO : 放个动画打开盖子
            ShowAnimationAndEffect();

            // 获得元素
            DataManager.Instance.PlayerData.AddElementalEnergy(new NormalElement(100));
            
            // Invoke("Release",2f);
        }

        private void ShowAnimationAndEffect()
        {
            GetComponent<Animator>()?.SetTrigger("open");
            var vfx = PoolManager.Instance.GetGameObject<VfxController>();
            vfx.GetComponent<VfxController>().Play("TreasureOpenEffect",transform.position);
        }


        public void Release()
        {
            PoolManager.Instance.Release(gameObject);
        }
    }
}