using AILand.GamePlay.Battle.Enemy;

namespace AILand.System.CharacterFSM
{
    
    [CFSMStateID(CFSMStateID.EnemyDead)]
    public class EnemyDead : CFSMState
    {
        public override CFSMStateID StateID => CFSMStateID.EnemyDead;

        private BaseEnemy m_enemy => System.GetComponent<BaseEnemy>();

        public override void DoBeforeEntering()
        {
            m_enemy.Animator.SetBool("isDead", true);
            m_enemy.IsMoving = false;
        }
    }

}