using UnityEngine;
using UnityEngine.Events;

namespace ClashingArmies.Health
{
public class HealthSystem : MonoBehaviour
{
    [Header("Configuration")]
    [SerializeField] private float maxHealth = 100f;
    [SerializeField] private bool destroyOnDeath = false;
    [SerializeField] private float destroyDelay = 2f;
    
    [Header("Visual Feedback")]
    [SerializeField] private bool showHealthBar = true;
    [SerializeField] private GameObject healthBarPrefab;
    
    private HealthLogic  healthLogic;
    private HealthBarUI healthBarUI;
    
    public IHealthSystem Health => healthLogic;
    public bool IsDead => healthLogic.IsDead;
    
    public UnityEvent<float> OnDamageEvent;
    public UnityEvent<float> OnHealEvent;
    public UnityEvent OnDeathEvent;
    
    private void Awake()
    {
        healthLogic = new HealthLogic(maxHealth);
        
        healthLogic.OnDamageTaken += HandleDamageTaken;
        healthLogic.OnHealed += HandleHealed;
        healthLogic.OnDeath += HandleDeath;
        healthLogic.OnHealthChanged += HandleHealthChanged;
        
        if (showHealthBar && healthBarPrefab != null)
        {
            GameObject bar = Instantiate(healthBarPrefab, transform);
            healthBarUI = bar.GetComponent<HealthBarUI>();
            healthBarUI?.Initialize(healthLogic);
        }
    }
    
    private void HandleDamageTaken(float damage)
    {
        OnDamageEvent?.Invoke(damage);
    }
    
    private void HandleHealed(float amount)
    {
        OnHealEvent?.Invoke(amount);
    }
    
    private void HandleDeath()
    {
        OnDeathEvent?.Invoke();
        
        if (destroyOnDeath)
        {
            Destroy(gameObject, destroyDelay);
        }
    }
    
    private void HandleHealthChanged(float current, float max)
    {
        
    }
    
    private void OnDestroy()
    {
        if (healthLogic != null)
        {
            healthLogic.OnDamageTaken -= HandleDamageTaken;
            healthLogic.OnHealed -= HandleHealed;
            healthLogic.OnDeath -= HandleDeath;
            healthLogic.OnHealthChanged -= HandleHealthChanged;
        }
    }
    
    public void TakeDamage(float amount)
    {
        healthLogic.TakeDamage(amount);
    }
    
    public void Heal(float amount)
    {
        healthLogic.Heal(amount);
    }
    /*
    public float GetHealthPercentage()
    {
        return healthLogic.GetHealthPercentage();
    }
    */
}
}