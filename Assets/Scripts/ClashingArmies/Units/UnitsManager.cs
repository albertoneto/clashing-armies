using System.Collections.Generic;
using UnityEngine;

namespace ClashingArmies.Units
{
    public class Unit
    {
        public UnitsManager.UnitType UnitType;
    }
    public class UnitsManager : MonoBehaviour
    {
        public enum UnitType{Red, Green, Blue, Yellow}
        
        private readonly List<Unit> _units = new();

        public void AddUnit(Unit unit)
        {
            _units.Add(unit);
        }

        public int GetUnitCount()
        {
            return _units.Count;
        }

        public Unit GetLastUnit()
        {
            return _units.Count > 0 ? _units[^1] : null;
        }

        public List<Unit> GetAllUnits()
        {
            return _units;
        }
    }
}
