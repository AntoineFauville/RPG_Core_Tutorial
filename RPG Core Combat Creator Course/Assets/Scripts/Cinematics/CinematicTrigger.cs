using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using RPG.Saving;

namespace RPG.Cinematics
{
    public class CinematicTrigger : MonoBehaviour, ISaveable
    {
        [SerializeField] private bool alreadyTriggered;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!alreadyTriggered && other.gameObject.tag == "Player")
            {
                alreadyTriggered = true;
                GetComponent<PlayableDirector>().Play();
            }
        }

        public object CaptureState()
        {
            Debug.Log("Saved CinematicTrigger");
            return alreadyTriggered;
        }

        public void RestoreState(object state)
        {
            Debug.Log("Restored CinematicTrigger");
            alreadyTriggered = (bool)state;
        }
    }
}