using UnityEngine;
using UnityEngine.UI;

namespace ClashingArmies.Health
{
    [RequireComponent(typeof(Image))]
    public class HealthBarUI : MonoBehaviour
    {
        private Image _fillImage;
        private HealthSystem _healthSystem;
        private Camera _mainCamera;
    
        public void Initialize(HealthSystem health)
        {
            _fillImage = GetComponent<Image>();
            _mainCamera = Camera.main;
            _healthSystem = health;
            _healthSystem.OnHealthChanged += UpdateDisplay;
            UpdateDisplay(health.CurrentHealth, health.MaxHealth);
        }
        
        private void LateUpdate()
        {
            if (_mainCamera == null) return;
            
            transform.LookAt(transform.position + _mainCamera.transform.forward);
        }
    
        private void OnDisable()
        {
            _healthSystem.OnHealthChanged -= UpdateDisplay;
        }
    
        private void UpdateDisplay(float current, float max)
        {
            if (_fillImage != null)
            {
                _fillImage.fillAmount = current / max;
            }
        }
    }
}