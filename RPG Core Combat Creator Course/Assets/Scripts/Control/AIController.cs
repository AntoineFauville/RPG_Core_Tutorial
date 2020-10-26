using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        private Fighter _fighter;
        private Health _health;
        private GameObject _player;

        [SerializeField] private float _chaseDistance = 5f;

        void Start()
        {
            _fighter = GetComponent<Fighter>();
            _player = GameObject.FindGameObjectWithTag("Player");
            _health = GetComponent<Health>();
        }

        private void Update()
        {
            if (_health.IsDead()) { return; }

            if (InAttackRangeOfPlayer() && _fighter.CanAttack(_player))
            {
                _fighter.Attack(_player);
            }
            else
            {
                _fighter.Cancel();
            }
        }

        private bool InAttackRangeOfPlayer()
        {
            float distanceToPlayer = Vector3.Distance(_player.transform.position, transform.position);
            return distanceToPlayer < _chaseDistance;
        }
    }
}