using System.Collections.Generic;
using UnityEngine;

namespace Fasteraune.SO
{
    public class InstanceOwner : MonoBehaviour
    {
        public InstanceOwner Parent;

        private List<ScriptableObjectBase> connectedScriptableObjects = new List<ScriptableObjectBase>();

        internal void Register(ScriptableObjectBase scriptableObject)
        {
            connectedScriptableObjects.Add(scriptableObject);
        }

        private void OnDestroy()
        {
            foreach (var scriptableObjects in connectedScriptableObjects)
            {
                if (scriptableObjects != null)
                {
                    scriptableObjects.ClearConnection(this);
                }
            }
        }
    }
}