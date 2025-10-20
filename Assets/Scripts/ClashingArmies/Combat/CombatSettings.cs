using System.Collections.Generic;
using System.Linq;
using ClashingArmies.Units;
using UnityEngine;

namespace ClashingArmies.Combat
{
    [CreateAssetMenu(menuName = "Clashing Armies/Combat Hierarchy", fileName = "CombatHierarchy", order = 1)]
    public class CombatSettings : ScriptableObject
    {
        [Range(0f, 1f)] public float combatCooldown = 0.5f;
        [Range(0f, 10f)] public float combatDuration = 1f;
        
        [System.Serializable]
        public class UnitStrength
        {
            public UnitType unitType;
            [Range(1, 10)]
            [Tooltip("Higher value means stronger unit")]
            public int StrengthLevel = 1;
        }
        
        [Header("Unit Strength")]
        public List<UnitStrength> unitStrengths = new();
        
        [Header("Combat Rules")]
        [Range(0f, 1f)]
        [Tooltip("Chance for the weaker unit to win (0 = never, 1 = always)")]
        public float randomWinChance = 0.2f;

        [Range(0f, 100f)] [Tooltip("Percentage of health the winner loses")]
        public float winnerDamagePercent = 0.1f;
        
        public int GetStrength(UnitType unitType)
        {
            var strength = unitStrengths.FirstOrDefault(s => s.unitType == unitType);
            return strength?.StrengthLevel ?? 1;
        }
        
        public UnitType GetStrongerUnit(UnitType unit1, UnitType unit2)
        {
            int strength1 = GetStrength(unit1);
            int strength2 = GetStrength(unit2);
            
            return strength1 >= strength2 ? unit1 : unit2;
        }
    }
}