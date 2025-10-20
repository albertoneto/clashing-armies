using UnityEngine;
using UnityEngine.UI;

namespace ClashingArmies.Health
{
    [RequireComponent(typeof(SpriteRenderer))]
    public class HealthBarUI : MonoBehaviour
    {
        private SpriteRenderer _spriteRenderer;
        private HealthSystem _healthSystem;
        private Camera _mainCamera;
        private Vector3 _initialScale;
    
        public void Initialize(HealthSystem health)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            _mainCamera = Camera.main;
            _healthSystem = health;
            _healthSystem.OnHealthChanged += UpdateDisplay;
            
            _initialScale = transform.localScale;
            UpdateDisplay(health.CurrentHealth, health.MaxHealth);
        }
        
        private void LateUpdate()
        {
            if (_mainCamera == null) return;
            
            transform.LookAt(transform.position + _mainCamera.transform.forward);
        }
    
        private void OnDisable()
        {
            if(_healthSystem == null) return;
            _healthSystem.OnHealthChanged -= UpdateDisplay;
        }
    
        private void UpdateDisplay(float current, float max)
        {
            if (_spriteRenderer == null || !(max > 0)) return;
            
            float ratio = Mathf.Clamp01(current / max);
            Vector3 scale = _initialScale;
            scale.x = _initialScale.x * ratio;
            transform.localScale = scale;
        }
    }
}