using System.Collections;
using UnityEngine;

namespace ClashingArmies.Effects
{
    public class EffectsService
    {
        private readonly PoolingSystem _poolingSystem;
        private readonly MonoBehaviour _coroutineRunner;
        
        public EffectsService(PoolingSystem poolingSystem, MonoBehaviour coroutineRunner)
        {
            _poolingSystem = poolingSystem;
            _coroutineRunner = coroutineRunner;
        }
        
        public void PlayEffect(VisualEffectData effectData, Vector3 position)
        {
            GameObject vfx = _poolingSystem.SpawnFromPool(effectData.poolType, position, null);
            _coroutineRunner.StartCoroutine(ReturnToPoolAfterDelay(effectData, vfx));
        }
        
        public void PlayEffect(SoundEffectData sfx, Vector3 position)
        {
            if (sfx.clip == null || _poolingSystem == null) return;
            
            GameObject audioSourceGO = _poolingSystem.SpawnFromPool(PoolingSystem.PoolType.AudioSource, position, null);
            AudioSource source = audioSourceGO.GetComponent<AudioSource>();
            source.clip = sfx.clip;
            source.volume = sfx.volume;
            source.pitch = sfx.randomizePitch ? Random.Range(sfx.minPitch, sfx.maxPitch) : 1f;
            source.Play();
            
            _coroutineRunner.StartCoroutine(ReturnToPoolAfterDelay(sfx, audioSourceGO));
        }
        
        private IEnumerator ReturnToPoolAfterDelay(EffectData effect, GameObject obj)
        {
            yield return new WaitForSeconds(effect.duration);
            _poolingSystem.ReturnToPool(effect.poolType, obj);
        }
    }
}