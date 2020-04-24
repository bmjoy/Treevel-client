using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.Utils.Patterns.StateMachine
{
    public class StateMachine
    {
        public State CurrentState {get; private set;}

        /// <summary>
        /// このステートマシンで制御できるステートの集合
        /// コンストラクタ以外で書き込み禁止
        /// </summary>
        private readonly HashSet<State> _states;

        private readonly Dictionary<State, HashSet<State>> _validTransitions = new Dictionary<State, HashSet<State>>();

        public StateMachine(State startState, State[] allStates)
        {
            _states = new HashSet<State>(allStates);
            CurrentState = startState;
        }

        public void Start()
        {
            CurrentState.OnEnter(null);
        }

        /// <summary>
        /// 合法的遷移を追加
        /// </summary>
        /// <param name="from"> 遷移の始点状態 </param>
        /// <param name="to"> 遷移の終点状態 </param>
        public void AddTransition(State from, State to)
        {
            if (!IsValidState(from) || !IsValidState(to)) {
                throw new System.ArgumentException($"Invalid States: from {from.ToString()} to {to.ToString()}");
            }

            // fromからの遷移がなければ新しいセットを作成
            if (!_validTransitions.ContainsKey(from)) {
                _validTransitions.Add(from, new HashSet<State>());
            }

            // from から to の遷移を追加
            _validTransitions[from].Add(to);
        }

        public bool SetState(State to)
        {
            if (!IsValidState(to))
                return false;

            if (!IsTransitionValid(to)) {
                Debug.LogWarning($"Invalid state transition [{nameof(CurrentState)}] => [{nameof(CurrentState)}]");
                return false;
            }
            
            // leave current state
            CurrentState.OnExit(to);

            // save current state temporally
            var from = CurrentState;

            // change state
            CurrentState = to;

            // get into new state
            to.OnEnter(from);

            return true;
        }

        private bool IsValidState(State state)
        {
            return _states.Contains(state);
        }

        private bool IsTransitionValid(State to)
        {
            // 設定してないならfalse
            if (!_validTransitions.ContainsKey(CurrentState))
                return false;
            else 
                return _validTransitions[CurrentState].Contains(to);
        }
    }
}
