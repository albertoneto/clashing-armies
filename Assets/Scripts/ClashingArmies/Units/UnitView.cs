using ClashingArmies.Effects;
using UnityEngine;

namespace ClashingArmies.Units
{
    public class UnitView
    {
        public Animator animator;
        
        private readonly EffectsService _effectsService;
        private readonly Unit _unit;

        public UnitView(Unit unit, PoolingSystem poolingSystem)
        {
            _unit = unit;
            
            animator = _unit.UnitObject.gameObject.GetComponent<Animator>();
            
            var renderer = _unit.UnitObject.gameObject.GetComponent<MeshRenderer>();
            renderer.material = _unit.data.material;
            
            _effectsService = new EffectsService(poolingSystem, poolingSystem);
            _unit.health.OnDeath += DeathEffect;

            SpawnEffect();
        }

        private void SpawnEffect()
        {
            _effectsService.PlayEffect(_unit.data.spawnEffect, _unit.UnitObject.transform.position);
            _effectsService.PlayEffect(_unit.data.spawnSfxEffect, _unit.UnitObject.transform.position);
        }

        private void DeathEffect()
        {
            _effectsService.PlayEffect(_unit.data.deathEffect, _unit.UnitObject.transform.position);
            _effectsService.PlayEffect(_unit.data.deathSfxEffect, _unit.UnitObject.transform.position);
            
            _unit.health.OnDeath -= DeathEffect;
        }
    }
}