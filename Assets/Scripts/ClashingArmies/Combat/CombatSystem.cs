using ClashingArmies.Units;
using UnityEngine;

namespace ClashingArmies.Combat
{
    public class CombatSystem : MonoBehaviour
    {
        private CombatHierarchy _combatHierarchy;
        private UnitController _controller;
        private CombatDetector _combatDetector;
        private CombatResolver _combatResolver;
        
        public void Initialize(UnitController controller, CombatHierarchy combatHierarchy, UnitsManager unitsManager)
        {
            _controller = controller;
            _combatHierarchy = combatHierarchy;
            _combatDetector = new CombatDetector(transform, _controller.Unit.data.layer, _controller.Unit.data.detectionRadius, unitsManager);
            
            if (_combatHierarchy == null)
            {
                Debug.LogError("Combat Hierarchy not assigned! Please assign it in the inspector.");
                return;
            }
            
            _combatResolver = new CombatResolver(_combatHierarchy);
        }
        
        private void Update()
        {
            if (_combatDetector == null) return;
            
            Unit enemy = _combatDetector.FindNearestEnemy();
            if (enemy == null) return;
            
            TryEngageCombat(enemy);
        }
        
        private void TryEngageCombat(Unit enemy)
        {
            var enemyController = enemy.controller;
            var enemyCombatSystem = enemyController.combatSystem;
            
            if (!enemyCombatSystem) return;
            
            CombatResult result = _combatResolver.ResolveCombat(_controller.Unit, enemyController.Unit);
            ApplyCombatResult(result);
        }
        
        private void ApplyCombatResult(CombatResult result)
        {
            int winnerStrength = _combatHierarchy.GetStrength(result.Winner.data.unitType);
            int loserStrength = _combatHierarchy.GetStrength(result.Loser.data.unitType);
            
            Debug.Log($"<color=yellow>⚔️ COMBAT RESOLVED ⚔️</color>\n" +
                      $"Winner: [{result.Winner.data.unitType}] (Strength: {winnerStrength}, Health: {result.Winner.health.CurrentHealth:F0})\n" +
                      $"Loser: [{result.Loser.data.unitType}] (Strength: {loserStrength}, Dies)\n" +
                      $"Random Factor: {result.WasRandomFactor}");
            
            result.Winner.health.TakeDamage(result.DamageDealt);
            result.Loser.health.TakeDamage(result.Loser.health.MaxHealth);
            
            // Efeitos visuais
            //SpawnHitEffect(result.Winner.Transform.position);
            //SpawnDeathEffect(result.Loser.Transform.position);
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            Gizmos.DrawWireSphere(transform.position, _controller.Unit.data.detectionRadius);
        }
    }
#endif
}