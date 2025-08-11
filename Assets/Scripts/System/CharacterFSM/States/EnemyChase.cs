using AILand.GamePlay.Battle.Enemy;
using UnityEngine;

namespace AILand.System.CharacterFSM
{
    [CFSMStateID(CFSMStateID.EnemyChase)]

    public class EnemyChase : CFSMState
    {
        public override CFSMStateID StateID => CFSMStateID.EnemyChase;

        private BaseEnemy m_enemy => System.GetComponent<BaseEnemy>();

        public EnemyChase()
        {
            AddTransition(CFSMTransition.LeaveChaseRange, CFSMStateID.EnemyIdle);
            AddTransition(CFSMTransition.EnterAttackRange, CFSMStateID.EnemyAttack);
            AddTransition(CFSMTransition.Dead, CFSMStateID.EnemyDead);
        }

        public override void DoBeforeEntering()
        {
            m_enemy.Animator.SetBool("chase", true);
            // m_enemy.GetComponent<CharacterController>().enabled = true;
            m_enemy.IsMoving = true;
        }

        public override void DoBeforeLeaving()
        {
            m_enemy.Animator.SetBool("chase", false);
        }

        public override void Update()
        {
            // base.Update();
            m_enemy.Chase();
        }

        public override void Reason()
        {
            if (m_enemy.IsDead) System.PerformTransition(CFSMTransition.Dead);
            
            if (!m_enemy.ChasePlayer)
            {
                System.PerformTransition(CFSMTransition.LeaveChaseRange);
            }
            
            if (m_enemy.CanAttackTarget())
            {
                System.PerformTransition(CFSMTransition.EnterAttackRange);
            }
        }
    }

}