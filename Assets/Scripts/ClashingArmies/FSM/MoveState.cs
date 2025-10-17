namespace ClashingArmies
{
    public abstract class MoveState : IState
    {
        private readonly string _stateName;

        protected MoveState(string name)
        {
            _stateName = name;
        }

        public virtual void OnEnter() { }
        public virtual void OnExit() { }
        public virtual void OnUpdate() { }
        public virtual void OnFixedUpdate() { }
        public string GetStateName() => _stateName;
    }
}