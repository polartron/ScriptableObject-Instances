using UnityEngine;

namespace Generated.Events
{
    public class TransformEventInvokeable : MonoBehaviour
    {
        public TransformEventReference Reference;

        public void Invoke(UnityEngine.Transform value)
        {
            Reference.Invoke(value);
        }
    }
}

