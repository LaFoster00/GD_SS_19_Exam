namespace StateSystem
{
    public class StateMachine
    {
        private IState _current;
        private IState _previous;

        public void ChangeState(IState newState)
        {
            if (_current != null)
            {
                _current.Exit();
            }

            _previous = _current;
            _current = newState;
            _current.Enter();
        }

        public void ExecuteState()
        {
            if (_current != null)
            {
                _current.Execute();
            }
        }


        public void SwitchToPreviousState()
        {
            if (_current != null)
            {
                _current.Exit();
            }

            _current = _previous;
            _current.Enter();
        }
    }
}



