using ClashingArmies.Units;

namespace ClashingArmies.Combat
{
    public interface ICombatant : IDamageable
    {
        UnitType UnitType { get; }
        int CombatLayer { get; }
        float DetectionRadius { get; }
        
        bool CanEngageCombat();
        void OnCombatVictory();
        void OnCombatDefeat();
    }
}