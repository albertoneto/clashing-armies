using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClashingArmies
{
    public class StateMachine : MonoBehaviour
    {
        private IState _currentState;
        private bool _isTransitioning;

        private readonly Dictionary<Type, IState> _states = new();

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

        private void OnDestroy()
        {
            _currentState?.OnExit();
            _currentState = null;
            _states.Clear();
        }
        
        public void AddState<T>(T state) where T : IState
        {
            var type = typeof(T);
            
            if (_states.ContainsKey(type))
            {
                Debug.LogWarning($"State already exists: {type.Name}");
                return;
            }

            _states.Add(type, state);
        }

        public void SetState<T>() where T : IState
        {
            var type = typeof(T);

            if (IsInState<T>())
            {
                Debug.Log($"Already in state: {type.Name}");
                return;
            }

            if (_isTransitioning)
            {
                Debug.LogWarning($"Transition in progress. Can't set state: {type.Name}");
                return;
            }

            if (!HasState<T>())
            {
                Debug.LogError($"State not found: {type.Name}. Did you forget to add it?");
                return;
            }

            _isTransitioning = true;
            _currentState?.OnExit();
            _currentState = _states[type];
            _currentState.OnEnter();
            _isTransitioning = false;
        }
        
        public bool IsInState<T>() where T : IState
        {
            return _currentState != null && _currentState.GetType() == typeof(T);
        }

        public bool HasState<T>() where T : IState
        {
            return _states.ContainsKey(typeof(T));
        }

        public T GetState<T>() where T : IState
        {
            var type = typeof(T);
            if (_states.TryGetValue(type, out var state))
            {
                return (T)state;
            }
            return default;
        }
    }
}