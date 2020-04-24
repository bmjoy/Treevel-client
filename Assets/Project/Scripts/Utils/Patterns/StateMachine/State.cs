namespace Project.Scripts.Utils.Patterns.StateMachine
{
    public interface State
    {
        /// <summary>
        /// この状態遷移した時の処理
        /// </summary>
        /// <param name="from">前の状態（初期状態は`from = null`）</param>
        void OnEnter(State from = null);

        /// <summary>
        /// 次の状態に遷移する前の処理
        /// </summary>
        /// <param name="to">次の状態</param>
        void OnExit(State to);
    }

    public abstract class AbstractState : State
    {
        public virtual void OnEnter(State from = null) {}

        public virtual void OnExit(State to) {}
    }
}
