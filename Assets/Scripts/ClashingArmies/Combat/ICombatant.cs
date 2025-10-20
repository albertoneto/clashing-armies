using ClashingArmies.Units;

namespace ClashingArmies.Combat
{
    public interface ICombatant : IDamageable
    {
        UnitType UnitType { get; }
        UnitController Controller { get; }
        int CombatLayer { get; }
        float DetectionRadius { get; }
        
        bool CanEngageCombat();
        void OnCombatVictory();
        void OnCombatDefeat();
        
        
    }
}