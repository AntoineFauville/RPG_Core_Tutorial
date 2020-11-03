using RPG.Core;
using RPG.Attributes;
using RPG.Saving;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace RPG.Movement
{
    public class Mover : MonoBehaviour, IAction, ISaveable
    {
        [SerializeField] private Transform _target;
        [SerializeField] private float _maxSpeed = 6f;
                
        void Update()
        {
            GetComponent<NavMeshAgent>().enabled = !GetComponent<Health>().IsDead();

            UpdateAnimator();
        }

        public void StartMoveAction(Vector3 destination, float fractionSpeed)
        {
            GetComponent<ActionScheduler>().StartAction(this);
            MoveTo(destination, fractionSpeed);
        }
        
        public void MoveTo(Vector3 destination, float fractionSpeed)
        {
            GetComponent<NavMeshAgent>().destination = destination;
            GetComponent<NavMeshAgent>().speed = _maxSpeed * Mathf.Clamp01(fractionSpeed);
            GetComponent<NavMeshAgent>().isStopped = false;
        }

        public void Cancel()
        {
            GetComponent<NavMeshAgent>().isStopped = true;
        }

        private void UpdateAnimator()
        {
            Vector3 velocity = GetComponent<NavMeshAgent>().velocity;
            Vector3 localVelocity = transform.InverseTransformDirection(velocity);
            float speed = localVelocity.z;
            GetComponent<Animator>().SetFloat("forwardSpeed", speed);
        }

        //Saving Part
        [System.Serializable]
        struct MoverSaveData
        {
            public SerializableVector3 position;
            public SerializableVector3 rotation;
        }

        public object CaptureState()
        {
            MoverSaveData data = new MoverSaveData();
            data.position = new SerializableVector3(transform.position);
            data.rotation = new SerializableVector3(transform.eulerAngles);
            return data;
        }

        public void RestoreState(object state)
        {
            MoverSaveData data = (MoverSaveData)state;
            GetComponent<NavMeshAgent>().enabled = false;
            transform.position = data.position.ToVector();
            transform.eulerAngles = data.rotation.ToVector();
            GetComponent<NavMeshAgent>().enabled = true;
        }
    }
}
