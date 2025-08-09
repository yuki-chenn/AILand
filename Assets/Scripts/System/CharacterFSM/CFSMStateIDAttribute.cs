using System;

namespace AILand.System.CharacterFSM
{
    
    [AttributeUsage(AttributeTargets.Class)]
    public class CFSMStateIDAttribute : Attribute
    {
        public CFSMStateID ID { get; }
        public CFSMStateIDAttribute(CFSMStateID id) => ID = id;
    }
}