namespace ClashingArmies.Combat
{
    public interface ICombatResolver
    {
        CombatResult ResolveCombat(ICombatant combatant1, ICombatant combatant2);
    }
}