using System.Collections;
using System.Collections.Generic;
using Generated.Variables;
using UnityEngine;

public class TransformParent : MonoBehaviour
{
    public TransformVariableReference Parent;
    
    void Awake()
    {
        Parent.Value = transform;
    }
}
