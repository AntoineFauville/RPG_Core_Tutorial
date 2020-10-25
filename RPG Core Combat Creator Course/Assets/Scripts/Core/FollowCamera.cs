using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RPG.Core
{
    public class FollowCamera : MonoBehaviour
    {
        [SerializeField] private Transform Target;

        void Start()
        {
            if (Target == null)
            {
                Target = GameObject.Find("Player").transform;
            }
        }

        void LateUpdate()
        {
            this.transform.position = Target.position;
        }
    }
}
