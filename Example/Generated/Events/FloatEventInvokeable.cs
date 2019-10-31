using System.Collections;
using System.Collections.Generic;
using Generated.Events;
using UnityEngine;

public class FloatEventInvokeable : MonoBehaviour
{
    public FloatEventReference FloatEvent;
    
    public void Invoke(float value)
    {
        FloatEvent.Invoke(value);
    }
}
