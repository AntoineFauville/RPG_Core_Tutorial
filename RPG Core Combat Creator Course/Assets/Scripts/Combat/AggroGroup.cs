using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using RPG.Saving;

namespace RPG.Combat
{
    //example of script
    //where you can trigger multiple enemies using events to attack you
    //use this script when you have few enemies near the npc that was talking

    public class AggroGroup : MonoBehaviour, ISaveable
    {
        [SerializeField] Fighter[] fighters;
        [SerializeField] bool activateOnStart = false;
        bool hasBeenActivated = false;

        private void Awake()
        {
            ActivateFightingOnPassiveFighters(activateOnStart);
        }

        public void ActivateFightingOnPassiveFighters(bool shouldActivate)
        {
            hasBeenActivated = shouldActivate;

            foreach (Fighter fighter in fighters)
            {
                CombatTarget combatTarget = fighter.GetComponent<CombatTarget>();
                if (combatTarget != null)
                {
                    combatTarget.enabled = hasBeenActivated;
                }
                fighter.enabled = hasBeenActivated;
            }
        }

        public object CaptureState()
        {
            return hasBeenActivated;
        }

        public void RestoreState(object state)
        {
            hasBeenActivated = (bool)state;

            ActivateFightingOnPassiveFighters(hasBeenActivated);
        }
    }
}