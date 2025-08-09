
using AILand.GamePlay.Battle.Enemy;
using UnityEngine;

namespace AILand.System.CharacterFSM
{
    [CFSMStateID(CFSMStateID.EnemyAttack)]
    public class EnemyAttack : CFSMState
    {
        public override CFSMStateID StateID => CFSMStateID.EnemyAttack;

        private BaseEnemy m_enemy => System.GetComponent<BaseEnemy>();
    
        public EnemyAttack()
        {
            AddTransition(CFSMTransition.EnterChaseRange,CFSMStateID.EnemyChase);
            AddTransition(CFSMTransition.LeaveChaseRange, CFSMStateID.EnemyIdle);
            AddTransition(CFSMTransition.Dead, CFSMStateID.EnemyDead);
        }

        public override void Reason()
        {
            if(m_enemy.IsDead) System.PerformTransition(CFSMTransition.Dead); 
            
            if (!m_enemy.CanAttackTarget())
            {
                if(m_enemy.ChasePlayer) System.PerformTransition(CFSMTransition.EnterChaseRange);
                else System.PerformTransition(CFSMTransition.LeaveChaseRange);
            }
        }
        
        public override void DoBeforeEntering()
        {
            m_enemy.IsMoving = false;
        }

        public override void Update()
        {
            m_enemy.Attack();
        }
    }
}

