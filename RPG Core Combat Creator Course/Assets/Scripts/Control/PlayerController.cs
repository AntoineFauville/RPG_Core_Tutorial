using RPG.Combat;
using RPG.Core;
using RPG.Movement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        private Mover _mover;
        private Health _health;

        private bool _holdOnSpamming;
        [SerializeField] private float _timeSpammClick = 0.1f;
        [SerializeField] private float _minimumDistanceToMove = 0.5f;

        Ray lastRay;
        
        void Start()
        {
            _mover = GetComponent<Mover>();
            _health = GetComponent<Health>();
        }
        
        void Update()
        {
            if (_health.IsDead()) { return; }

            if (InteractWithCombat()) { return; }
            if (InteractWithMovement()) { return; }
        }

        private bool InteractWithCombat()
        {
            RaycastHit[] hits = Physics.RaycastAll(GetMouseRay());
            foreach (RaycastHit hit in hits)
            {
                CombatTarget target = hit.transform.GetComponent<CombatTarget>();
                if (target == null) { continue; }
                
                if (!GetComponent<Fighter>().CanAttack(target.gameObject))
                {
                    continue;
                }

                if (Input.GetMouseButton(0))
                {
                    GetComponent<Fighter>().Attack(target.gameObject);
                }
                return true;
            }

            return false;
        }

        private bool InteractWithMovement()
        {
            Debug.DrawRay(lastRay.origin, lastRay.direction * 100);
            lastRay = GetMouseRay();
            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);

            if (Vector3.Distance(hit.point, transform.position) > _minimumDistanceToMove)
            {
                if (hasHit)
                {
                    if (Input.GetMouseButton(0) && !_holdOnSpamming)
                    {
                        _holdOnSpamming = true;
                        _mover.StartMoveAction(hit.point, 1f);
                        StartCoroutine(HoldOnSpamming());
                    }
                    return true;
                }
            }
            return false;
        }

        private static Ray GetMouseRay()
        {
            return Camera.main.ScreenPointToRay(Input.mousePosition);
        }

        IEnumerator HoldOnSpamming()
        {
            yield return new WaitForSeconds(_timeSpammClick);
            _holdOnSpamming = false;
        }
    }
}
