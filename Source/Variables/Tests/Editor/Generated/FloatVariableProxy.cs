using UnityEngine;
using Fasteraune.Variables;

#if UNITY_EDITOR

namespace Fasteraune.Variables
{
	class FloatVariableProxy : ScriptableObject
	{ 
		public float ProxyValue;
	}
}
#endif