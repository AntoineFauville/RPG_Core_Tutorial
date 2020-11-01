﻿using GameDevTV.Utils;
using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using RPG.Resources;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{
    public class AIController : MonoBehaviour
    {
        private GameObject _player;
        
        LazyValue<Vector3> _guardPosition;
        private float _timeSinceLastSawPlayer = Mathf.Infinity;
        private float _timeSinceArrivedAtWaypoint = Mathf.Infinity;
        private int _currentWayPointIndex = 0;

        [SerializeField] private float _chaseDistance = 5f;
        [SerializeField] private float _suspicionTime = 3f;
        [SerializeField] private PatrolPath _patrolPath;
        [SerializeField] private float _waypointTolerance = 1f;
        [SerializeField] private float _waypointDwellTime = 1f; // waiting time
        [Range(0,1)]
        [SerializeField] private float _patrolFractionSpeed = 0.2f;

        private void Awake()
        {
            _player = GameObject.FindGameObjectWithTag("Player");

            _guardPosition = new LazyValue<Vector3>(GetGuardPosition);
        }

        private Vector3 GetGuardPosition()
        {
            return transform.position;
        }

        private void Start()
        {
            _guardPosition.ForceInit();
        }

        private void Update()
        {
            if (GetComponent<Health>().IsDead()) { return; }

            if (InAttackRangeOfPlayer() && GetComponent<Fighter>().CanAttack(_player))
            {
                AttackBehavior();
            }
            else if (_timeSinceLastSawPlayer < _suspicionTime)
            {
                SuspicionBehavior();
            }
            else
            {
                PatrolBehavior();
            }

            UpdateTimes();
        }

        private void UpdateTimes()
        {
            _timeSinceLastSawPlayer += Time.deltaTime;
            _timeSinceArrivedAtWaypoint += Time.deltaTime;
        }

        private void PatrolBehavior()
        {
            Vector3 nextPosition = _guardPosition.value;

            if (_patrolPath != null)
            {
                if (AtWayPoint())
                {
                    _timeSinceArrivedAtWaypoint = 0;
                    CycleWayPoint();
                }

                nextPosition = GetCurrentWaypoint();
            }

            if (_timeSinceArrivedAtWaypoint > _waypointDwellTime)
            {
                GetComponent<Mover>().StartMoveAction(nextPosition, _patrolFractionSpeed); // this cancels the previous action and move to next action
            }
        }
        
        private bool AtWayPoint()
        {
            float distanceToWayPoint = Vector3.Distance(transform.position, GetCurrentWaypoint());
            return distanceToWayPoint < _waypointTolerance;
        }

        private void CycleWayPoint()
        {
            _currentWayPointIndex = _patrolPath.GetNextIndex(_currentWayPointIndex);
        }

        private Vector3 GetCurrentWaypoint()
        {
            return _patrolPath.GetWaypoint(_currentWayPointIndex);
        }

        private void SuspicionBehavior()
        {
            GetComponent<ActionScheduler>().CancelCurrentAction();
        }

        private void AttackBehavior()
        {
            _timeSinceLastSawPlayer = 0;
            GetComponent<Fighter>().Attack(_player);
        }

        private bool InAttackRangeOfPlayer()
        {
            float distanceToPlayer = Vector3.Distance(_player.transform.position, transform.position);
            return distanceToPlayer < _chaseDistance;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(transform.position, _chaseDistance);
        }
    }
}