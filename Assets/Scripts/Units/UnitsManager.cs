using System.Collections.Generic;
using UnityEngine;

namespace ClashingArmies.Units
{
    public class Unit
    {
        public UnitsManager.UnitType type;
    }
    public class UnitsManager : MonoBehaviour
    {
        public enum UnitType{Red, Green, Blue, Yellow}
        
        private List<Unit> _units = new ();

        public void AddUnit(Unit unit)
        {
            _units.Add(unit);
        }
    }
}
