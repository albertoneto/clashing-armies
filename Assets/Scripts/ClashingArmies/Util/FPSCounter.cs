using UnityEngine;
using TMPro;

namespace ClashingArmies
{
    public class FPSCounter : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI fpsText;
        [SerializeField] private float refreshFrequency = 0.2f;
        
        private float _currentFPS;
        private float _timeSinceUpdate;
        
        private void Update()
        {
            _currentFPS = 1f / Time.unscaledDeltaTime;
            
            _timeSinceUpdate += Time.unscaledDeltaTime;
            if (!(_timeSinceUpdate >= refreshFrequency)) return;
            
            fpsText.text = Mathf.RoundToInt(_currentFPS).ToString();
            _timeSinceUpdate = 0f;
        }
    }
}