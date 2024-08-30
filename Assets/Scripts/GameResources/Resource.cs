using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Resource
{
    [SerializeField]
    private int _currentValue;
    [SerializeField]
    private int _maxValue;

    public Resource(int currentValue, int maxValue)
    {
        _currentValue = Mathf.Clamp(currentValue, 0, maxValue);
        _maxValue = maxValue;
    }

    public int CurrentValue
    {
        get => _currentValue;
        private set => _currentValue = Mathf.Clamp(value, 0, _maxValue);
    }

    public int MaxValue
    {
        get => _maxValue;
        set => _maxValue = Mathf.Max(0, value);
    }

    public void Increase(int amount)
    {
        CurrentValue += amount;
    }
}
