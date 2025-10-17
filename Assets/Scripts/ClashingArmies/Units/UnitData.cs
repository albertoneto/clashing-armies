using UnityEngine;

namespace ClashingArmies.Units
{
    [CreateAssetMenu(menuName = "Create Unit", fileName = "Unit", order = 0)]
    public class UnitData : ScriptableObject
    {
        public UnitType UnitType;
        public Material Material;
        public float Speed = 2f;
        public float ChangeTargetTime = 5f;
    }
}