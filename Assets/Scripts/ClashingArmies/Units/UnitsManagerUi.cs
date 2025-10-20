using UnityEngine;
using TMPro;

namespace ClashingArmies.Units
{
    public class UnitsManagerUi : MonoBehaviour
    {
        [SerializeField] private TMP_Text amountText;
        private void OnEnable()
        {
            UnitsManager.OnUnitsChanged += UpdateUnitsAmountUi;
        }
        
        private void OnDisable()
        {
            UnitsManager.OnUnitsChanged -= UpdateUnitsAmountUi;
        }

        private void UpdateUnitsAmountUi(int amout)
        {
            amountText.text = amout.ToString();
        }
    }
}