using System.Collections.Generic;
using UnityEngine;

namespace Fasteraune.Variables
{
    public class InstancedVariableOwner : MonoBehaviour
    {
        public InstancedVariableOwner Parent;

        private List<Variable> connectedVariables = new List<Variable>();

        internal void Register(Variable variable)
        {
            connectedVariables.Add(variable);
        }

        private void OnDestroy()
        {
            foreach (var variable in connectedVariables)
            {
                if (variable != null)
                {
                    variable.ClearConnection(this);
                }
            }
        }
    }
}