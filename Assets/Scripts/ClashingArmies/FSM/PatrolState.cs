using ClashingArmies.Units;

namespace ClashingArmies
{
    public class PatrolState : MoveState
    {
        public PatrolState(string name, Unit unit) : base(name) { }

        public override void OnUpdate()
        {
            // lógica de patrulha
        }
    }
}