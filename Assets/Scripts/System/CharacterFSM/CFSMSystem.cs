using System.Collections.Generic;
using UnityEngine;

namespace AILand.System.CharacterFSM
{
    public class CFSMSystem : MonoBehaviour
    {
        [Header("状态")] public List<CFSMStateID> states = new List<CFSMStateID>();
        public CFSMStateID initialState;

        // 所有的状态
        private Dictionary<int, CFSMState> m_statesIdDictionary = new Dictionary<int, CFSMState>();

        // 当前的状态
        private CFSMStateID m_currentStateId;

        public CFSMState CurrentState => m_statesIdDictionary[(int)m_currentStateId];

        protected virtual void Awake()
        {
            foreach (var stateId in states)
            {
                if (!AddState(stateId))
                {
                    m_statesIdDictionary = null;
                    Debug.LogError($"[CFSM] CFSMSystem.Awake() ERROR : CFSM初始化失败，状态 {stateId.ToString()} 重复");
                    return;
                }
            }

            if (!SetCurrentState(initialState))
            {
                m_statesIdDictionary = null;
                Debug.LogError("[CFSM] CFSMSystem.Awake() ERROR : CFSM初始化失败，初始状态 {initialState.ToString()} 不在状态列表中");
            }
        }

        public bool SetCurrentState(CFSMStateID stateId)
        {
            if (m_statesIdDictionary.ContainsKey((int)stateId))
            {
                CFSMState state = m_statesIdDictionary[(int)stateId];
                state.DoBeforeEntering();
                m_currentStateId = stateId;
                return true;
            }
            else
            {
                Debug.LogError($"[CFSM] CFSMSystem.SetCurrentState() ERROR : 状态 {initialState.ToString()} 不在状态列表中");
                return false;
            }
        }

        public bool AddState(CFSMStateID stateId)
        {
            if (m_statesIdDictionary.ContainsKey((int)stateId))
            {
                Debug.LogError($"[CFSM] CFSMSystem.AddState() ERROR : 状态 {stateId.ToString()} 已经存在");
                return false;
            }
            else
            {
                var state = CFSMStateFactory.Create(stateId);
                state.System = this;
                m_statesIdDictionary.Add((int)stateId, state);
                return true;
            }
        }


        public bool RemoveState(CFSMStateID stateId)
        {
            if (m_statesIdDictionary.ContainsKey((int)stateId))
            {
                m_statesIdDictionary.Remove((int)stateId);
                if (m_currentStateId == stateId)
                {
                    Debug.LogWarning($"[CFSM] CFSMSystem.RemoveState() WARNING : 移除当前状态 {stateId.ToString()}");
                    SetCurrentState(CFSMStateID.NullState);
                }

                return true;
            }
            else
            {
                Debug.LogError($"[CFSM] CFSMSystem.RemoveState() ERROR : 状态 {stateId.ToString()} 不在状态列表中");
                return false;
            }
        }


        public bool PerformTransition(CFSMTransition transition)
        {
            CFSMState currentState = m_statesIdDictionary[(int)m_currentStateId];
            CFSMStateID nextStateId = currentState.GetNextState(transition);
            if (nextStateId == CFSMStateID.NullState)
            {
                Debug.LogError(
                    $"[CFSM] CFSMSystem.PerformTransition() ERROR : 状态 {currentState.ToString()} 没有找到 {transition.ToString()} 的下一个状态");
                return false;
            }

            currentState.DoBeforeLeaving();
            return SetCurrentState(nextStateId);
        }


        protected virtual void Update()
        {
            if (m_currentStateId == CFSMStateID.NullState) return;
            CurrentState.Reason();
            CurrentState.Update();
        }

    }
}