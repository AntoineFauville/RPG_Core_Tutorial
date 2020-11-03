using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.UI.DamageText
{
    public class DamageText : MonoBehaviour
    {
        [SerializeField] private Text damageTextAmount = null;

        public void SetValue(float amount)
        {
            damageTextAmount.text = amount.ToString();
        }
    }
}