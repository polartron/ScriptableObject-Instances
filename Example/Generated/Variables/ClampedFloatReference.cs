using System;
using System.Collections;
using System.Collections.Generic;
using Fasteraune.Variables;
using UnityEngine;

[Serializable]
public class ClampedFloatReference : BaseReference<float, FloatVariable>
{
    [Serializable]
    public class ClampData
    {
        public FloatReference Min = new FloatReference();
        public FloatReference Max = new FloatReference();
    }
    
    public ClampData Clamp = new ClampData();
    
    public ClampedFloatReference(float Value) : base(Value)
    {
    }

    public ClampedFloatReference()
    {
    }

    public override float Value
    {
        get
        {
            float value = base.Value;
            float maxValue = Clamp.Max.Value;
                    
            if (value.CompareTo(maxValue) > 0)
            {
                base.Value = maxValue;
                return maxValue;
            }
            
            float minValue = Clamp.Min.Value;
                 
            if (value.CompareTo(minValue) < 0)
            {
                base.Value = minValue;
                return minValue;
            }

            return value;
        }
        set
        {
            float maxValue = Clamp.Max.Value;
                    
            if (value.CompareTo(maxValue) > 0)
            {
                base.Value = maxValue;
                return;
            }
            
            float minValue = Clamp.Min.Value;
                 
            if (value.CompareTo(minValue) < 0)
            {
                base.Value = minValue;
                return;
            }
            
            base.Value = value; 
        }
    }

    private void OnMinValueChanged(float minValue)
    {
        if (minValue.CompareTo(base.Value) < 0)
        {
            base.Value = minValue;
        }
    }
    
    private void OnMaxValueChanged(float maxValue)
    {
        if (maxValue.CompareTo(base.Value) > 0)
        {
            base.Value = maxValue;
        }
    }

    public override void AddListener(Action<float> listener)
    {
        Clamp.Min.AddListener(OnMinValueChanged);
        Clamp.Max.AddListener(OnMaxValueChanged);
        base.AddListener(listener);
    }
    
    public override void RemoveListener(Action<float> listener)
    {
        Clamp.Min.RemoveListener(OnMinValueChanged);
        Clamp.Max.RemoveListener(OnMaxValueChanged);
        base.RemoveListener(listener);
    }
}

[Serializable]
public class ClampedPlayerDataReference : BaseReference<Player.PlayerData, PlayerDataVariable>
{
    [Serializable]
    public class ClampData
    {
        public PlayerDataReference Min = new PlayerDataReference();
        public PlayerDataReference Max = new PlayerDataReference();
    }
    
    public ClampData Clamp = new ClampData();
    
    public ClampedPlayerDataReference(Player.PlayerData Value) : base(Value)
    {
    }

    public ClampedPlayerDataReference()
    {
    }

    public override Player.PlayerData Value
    {
        get
        {
            Player.PlayerData value = base.Value;
            Player.PlayerData maxValue = Clamp.Max.Value;
                    
            if (value.CompareTo(maxValue) > 0)
            {
                base.Value = maxValue;
                return maxValue;
            }
            
            Player.PlayerData minValue = Clamp.Min.Value;
                 
            if (value.CompareTo(minValue) < 0)
            {
                base.Value = minValue;
                return minValue;
            }

            return value;
        }
        set
        {
            Player.PlayerData maxValue = Clamp.Max.Value;
                    
            if (value.CompareTo(maxValue) > 0)
            {
                base.Value = maxValue;
                return;
            }
            
            Player.PlayerData minValue = Clamp.Min.Value;
                 
            if (value.CompareTo(minValue) < 0)
            {
                base.Value = minValue;
                return;
            }
            
            base.Value = value; 
        }
    }

    private void OnMinValueChanged(Player.PlayerData minValue)
    {
        if (minValue.CompareTo(base.Value) < 0)
        {
            base.Value = minValue;
        }
    }
    
    private void OnMaxValueChanged(Player.PlayerData maxValue)
    {
        if (maxValue.CompareTo(base.Value) > 0)
        {
            base.Value = maxValue;
        }
    }

    public override void AddListener(Action<Player.PlayerData> listener)
    {
        Clamp.Min.AddListener(OnMinValueChanged);
        Clamp.Max.AddListener(OnMaxValueChanged);
        base.AddListener(listener);
    }
    
    public override void RemoveListener(Action<Player.PlayerData> listener)
    {
        Clamp.Min.RemoveListener(OnMinValueChanged);
        Clamp.Max.RemoveListener(OnMaxValueChanged);
        base.RemoveListener(listener);
    }
}