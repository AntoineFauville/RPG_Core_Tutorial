using RPG.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Combat
{
    public class Projectile : MonoBehaviour
    {
        [SerializeField] private float speed = 1;
        [SerializeField] private bool isHoming = false;
        [SerializeField] private float maxLifeTime = 10f;
        [SerializeField] private GameObject[] destoyOnHit = null;
        [SerializeField] private float lifeAfterInpact = 2f;


        Health _target = null;
        float _damage = 0;
        GameObject _hitEffectPrefab;
        GameObject _instigator;

        private void Start()
        {
            transform.LookAt(GetAimLocation());
        }

        void Update()
        {
            if (_target == null) { return; }

            if (isHoming && !_target.IsDead()) { transform.LookAt(GetAimLocation()); }

            transform.Translate(Vector3.forward * speed * Time.deltaTime);
        }

        public void SetupHitEffects(GameObject hitEffectPrefab)
        {
            _hitEffectPrefab = hitEffectPrefab;
        }

        public void SetTarget(Health target, GameObject instigator, float damage)
        {
            _target = target;
            _damage = damage;
            _instigator = instigator;

            Destroy(gameObject, maxLifeTime);
        }

        private Vector3 GetAimLocation()
        {
            CapsuleCollider targetCapsule = _target.GetComponent<CapsuleCollider>();
            if (targetCapsule == null)
            {
                return _target.transform.position;
            }
            return _target.transform.position + Vector3.up * targetCapsule.height / 2;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.GetComponent<Health>() != _target) { return; }
            if (_target.IsDead()) { return; }
            _target.TakeDamage(_instigator, _damage);

            if (_hitEffectPrefab != null)
            {
                Instantiate(_hitEffectPrefab, GetAimLocation(), transform.rotation);
            }

            foreach (GameObject toDestroy in destoyOnHit)
            {
                Destroy(toDestroy);
            }

            Destroy(gameObject, lifeAfterInpact);
        }
    }
}