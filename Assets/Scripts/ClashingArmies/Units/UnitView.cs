using ClashingArmies.Effects;
using UnityEngine;

namespace ClashingArmies.Units
{
    public class UnitView
    {
        private readonly EffectsService _effectsService;
        private readonly Unit _unit;
        private readonly CombatAnimation _combatAnimation;

        public UnitView(Unit unit, PoolingSystem poolingSystem)
        {
            _unit = unit;
            
            MeshRenderer[] renderers = _unit.GameObject.GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer renderer in renderers)
            {
                renderer.material = _unit.data.material;
            }

            _combatAnimation = _unit.GameObject.GetComponent<CombatAnimation>();
            
            _effectsService = new EffectsService(poolingSystem, poolingSystem);
            _unit.health.OnDeath += DeathEffect;

            SpawnEffect();
        }

        private void SpawnEffect()
        {
            if(_effectsService == null) return;
            
            _effectsService.PlayEffect(_unit.data.spawnEffect, _unit.UnitObject.transform.position);
            _effectsService.PlayEffect(_unit.data.spawnSfxEffect, _unit.UnitObject.transform.position);
        }

        private void DeathEffect()
        {
            if(_effectsService == null) return;

            _effectsService.PlayEffect(_unit.data.deathEffect, _unit.UnitObject.transform.position);
            _effectsService.PlayEffect(_unit.data.deathSfxEffect, _unit.UnitObject.transform.position);
            
            _unit.health.OnDeath -= DeathEffect;
        }

        public void SetCombatRotation(bool value)
        {
            if(_combatAnimation == null) return;
            
            _combatAnimation.enabled = value;
        }
    }
}