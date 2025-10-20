namespace ClashingArmies.Combat
{
    public interface IEnemyDetector
    {
        ICombatant FindFirst();
        bool InRange();
    }
}