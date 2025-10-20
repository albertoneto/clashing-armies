using ClashingArmies.Units;
using UnityEngine;

namespace ClashingArmies.Combat
{
    public class CombatResolver : ICombatResolver
    {
        private readonly CombatHierarchy _hierarchy;
        
        public CombatResolver(CombatHierarchy hierarchy)
        {
            _hierarchy = hierarchy;
        }
        
        public CombatResult ResolveCombat(ICombatant unit1, ICombatant unit2)
        {
            UnitType strongerType = _hierarchy.GetStrongerUnit(unit1.UnitType, unit2.UnitType);
            ICombatant stronger = unit1.UnitType == strongerType ? unit1 : unit2;
            ICombatant weaker = stronger == unit1 ? unit2 : unit1;
            
            bool randomWin = Random.value < _hierarchy.randomWinChance;
            ICombatant winner = randomWin ? weaker : stronger;
            ICombatant loser = randomWin ? stronger : weaker;
            
            float damageToWinner = winner.MaxHealth * _hierarchy.winnerDamagePercent / 100;
            return new CombatResult(winner, loser, damageToWinner);
        }
    }
}