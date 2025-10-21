using System;
using UnityEngine;

namespace ClashingArmies
{
    public class RotateAnimation : MonoBehaviour
    {
        [SerializeField] private Vector3 rotationAxis = Vector3.up;
        [SerializeField] private float rotationSpeed = 90f;
        [SerializeField] private bool useUnscaledTime;
        
        private Quaternion _originalRotation;

        private void Update()
        {
            float deltaTime = useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
            transform.Rotate(rotationAxis, rotationSpeed * deltaTime, Space.Self);
        }
    }
}
