using UnityEngine;

namespace ClashingArmies.Effects
{
    [CreateAssetMenu(menuName = "Clashing Armies/Sound Effect Data", fileName = "Sound Effect Data", order = 4)]
    public class SoundEffectData : EffectData
    {
        public AudioClip clip;
        [Range(0f, 1f)] public float volume = 1f;
        public bool randomizePitch;
        [Range(0.5f, 2f)] public float minPitch = 0.9f;
        [Range(0.5f, 2f)] public float maxPitch = 1.1f;
    }
}