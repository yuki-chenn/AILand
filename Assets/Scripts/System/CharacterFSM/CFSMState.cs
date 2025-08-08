
using System.Collections.Generic;
using UnityEngine;

namespace AILand.System.CharacterFSM
{
    public abstract class CFSMState
    {

        public abstract CFSMStateID StateID { get; }

        // FSM系统
        protected CFSMSystem m_system;

        public CFSMSystem System
        {
            get => m_system;
            set => m_system = value;
        }

        // 状态转换图
        protected Dictionary<int, int> m_transitionStates = new Dictionary<int, int>();

        public bool AddTransition(CFSMTransition transition, CFSMStateID stateId)
        {
            if (transition == CFSMTransition.NullTransition)
            {
                Debug.LogError("CFSMState.AddTransition() ERROR: NullTransition is not allowed for a real transition");
                return false;
            }

            if (stateId == CFSMStateID.NullState)
            {
                Debug.LogError("FSMState.AddTransition() ERROR: NullState is not allowed for a real State");
                return false;
            }

            if (m_transitionStates.ContainsKey((int)transition))
            {
                Debug.LogError(
                    $"FSMState.AddTransition() ERROR: State {stateId.ToString()} already has transition {transition.ToString()}. Impossible to assign to another state");
                return false;
            }

            m_transitionStates.Add((int)transition, (int)stateId);
            return true;
        }

        public bool DeleteTransition(CFSMTransition transition)
        {
            if (transition == CFSMTransition.NullTransition)
            {
                Debug.LogError("FSMState.DeleteTransition() ERROR: NullTransition is not allowed");
                return false;
            }

            if (!m_transitionStates.ContainsKey((int)transition))
            {
                Debug.LogError(
                    $"FSMState.DeleteTransition() ERROR: Transition {transition.ToString()} passed to {StateID.ToString()} was not on the state's transition list");
                return false;
            }

            m_transitionStates.Remove((int)transition);
            return true;
        }

        public CFSMStateID GetNextState(CFSMTransition transition)
        {
            if (m_transitionStates.ContainsKey((int)transition))
            {
                return (CFSMStateID)m_transitionStates[(int)transition];
            }

            return CFSMStateID.NullState;
        }

        public virtual void DoBeforeEntering()
        {
            // Debug.Log($"[FSM] enter state : {StateID.ToString()}");
        }

        public virtual void DoBeforeLeaving()
        {
            // Debug.Log($"[FSM] leave state : {StateID.ToString()}");
        }

        public virtual void Update()
        {
            // Debug.Log($"[FSM] update : {StateID.ToString()}");
        }

        public virtual void Reason()
        {
            // Debug.Log($"[FSM] reason : {StateID.ToString()}");
        }

    }
}
