using UnityEngine;

namespace ClashingArmies.Units
{
    [CreateAssetMenu(menuName = "Create Unit", fileName = "Unit", order = 0)]
    public class UnitData : ScriptableObject
    {
        public enum InitialStateType
        {
            Patrol,
            Randomly
        }
        public UnitType UnitType;
        public InitialStateType InitialState = InitialStateType.Randomly;
        public Material Material;
        public float Speed = 2f;
        public float Damage = 2f;
        public float Health = 10f;
        public float ChangeTargetTime = 5f;
        public Vector3 RandomOffset = new Vector3(10f, 10f, 10f);
        public Vector3[] Waypoints;
        public LayerMask layer;
    }
}