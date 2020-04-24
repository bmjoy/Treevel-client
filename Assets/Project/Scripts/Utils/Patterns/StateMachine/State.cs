namespace Project.Scripts.Utils.Patterns.StateMachine
{
    public interface State
    {
        void OnEnter(State from = null);
        void OnExit(State to);
    }

    public abstract class AbstractState : State
    {
        public virtual void OnEnter(State from = null) {}

        public virtual void OnExit(State to) {}
    }

    public abstract class TransitionState: AbstractState
    {
        private State _nextState;

        public State NextState => _nextState;

        public TransitionState(State nextState)
        {
            _nextState = nextState;
        }
    }
}
