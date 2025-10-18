using ClashingArmies.Units;
using UnityEngine;

namespace ClashingArmies.Combat
{
    public class CombatResolver
    {
        private readonly CombatHierarchy _hierarchy;
        
        public CombatResolver(CombatHierarchy hierarchy)
        {
            _hierarchy = hierarchy;
        }
        
        public CombatResult ResolveCombat(Unit unit1, Unit unit2)
        {
            UnitType strongerType = _hierarchy.GetStrongerUnit(unit1.data.unitType, unit2.data.unitType);
            Unit stronger = unit1.data.unitType == strongerType ? unit1 : unit2;
            Unit weaker = stronger == unit1 ? unit2 : unit1;
            
            bool randomWin = Random.value < _hierarchy.randomWinChance / 100;
            Unit winner = randomWin ? weaker : stronger;
            Unit loser = randomWin ? stronger : weaker;
            
            float damageToWinner = winner.health.MaxHealth * _hierarchy.winnerDamagePercent;
            return new CombatResult(winner, loser, damageToWinner);
        }
    }
}