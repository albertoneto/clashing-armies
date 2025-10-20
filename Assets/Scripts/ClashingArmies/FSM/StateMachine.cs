using System;
using System.Collections.Generic;
using UnityEngine;

namespace ClashingArmies
{
    public class StateMachine : MonoBehaviour
    {
        public IState CurrentState { get; private set; }
        private bool _isTransitioning;

        private readonly Dictionary<Type, IState> _states = new();

        private void Update()
        {
            if (CurrentState == null || _isTransitioning) return;

            CurrentState.OnUpdate();
        }

        private void FixedUpdate()
        {
            if (CurrentState == null || _isTransitioning) return;

            CurrentState.OnFixedUpdate();
        }

        private void OnDestroy()
        {
            CurrentState?.OnExit();
            CurrentState = null;
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
            CurrentState?.OnExit();
            CurrentState = _states[type];
            CurrentState.OnEnter();
            _isTransitioning = false;
        }

        private bool IsInState<T>() where T : IState
        {
            return CurrentState != null && CurrentState.GetType() == typeof(T);
        }

        private bool HasState<T>() where T : IState
        {
            return _states.ContainsKey(typeof(T));
        }
    }
}