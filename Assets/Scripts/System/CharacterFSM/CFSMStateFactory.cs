using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace AILand.System.CharacterFSM
{
    public static class CFSMStateFactory
    {
        private static readonly Dictionary<CFSMStateID, Type> m_map;

        static CFSMStateFactory()
        {
            m_map = Assembly.GetExecutingAssembly()
                .GetTypes()
                .Where(t => !t.IsAbstract && typeof(CFSMState).IsAssignableFrom(t))
                .Select(t => new
                {
                    Type = t,
                    Attr = t.GetCustomAttribute<CFSMStateIDAttribute>()
                })
                .Where(x => x.Attr != null)
                .ToDictionary(x => x.Attr.ID, x => x.Type);
        }

        public static CFSMState Create(CFSMStateID id)
        {
            if (m_map.TryGetValue(id, out var t))
                return (CFSMState)Activator.CreateInstance(t);
            throw new ArgumentException($"[CFSM] CFSMStateFactory.Create() 找不到状态类：{id.ToString()}");
        }
    }
}