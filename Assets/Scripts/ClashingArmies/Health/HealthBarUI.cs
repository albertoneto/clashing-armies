using UnityEngine;
using UnityEngine.UI;

namespace ClashingArmies.Health
{
    public class HealthBarUI : MonoBehaviour
    {
        [SerializeField] private Image fillImage;
        [SerializeField] private Text healthText;
    
        private IHealthSystem healthSystem;
    
        public void Initialize(IHealthSystem health)
        {
            healthSystem = health;
        
            if (health is HealthLogic logic)
            {
                logic.OnHealthChanged += UpdateDisplay;
                UpdateDisplay(health.CurrentHealth, health.MaxHealth);
            }
        }
    
        private void UpdateDisplay(float current, float max)
        {
            if (fillImage != null)
            {
                fillImage.fillAmount = current / max;
            }
        
            if (healthText != null)
            {
                healthText.text = $"{current:F0}/{max:F0}";
            }
        }
    
        private void OnDestroy()
        {
            if (healthSystem is HealthLogic logic)
            {
                logic.OnHealthChanged -= UpdateDisplay;
            }
        }
    }
}