using UnityEngine;
using Random = UnityEngine.Random;

namespace ClashingArmies
{
    public class CombatAnimation : MonoBehaviour
    {
        public float rotationAmplitude = 5f;
        public float rotationSpeed = 2f;

        private Quaternion _initialRotation;
        private float _time;

        void Start()
        {
            _initialRotation = transform.localRotation;
        }

        private void OnEnable()
        {
            _time = Random.Range(-1f, 0f);
        }

        private void OnDisable()
        {
            transform.localRotation = _initialRotation;
        }

        void Update()
        {
            _time += Time.deltaTime * rotationSpeed;

            float x = Mathf.Sin(_time) * rotationAmplitude;
            float z = Mathf.Sin(_time * 1.3f) * rotationAmplitude * 0.8f;

            Quaternion targetRotation = Quaternion.Euler(x, 0f, z);
            transform.localRotation = _initialRotation * targetRotation;
        }
    }

}