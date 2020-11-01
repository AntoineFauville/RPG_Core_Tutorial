using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace RPG.Resources
{
    public class ExperienceDisplay : MonoBehaviour
    {
        Experience Experience;

        [SerializeField] private Text experienceText;

        private void Awake()
        {
            Experience = GameObject.FindWithTag("Player").GetComponent<Experience>();
        }

        private void Update()
        {
            experienceText.text = Experience.GetPoints().ToString();
        }
    }
}
