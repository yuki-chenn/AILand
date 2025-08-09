namespace AILand.System.CharacterFSM
{
    public enum CFSMTransition
    {
        NullTransition = 0,
        
        EnterChaseRange,
        LeaveChaseRange,
        EnterAttackRange,
        Dead,
    }
}
