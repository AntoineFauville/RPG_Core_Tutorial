using RPG.Combat;
using RPG.Movement;
using RPG.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.AI;
using RPG.Inventories;

namespace RPG.Control
{
    public class PlayerController : MonoBehaviour
    {
        private bool _holdOnSpamming;
        [SerializeField] private float _timeSpammClick = 0.1f;
        [SerializeField] private float _minimumDistanceToMove = 0.5f;
        [SerializeField] private float _maxNavMeshProjectionDistance = 1f;
        [SerializeField] private float _raycastRadius = 1f;
        [SerializeField] private CursorMapping[] _cursorMappings = null;

        private Ray _lastRay;

        private Health _health;

        bool isDraggingUI = false;
        
        [System.Serializable]
        struct CursorMapping
        {
            public CursorType type;
            public Texture2D texture;
            public Vector2 hotspot;
        }
        
        private void Awake()
        {
            _health = GetComponent<Health>();
        }

        void Update()
        {
            CheckSpecialAbilityKeys();

            if (InteractWithUI()) { return; }

            if (_health.IsDead())
            {
                SetCursor(CursorType.None);
                return;
            }

            if (InteractWithComponent()) { return; }
            if (InteractWithMovement()) { return; }

            SetCursor(CursorType.None);
        }

        void CheckSpecialAbilityKeys()
        {
            var actionStore = GetComponent<ActionStore>();
            if (Input.GetKeyDown(KeyCode.Alpha1))
            {
                actionStore.Use(0, gameObject);
            }
        }

        private bool InteractWithComponent()
        {
            RaycastHit[] hits = RaycastAllSorted();
            foreach (RaycastHit hit in hits)
            {
                IRaycastable[] raycastables = hit.transform.GetComponents<IRaycastable>();
                foreach (IRaycastable raycastable in raycastables)
                {
                    if (raycastable.HandleRaycast(this))
                    {
                        SetCursor(raycastable.GetCursorType());
                        return true;
                    }
                }
            }
            return false;
        }

        RaycastHit[] RaycastAllSorted()
        {
            RaycastHit[] hits = Physics.SphereCastAll(GetMouseRay(), _raycastRadius);
            float[] distances = new float[hits.Length];
            for (int i = 0; i < hits.Length; i++)
            {
                distances[i] = hits[i].distance;
            }
            Array.Sort(distances, hits);
            return hits;
        }

        private bool InteractWithMovement()
        {
            Debug.DrawRay(_lastRay.origin, _lastRay.direction * 100);
            _lastRay = GetMouseRay();

            Vector3 target;
            bool hasHit = RaycastNavMesh(out target);

            if (Vector3.Distance(target, transform.position) > _minimumDistanceToMove)
            {
                if (hasHit)
                {
                    if (!GetComponent<Mover>().CanMoveTo(target)) return false;

                    if (Input.GetMouseButton(0) && !_holdOnSpamming)
                    {
                        _holdOnSpamming = true;
                        GetComponent<Mover>().StartMoveAction(target, 1f);
                        StartCoroutine(HoldOnSpamming());
                    }
                    SetCursor(CursorType.Movement);
                    return true;
                }
            }
            return false;
        }

        private bool RaycastNavMesh(out Vector3 target)
        {
            target = new Vector3();

            RaycastHit hit;
            bool hasHit = Physics.Raycast(GetMouseRay(), out hit);
            if (!hasHit) return false;

            NavMeshHit navMeshHit;
            bool hasCastToNavMesh = NavMesh.SamplePosition(
                hit.point, out navMeshHit, _maxNavMeshProjectionDistance, NavMesh.AllAreas);
            if (!hasCastToNavMesh) return false;

            target = navMeshHit.position;
            
            return true;
        }

        public bool InteractWithUI()
        {
            if (Input.GetMouseButtonUp(0))
            {
                isDraggingUI = false;
            }
            if (EventSystem.current.IsPointerOverGameObject())
                {
                if (Input.GetMouseButtonDown(0))
                {
                    isDraggingUI = true;
                }

                SetCursor(CursorType.UI);
                return true;
            }

            if (isDraggingUI)
            {
                return true;
            }

            return false;
        }

        private void SetCursor(CursorType cursorType)
        {
            CursorMapping mapping = GetCursorMapping(cursorType);
            Cursor.SetCursor(mapping.texture, mapping.hotspot, CursorMode.Auto);
        }

        private CursorMapping GetCursorMapping(CursorType type)
        {
            foreach (CursorMapping mapping in _cursorMappings)
            {
                if (mapping.type == type)
                {
                    return mapping;
                }
            }
            return _cursorMappings[0];
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
