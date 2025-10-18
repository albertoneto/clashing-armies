using ClashingArmies.Units;

namespace ClashingArmies.Combat
{
    public class CombatResult
    {
        public Unit Winner { get; }
        public Unit Loser { get; }
        public float DamageDealt { get; }
        
        public CombatResult(Unit winner, Unit loser, float damage)
        {
            Winner = winner;
            Loser = loser;
            DamageDealt = damage;
        }
    }
}