using System;
using System.Collections.Generic;
using UnityEngine;

namespace Treevel.Common.Patterns.StateMachine
{
    public class StateMachine
    {
        /// <summary>
        /// 現在の状態
        /// </summary>
        public StateBase CurrentState { get; private set; }

        /// <summary>
        /// このステートマシンで制御できるステートの集合
        /// </summary>
        private readonly HashSet<StateBase> _states;

        /// <summary>
        /// 合法的状態遷移の集合
        /// </summary>
        private readonly Dictionary<StateBase, HashSet<StateBase>> _validTransitions = new Dictionary<StateBase, HashSet<StateBase>>();

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="startState">ステートマシンの最初状態</param>
        /// <param name="allStates">遷移可能の状態集合</param>
        public StateMachine(StateBase startState, IEnumerable<StateBase> allStates)
        {
            _states = new HashSet<StateBase>(allStates);
            CurrentState = startState;
        }

        /// <summary>
        /// ステートマシンを起動する
        /// </summary>
        public void Start()
        {
            CurrentState.OnEnter();
        }

        /// <summary>
        /// 合法的遷移を追加
        /// </summary>
        /// <param name="from"> 遷移の始点状態 </param>
        /// <param name="to"> 遷移の終点状態 </param>
        public void AddTransition(StateBase from, StateBase to)
        {
            if (!IsValidState(from) || !IsValidState(to)) {
                throw new ArgumentException($"Invalid States: from {from} to {to}");
            }

            // fromからの遷移がなければ新しいセットを作成
            if (!_validTransitions.ContainsKey(from)) {
                _validTransitions.Add(from, new HashSet<StateBase>());
            }

            // from から to の遷移を追加
            _validTransitions[from].Add(to);
        }

        /// <summary>
        /// 状態遷移
        /// </summary>
        /// <param name="to">次の状態</param>
        /// <returns>遷移成功かどうか</returns>
        public bool SetState(StateBase to)
        {
            if (!IsValidState(to)) return false;

            if (!IsTransitionValid(to)) {
                Debug.LogWarning($"Invalid state transition [{CurrentState.GetType()}] => [{to.GetType()}]");
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

        /// <summary>
        /// `state`は合法な状態かどうかを確認する
        /// </summary>
        /// <param name="state">確認対象</param>
        /// <returns>合法的な状態かどうか</returns>
        private bool IsValidState(StateBase state)
        {
            return _states.Contains(state);
        }

        /// <summary>
        /// <see cref="CurrentState">`CurrentState`</see>から`to`の遷移は合法かどうかを確認する
        /// </summary>
        /// <param name="to">次の状態</param>
        /// <returns>合法的な遷移かどうか</returns>
        private bool IsTransitionValid(StateBase to)
        {
            return _validTransitions.ContainsKey(CurrentState) && _validTransitions[CurrentState].Contains(to);
        }
    }
}
