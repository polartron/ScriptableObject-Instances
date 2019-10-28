using System;
using UnityEngine;

namespace Fasteraune.Variables
{
    [Serializable]
    public abstract class BaseReference
    {
    }

    public enum ReferenceType
    {
        ConstantValue,
        SharedReference,
        InstancedReference
    }

    [Serializable]
    public class BaseReference<TVariableType, TVariable> : BaseReference, IDisposable where TVariable : Variable<TVariableType>
    {
        public ReferenceType Type = ReferenceType.ConstantValue;
        public TVariableType ConstantValue;
        public TVariable Variable;
        public InstancedVariableOwner Connection;
        private event Action<TVariableType> OnConstantValueChanged;
        private Variable<TVariableType> instancedVariable;

        private Variable<TVariableType> InstancedVariable
        {
            get
            {
                if (instancedVariable == null)
                {
                    var connection = Connection.Parent ? Connection.Parent : Connection;
                    instancedVariable = Variable.GetOrCreateInstancedVariable(connection);
                }

                return instancedVariable;
            }
        }

        private bool caching = false;
        private TVariableType cachedVariable;

        public BaseReference()
        {
        }

        public BaseReference(TVariableType value) : base()
        {
            Value = value;
        }

        public static implicit operator TVariableType(BaseReference<TVariableType, TVariable> reference)
        {
            return reference.Value;
        }

        public virtual TVariableType Value
        {
            get
            {
                switch (Type)
                {
                    case ReferenceType.ConstantValue:
                    {
                        return ConstantValue;
                    }

                    case ReferenceType.SharedReference:
                    {
                        if (caching)
                        {
                            return cachedVariable;
                        }
                        
                        if (VariableReferenceMissing())
                        {
                            return default(TVariableType);
                        }

                        return Variable.Value;
                    }

                    case ReferenceType.InstancedReference:
                    {
                        if (VariableReferenceMissing() || ConnectionReferenceMissing())
                        {
                            return default(TVariableType);
                        }
                        
                        if (caching)
                        {
                            return cachedVariable;
                        }

                        return InstancedVariable.Value;
                    }

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }

            set
            {
                switch (Type)
                {
                    case ReferenceType.ConstantValue:
                    {
                        ConstantValue = value;

                        OnConstantValueChanged?.Invoke(value);

                        break;
                    }

                    case ReferenceType.SharedReference:
                    {
                        if (VariableReferenceMissing())
                        {
                            return;
                        }

                        Variable.Value = value;

                        break;
                    }

                    case ReferenceType.InstancedReference:
                    {
                        if (VariableReferenceMissing() || ConnectionReferenceMissing())
                        {
                            return;
                        }

                        InstancedVariable.Value = value;

                        break;
                    }

                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }

        public virtual void AddListener(Action<TVariableType> listener)
        {
            switch (Type)
            {
                case ReferenceType.ConstantValue:
                {
                    OnConstantValueChanged += listener;
                    break;
                }

                case ReferenceType.SharedReference:
                {
                    if (VariableReferenceMissing())
                    {
                        return;
                    }

                    Variable.OnValueChanged += listener;
                    break;
                }

                case ReferenceType.InstancedReference:
                {
                    if (VariableReferenceMissing() || ConnectionReferenceMissing())
                    {
                        return;
                    }

                    InstancedVariable.OnValueChanged += listener;
                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public virtual void RemoveListener(Action<TVariableType> listener)
        {
            switch (Type)
            {
                case ReferenceType.ConstantValue:
                {
                    OnConstantValueChanged -= listener;
                    break;
                }

                case ReferenceType.SharedReference:
                {
                    if (VariableReferenceMissing())
                    {
                        return;
                    }

                    Variable.OnValueChanged -= listener;
                    break;
                }

                case ReferenceType.InstancedReference:
                {
                    if (VariableReferenceMissing() || ConnectionReferenceMissing())
                    {
                        return;
                    }

                    InstancedVariable.OnValueChanged -= listener;
                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private bool VariableReferenceMissing()
        {
            if (Variable == null)
            {
                Debug.LogError("Missing reference to variable asset");
                return true;
            }

            return false;
        }

        private bool ConnectionReferenceMissing()
        {
            if (Connection == null)
            {
                Debug.LogError("Missing reference to InstancedVariableOwner script");
                return true;
            }

            return false;
        }

        public void EnableCaching()
        {
            switch (Type)
            {
                case ReferenceType.SharedReference:
                {
                    if (VariableReferenceMissing())
                    {
                        return;
                    }
                    
                    Variable.OnValueChanged += VariableOnOnValueChanged;
                    cachedVariable = Variable.Value;
                    break;
                }

                case ReferenceType.InstancedReference:
                {
                    if (VariableReferenceMissing() || ConnectionReferenceMissing())
                    {
                        return;
                    }

                    InstancedVariable.OnValueChanged += VariableOnOnValueChanged;
                    cachedVariable = InstancedVariable.Value;
                    break;
                }

                default:
                    throw new ArgumentOutOfRangeException();
            }

            caching = true;
        }

        private void VariableOnOnValueChanged(TVariableType value)
        {
            cachedVariable = value;
        }

        public void Dispose()
        {
            if (caching)
            {
                switch (Type)
                {
                    case ReferenceType.SharedReference:
                    {
                        if (VariableReferenceMissing())
                        {
                            return;
                        }
                        
                        Variable.OnValueChanged -= VariableOnOnValueChanged;
                        break;
                    }
    
                    case ReferenceType.InstancedReference:
                    {
                        if (VariableReferenceMissing() || ConnectionReferenceMissing())
                        {
                            return;
                        }
    
                        InstancedVariable.OnValueChanged -= VariableOnOnValueChanged;
                        break;
                    }

                    default:
                        throw new ArgumentOutOfRangeException();
                }                
            }
        }
    }
}