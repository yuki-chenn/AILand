using AILand.GamePlay.Battle.Enemy;

namespace AILand.System.CharacterFSM
{
    [CFSMStateID(CFSMStateID.EnemyIdle)]

    public class EnemyIdle : CFSMState
    {
        public override CFSMStateID StateID => CFSMStateID.EnemyIdle;

        private BaseEnemy m_enemy => System.GetComponent<BaseEnemy>();

        public EnemyIdle()
        {
            AddTransition(CFSMTransition.EnterChaseRange, CFSMStateID.EnemyChase);
            AddTransition(CFSMTransition.EnterAttackRange, CFSMStateID.EnemyAttack);
            AddTransition(CFSMTransition.Dead, CFSMStateID.EnemyDead);
        }

        public override void DoBeforeEntering()
        {
            m_enemy.IsMoving = false;
        }
        
        public override void Reason()
        {
            if (m_enemy.IsDead) System.PerformTransition(CFSMTransition.Dead);
            
            if (m_enemy.ChasePlayer)
            {
                System.PerformTransition(CFSMTransition.EnterChaseRange);
            }
            
            if (m_enemy.CanAttackTarget())
            {
                System.PerformTransition(CFSMTransition.EnterAttackRange);
            }
        }
        
    }

}