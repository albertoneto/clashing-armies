using System.Collections.Generic;
using UnityEngine;

namespace ClashingArmies
{
    public class StateMachine : MonoBehaviour
    {
        private IState _currentState;
        private bool _isTransitioning;

        private readonly Dictionary<string, IState> _states = new();

        private void Update()
        {
            if (_currentState == null || _isTransitioning) return;

            _currentState.OnUpdate();
        }

        private void FixedUpdate()
        {
            if (_currentState == null || _isTransitioning) return;

            _currentState.OnFixedUpdate();
        }

        public void AddState(IState state)
        {
            if (!_states.ContainsKey(state.GetStateName()))
            {
                _states.Add(state.GetStateName(), state);
            }
        }

        public void SetState(string stateName)
        {
            if (_currentState != null && _currentState.GetStateName() == stateName)
            {
                Debug.Log("Already in state: " + stateName);
                return;
            }

            if (_isTransitioning)
            {
                Debug.Log("Transition in progress. Can't set state: " + stateName);
                return;
            }

            if (!_states.ContainsKey(stateName))
            {
                Debug.LogWarning("State not found: " + stateName);
                return;
            }

            _isTransitioning = true;
            _currentState?.OnExit();
            _currentState = _states[stateName];
            _currentState.OnEnter();
            _isTransitioning = false;
        }
    }
}