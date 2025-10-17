using ClashingArmies.Units;

namespace ClashingArmies
{
    public class CombatState : IState
    {
        private readonly string _stateName;

        public CombatState(string name, Unit unit)
        {
            _stateName = name;
        }

        public void OnEnter() { }
        public void OnExit() { }
        public void OnUpdate() { }
        public void OnFixedUpdate() { }
        public string GetStateName() => _stateName;
    }
}