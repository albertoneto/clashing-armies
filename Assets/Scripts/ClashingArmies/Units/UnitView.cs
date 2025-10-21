using System.Collections.Generic;
using ClashingArmies.Effects;
using UnityEngine;

namespace ClashingArmies.Units
{
    public class UnitView
    {
        private readonly EffectsService _effectsService;
        private readonly Unit _unit;
        private readonly List<RotateAnimation> _rotateAnimations = new();

        public UnitView(Unit unit, PoolingSystem poolingSystem)
        {
            _unit = unit;
            
            var renderer = _unit.GameObject.GetComponentInChildren<MeshRenderer>();
            renderer.material = _unit.data.material;

            _rotateAnimations.AddRange(_unit.GameObject.GetComponents<RotateAnimation>());
            
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

        public void ToggleRotation()
        {
            foreach (var rotate in _rotateAnimations)
            {
                rotate.enabled = !rotate.enabled;
            }
            _unit.GameObject.transform.localRotation = Quaternion.identity;
        }
    }
}