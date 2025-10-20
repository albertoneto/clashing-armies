namespace ClashingArmies.Combat
{
    public interface IEnemyDetector
    {
        ICombatant FindNearestEnemy();
        bool HasEnemyInRange();
    }
}