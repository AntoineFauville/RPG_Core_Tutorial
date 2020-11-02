using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace RPG.UI.DamageText
{
    public class DamageTextSpawner : MonoBehaviour
    {
        [SerializeField] private DamageText _damageTextPrefab;
        [SerializeField] private Transform _spawnTransform;

        public void Spawn(float damageAmount)
        {
            DamageText damageText = Instantiate(_damageTextPrefab, _spawnTransform);
            damageText.GetComponent<DamageText>().SetValue(damageAmount);
        }
    }
}
