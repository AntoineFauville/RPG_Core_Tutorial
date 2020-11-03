using System;
using System.Collections.Generic;
using GameDevTV.Utils;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Stats
{
    public class BaseStats : MonoBehaviour
    {
        [Range(1, 99)]
        [SerializeField] private int startingLevel = 1;
        [SerializeField] CharacterClass characterClass;
        [SerializeField] private Progression progression;
        [SerializeField] private GameObject levelUpParticleEffect;
        [SerializeField] private bool shouldUseModifiers = false;

        public event Action onLevelUp;

        LazyValue<int> currentLevel;
        
        private void Awake()
        {
            currentLevel = new LazyValue<int>(CalculateLevel);
        }

        private void Start()
        {
            currentLevel.ForceInit();
        }

        private void OnEnable()
        {
            if (GetComponent<Experience>() != null)
            {
                GetComponent<Experience>().onExperienceGained += UpdateLevel;
            }
        }

        private void OnDisable()
        {
            if (GetComponent<Experience>() != null)
            {
                GetComponent<Experience>().onExperienceGained -= UpdateLevel;
            }
        }

        private void UpdateLevel()
        {
            int newLevel = CalculateLevel();
            if (newLevel > currentLevel.value)
            {
                currentLevel.value = newLevel;
                LevelUpEffect();
                onLevelUp();
            }
        }

        private void LevelUpEffect()
        {
            Instantiate(levelUpParticleEffect, transform);
        }

        public float GetStat(Stat stat)
        {
            return (GetBaseStat(stat) + GetAdditiveModifier(stat)) * (1 + GetPercentageModifier(stat) / 100);
        }
        
        public int GetLevel()
        {
            return currentLevel.value;
        }

        private int CalculateLevel()
        {
            if (GetComponent<Experience>() == null) return startingLevel;

            float currentExperience = GetComponent<Experience>().GetPoints();

            int penultimateLevel = progression.GetLevels(Stat.ExperienceToLevelUp, characterClass);
            for (int level = 1; level < penultimateLevel; level++)
            {
                float XPToLevelUp = progression.GetStat(Stat.ExperienceToLevelUp, characterClass, level);
                if (XPToLevelUp > currentExperience)
                {
                    return level;
                }
            }

            return penultimateLevel + 1;
        }

        private float GetAdditiveModifier(Stat stat)
        {
            if (!shouldUseModifiers) { return 0; }

            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetAdditiveModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        private float GetPercentageModifier(Stat stat)
        {
            if (!shouldUseModifiers) { return 0; }

            float total = 0;
            foreach (IModifierProvider provider in GetComponents<IModifierProvider>())
            {
                foreach (float modifier in provider.GetPercentageModifiers(stat))
                {
                    total += modifier;
                }
            }
            return total;
        }

        private float GetBaseStat(Stat stat)
        {
            return progression.GetStat(stat, characterClass, GetLevel());
        }
    }
}
