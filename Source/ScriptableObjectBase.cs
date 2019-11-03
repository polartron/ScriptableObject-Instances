using System;
using System.Collections.Generic;
using UnityEngine;

namespace Fasteraune.SO.Instances
{
    [Serializable]
    public class ScriptableObjectBase : ScriptableObject
    {
        internal Dictionary<InstanceOwner, ScriptableObject> instanceValues
            = new Dictionary<InstanceOwner, ScriptableObject>();

        internal void ClearConnection(InstanceOwner connection)
        {
            instanceValues.Remove(connection);
        }
    }
}
