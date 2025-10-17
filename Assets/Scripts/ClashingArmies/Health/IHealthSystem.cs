namespace ClashingArmies.Health
{
    public interface IHealthSystem
    {
        float CurrentHealth { get; }
        float MaxHealth { get; }
        bool IsDead { get; }
    
        void TakeDamage(float amount);
        void Heal(float amount);
        void SetMaxHealth(float newMax);
    }
}