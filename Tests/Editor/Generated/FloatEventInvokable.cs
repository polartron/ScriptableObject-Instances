using UnityEngine;

namespace Generated.Events
{
    public class FloatEventInvokeable : MonoBehaviour
    {
        public FloatEventReference Reference;

        public void Invoke(float value)
        {
            Reference.Invoke(value);
        }
    }
}

