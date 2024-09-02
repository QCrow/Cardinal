using UnityEngine;

[System.Serializable]
public class Resource
{
    [SerializeField]
    private int _currentValue;
    [SerializeField]
    private int _maxValue;

    public int CurrentValue
    {
        get => _currentValue;
        set => _currentValue = Mathf.Clamp(value, 0, _maxValue);
    }

    public int MaxValue
    {
        get => _maxValue;
        set => _maxValue = Mathf.Clamp(value, 0, int.MaxValue);
    }

    public Resource(int currentValue = 0, int maxValue = int.MaxValue)
    {
        CurrentValue = Mathf.Clamp(currentValue, 0, maxValue);
        MaxValue = maxValue;
    }
}
