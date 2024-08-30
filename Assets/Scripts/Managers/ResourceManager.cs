using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    private static ResourceManager _instance;
    public static ResourceManager Instance
    {
        get => _instance;
    }

    [SerializeField]
    private Resource _energy;
    [SerializeField]
    private Resource _food;
    [SerializeField]
    private Resource _morale;

    public Resource Energy => _energy;
    public Resource Food => _food;
    public Resource Morale => _morale;

    [SerializeField]
    private ResourceUIDisplay _resourceUIDisplay;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeResources();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeResources()
    {
        _energy = new Resource(98, 99); 
        _food = new Resource(50, 99);    // Food starts at 50, with a max of 99
        _morale = new Resource(75, 99);  

        // Initialize UI display
        _resourceUIDisplay.InitializeUI(_energy.CurrentValue, _food.CurrentValue, _morale.CurrentValue);
    }

    public void ModifyResource(ResourceType type, int amount)
    {
        switch (type)
        {
            case ResourceType.Energy:
                _energy.Increase(amount);
                break;
            case ResourceType.Food:
                _food.Increase(amount);
                break;
            case ResourceType.Morale:
                _morale.Increase(amount);
                break;
        }

        _resourceUIDisplay.UpdateResourceUI(type);
    }

    public int GetResourceValue(ResourceType type)
    {
        return type switch
        {
            ResourceType.Energy => _energy.CurrentValue,
            ResourceType.Food => _food.CurrentValue,
            ResourceType.Morale => _morale.CurrentValue,
            _ => 0,
        };
    }
}
