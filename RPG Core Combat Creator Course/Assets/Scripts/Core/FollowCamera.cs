using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] private Transform Target;

        void LateUpdate()
        {
            this.transform.position = Target.position;
        }
    }
}
