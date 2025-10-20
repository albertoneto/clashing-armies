namespace ClashingArmies.Combat
{
    public class CombatResult
    {
        public ICombatant Winner { get; }
        public ICombatant Loser { get; }
        public float DamageToWinner { get; }
        
        public CombatResult(ICombatant winner, ICombatant loser, float damage)
        {
            Winner = winner;
            Loser = loser;
            DamageToWinner = damage;
        }

        public void Apply()
        {
            Winner.TakeDamage(DamageToWinner);
            Winner.OnCombatVictory();
            
            Loser.TakeDamage(Loser.MaxHealth);
            Loser.OnCombatDefeat();
        }
    }
}