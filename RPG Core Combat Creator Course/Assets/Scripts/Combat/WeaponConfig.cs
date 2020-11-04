﻿using System;
using RPG.Attributes;
using UnityEngine;

namespace RPG.Combat
{
    [CreateAssetMenu(fileName = "WeaponConfig", menuName = "Weapons/New Weapon Config", order = 0)]
    public class WeaponConfig : ScriptableObject
    {
        [SerializeField] public Weapon equippedPrefab;
        [SerializeField] public AnimatorOverrideController animatorOverride;
        [SerializeField] public float weaponDamage = 5f;
        [SerializeField] public float weaponPercentageBonus = 0;
        [SerializeField] public float weaponRange = 2.0f;
        [SerializeField] public bool isRightHanded = true;
        [SerializeField] public Projectile ProjectilePrefab = null;
        [SerializeField] public float timeBetweenAttacks = 1f;
        [SerializeField] public GameObject hitEffectPrefab = null;

        const string weaponName = "Weapon";

        public Weapon Spawn(Transform rightHand, Transform leftHand, Animator animator)
        {
            DestroyOldWeapon(rightHand, leftHand);

            Weapon weapon = null;

            if (equippedPrefab != null)
            {
                Transform handTransform = GetTransform(rightHand, leftHand);
                weapon = Instantiate(equippedPrefab, handTransform);
                weapon.name = weaponName;
            }

            var overrideController = animator.runtimeAnimatorController as AnimatorOverrideController;
            if (animatorOverride != null)
            {
                animator.runtimeAnimatorController = animatorOverride;
            }
            else if (overrideController != null)
            {
                animator.runtimeAnimatorController = overrideController.runtimeAnimatorController;
            }

            return weapon;
        }

        private void DestroyOldWeapon(Transform rightHand, Transform leftHand)
        {
            Transform oldWeapon = rightHand.Find(weaponName);
            if (oldWeapon == null)
            {
                oldWeapon = leftHand.Find(weaponName);
            }
            if (oldWeapon == null) return;

            oldWeapon.name = "DESTROYING";
            Destroy(oldWeapon.gameObject);
        }

        private Transform GetTransform(Transform rightHand, Transform leftHand)
        {
            Transform handTransform;
            if (isRightHanded) handTransform = rightHand;
            else handTransform = leftHand;
            return handTransform;
        }

        public bool HasProjectile()
        {
            return ProjectilePrefab != null;
        }

        public void LaunchProjectile(Transform rightHand, Transform leftHand, Health target, GameObject instigator, float damage)
        {
            Projectile projectileInstance = Instantiate(ProjectilePrefab, GetTransform(rightHand, leftHand).position, Quaternion.identity);
            projectileInstance.SetTarget(target, instigator, damage);
            projectileInstance.SetupHitEffects(hitEffectPrefab);
        }

        public float GetDamage { get { return weaponDamage; } }
        public float GetRange { get { return weaponRange; } }
        public float GetPercentageDamageBonus { get { return weaponPercentageBonus; } }
    }
}