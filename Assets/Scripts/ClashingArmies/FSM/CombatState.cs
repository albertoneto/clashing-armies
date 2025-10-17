using ClashingArmies.Units;
using UnityEngine;

namespace ClashingArmies
{
    public class CombatState : IState
    {
        public CombatState()
        {
            
        }

        public void OnEnter()
        {
        /*
            var targetHealth = other.gameObject.GetComponent<HealthSystem>();
            if (targetHealth != null && !targetHealth.IsDead)
            {
                targetHealth.TakeDamage(_unit.data.Damage);
            }
            */
        }
        public void OnExit() { }

        public void OnUpdate()
        {
            Debug.Log("CombatState.OnUpdate()");
        }
        public void OnFixedUpdate() { }
    }
}