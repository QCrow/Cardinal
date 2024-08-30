using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ResourceUIDisplay : MonoBehaviour
{
    [SerializeField]
    private GameObject _energyUIPrefab;  // The prefab for the Energy UI element
    [SerializeField]
    private GameObject _foodUIPrefab;    // The prefab for the Food UI element
    [SerializeField]
    private GameObject _moraleUIPrefab;  // The prefab for the Morale UI element

    [SerializeField]
    private RectTransform _resourceCanvas;

    private Dictionary<ResourceType, GameObject> _resourceUIMap = new Dictionary<ResourceType, GameObject>();

    public void InitializeUI(int initialEnergy, int initialFood, int initialMorale)
    {
        // Instantiate and store resource UI elements with initial values
        _resourceUIMap[ResourceType.Energy] = InstantiateResourceUI(_energyUIPrefab, initialEnergy);
        _resourceUIMap[ResourceType.Food] = InstantiateResourceUI(_foodUIPrefab, initialFood);
        _resourceUIMap[ResourceType.Morale] = InstantiateResourceUI(_moraleUIPrefab, initialMorale);
    }

    private GameObject InstantiateResourceUI(GameObject resourcePrefab, int resourceValue)
    {
        // Instantiate the UI element from the specific prefab
        GameObject resourceUI = Instantiate(resourcePrefab, _resourceCanvas);

        // Set the resource value text
        UpdateResourceValueText(resourceUI, resourceValue);

        return resourceUI;
    }

    public void UpdateResourceUI(ResourceType type)
    {
        if (_resourceUIMap.TryGetValue(type, out GameObject resourceUI))
        {
            int newValue = ResourceManager.Instance.GetResourceValue(type);
            UpdateResourceValueText(resourceUI, newValue);
        }
    }

    private void UpdateResourceValueText(GameObject resourceUI, int newValue)
    {
        TextMeshProUGUI resourceValueText = resourceUI.GetComponentInChildren<TextMeshProUGUI>();
        if (resourceValueText != null)
        {
            resourceValueText.text = newValue.ToString();
        }
    }

}
