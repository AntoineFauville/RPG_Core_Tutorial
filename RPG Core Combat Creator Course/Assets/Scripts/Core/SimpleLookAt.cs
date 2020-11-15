﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleLookAt : MonoBehaviour
{
    [SerializeField] GameObject target;
    void Update()
    {
        if (target != null)
        {
            transform.LookAt(target.transform);
        }
    }
}
