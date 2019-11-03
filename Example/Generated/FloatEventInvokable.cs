using UnityEngine;

namespace Generated.Events
{
    public class FloatEventInvokable : MonoBehaviour
    {
        public FloatEventReference Reference;

        public void Invoke(float value)
        {
            Reference.Invoke(value);
        }
    }
}

