namespace Treevel.Common.Patterns.StateMachine
{
    public abstract class StateBase
    {
        /// <summary>
        /// この状態遷移した時の処理
        /// </summary>
        /// <param name="from">前の状態（初期状態は`from = null`）</param>
        public virtual void OnEnter(StateBase from = null) { }

        /// <summary>
        /// 次の状態に遷移する前の処理
        /// </summary>
        /// <param name="to">次の状態</param>
        public virtual void OnExit(StateBase to) { }
    }
}
