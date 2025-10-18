using UnityEngine;

namespace ClashingArmies.Units
{
    [CreateAssetMenu(menuName = "Clashing Armies/Create Unit", fileName = "Unit", order = 0)]
    public class UnitData : ScriptableObject
    {
        public enum InitialStateType
        {
            Patrol,
            Randomly
        }
        
        [Header("Main")]
        public LayerMask layer;
        public UnitType unitType;
        public Material material;
        
        [Header("Movement")]
        public InitialStateType initialState = InitialStateType.Randomly;
        public float speed = 2f;
        public float changeTargetTime = 5f;
        public Vector3 randomOffset = new Vector3(10f, 10f, 10f);
        public Vector3[] waypoints;
        
        [Header("Combat")]        
        public float detectionRadius = 2f;
        
        [Header("Health")]
        public float maxHealth = 100f;
    }
}