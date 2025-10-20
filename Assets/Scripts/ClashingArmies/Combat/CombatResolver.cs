using ClashingArmies.Units;
using UnityEngine;

namespace ClashingArmies.Combat
{
    public class CombatResolver : ICombatResolver
    {
        private readonly CombatSettings _settings;
        
        public CombatResolver(CombatSettings settings)
        {
            _settings = settings;
        }
        
        public CombatResult ResolveCombat(ICombatant unit1, ICombatant unit2)
        {
            UnitType strongerType = _settings.GetStrongerUnit(unit1.UnitType, unit2.UnitType);
            ICombatant stronger = unit1.UnitType == strongerType ? unit1 : unit2;
            ICombatant weaker = stronger == unit1 ? unit2 : unit1;
            
            bool randomWin = Random.value < _settings.randomWinChance;
            ICombatant winner = randomWin ? weaker : stronger;
            ICombatant loser = randomWin ? stronger : weaker;
            
            float damageToWinner = winner.MaxHealth * _settings.winnerDamagePercent / 100;
            return new CombatResult(winner, loser, damageToWinner);
        }
    }
}