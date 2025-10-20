using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClashingArmies.Units
{
    public class UnitsManager : MonoBehaviour
    {
        public static event Action<int> OnUnitsChanged;
        private readonly Dictionary<GameObject, Unit> _units = new();
        private Unit _lastUnit;

        public void AddUnit(Unit unit)
        {
            _units.Add(unit.UnitObject, unit);
            _lastUnit = unit;
            OnUnitsChanged?.Invoke(_units.Count);
        }
        
        public void RemoveUnit(Unit unit)
        {
            _units.Remove(unit.UnitObject);
            unit.Dispose();
            OnUnitsChanged?.Invoke(_units.Count);
        }

        public int GetUnitCount()
        {
            return _units.Count;
        }

        public Unit GetLastUnit()
        {
            return _lastUnit;
        }

        public Unit GetUnit(GameObject hitObject)
        {
            _units.TryGetValue(hitObject, out var unit);
            return unit;
        }
    }
}
