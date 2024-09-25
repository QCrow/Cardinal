// using System.Collections.Generic;
// using UnityEngine;
// using TMPro;

// /// <summary>
// /// Manages the display of resource UI elements (Energy, Food, Morale) on the screen.
// /// Handles the initialization and updating of these UI elements based on current resource values.
// /// </summary>
// public class ResourceUIDisplay : MonoBehaviour
// {
//     // TODO: Change into a dictionary to store all prefabs
//     // Prefabs for each resource UI element
//     [SerializeField]
//     private GameObject _energyUIPrefab;
//     [SerializeField]
//     private GameObject _foodUIPrefab;
//     [SerializeField]
//     private GameObject _moraleUIPrefab;
//     // TODO: Add a new prefab for pollution

//     // The parent RectTransform where resource UI elements will be instantiated
//     [SerializeField]
//     private RectTransform _resourceCanvas;

//     // Dictionary to store the instantiated UI elements by resource type
//     private Dictionary<ResourceType, GameObject> _resourceUIMap = new();

//     /// <summary>
//     /// Initializes the UI elements for each resource with the given initial values.
//     /// </summary>
//     /// <param name="initialEnergy">Initial value for Energy.</param>
//     /// <param name="initialFood">Initial value for Food.</param>
//     /// <param name="initialMorale">Initial value for Morale.</param>
//     public void InitializeUI(int initialEnergy, int initialFood, int initialMorale)
//     {
//         // Instantiate and store resource UI elements with initial values
//         _resourceUIMap[ResourceType.Energy] = InstantiateResourceUI(_energyUIPrefab, initialEnergy);
//         _resourceUIMap[ResourceType.Food] = InstantiateResourceUI(_foodUIPrefab, initialFood);
//         _resourceUIMap[ResourceType.Morale] = InstantiateResourceUI(_moraleUIPrefab, initialMorale);
//     }

//     /// <summary>
//     /// Instantiates a resource UI element from the given prefab and sets its initial value.
//     /// </summary>
//     /// <param name="resourcePrefab">The prefab to instantiate.</param>
//     /// <param name="resourceValue">The initial value to display on the UI element.</param>
//     /// <returns>The instantiated UI element.</returns>
//     private GameObject InstantiateResourceUI(GameObject resourcePrefab, int resourceValue)
//     {
//         // Instantiate the UI element as a child of the resource canvas
//         GameObject resourceUI = Instantiate(resourcePrefab, _resourceCanvas);

//         // Set the resource value text
//         UpdateResourceValueText(resourceUI, resourceValue);

//         return resourceUI;
//     }

//     /// <summary>
//     /// Updates the UI element associated with the given resource type.
//     /// </summary>
//     /// <param name="type">The type of resource to update (Energy, Food, Morale).</param>
//     /// <param name="newValue">The newValue of resource to update.</param>
//     private void UpdateResourceUI(ResourceType type, int newValue)
//     {
//         if (_resourceUIMap.TryGetValue(type, out GameObject resourceUI))
//         {
//             UpdateResourceValueText(resourceUI, newValue);
//         }
//         else
//         {
//             Debug.LogWarning($"Resource UI for {type} not found.");
//         }
//     }

//     /// <summary>
//     /// Updates the displayed value of a resource UI element.
//     /// </summary>
//     /// <param name="resourceUI">The UI element to update.</param>
//     /// <param name="newValue">The new value to display.</param>
//     private void UpdateResourceValueText(GameObject resourceUI, int newValue)
//     {
//         // Find the TextMeshProUGUI component in the UI element
//         TextMeshProUGUI resourceValueText = resourceUI.GetComponentInChildren<TextMeshProUGUI>();

//         if (resourceValueText != null)
//         {
//             // Update the text to reflect the new resource value
//             resourceValueText.text = newValue.ToString();
//         }
//         else
//         {
//             Debug.LogWarning("TextMeshProUGUI component not found in resource UI.");
//         }
//     }

//     /// <summary>
//     /// Called when the UI component is enabled.
//     /// Subscribes to the ResourceManager's OnResourceChanged event to receive updates
//     /// whenever a resource value changes. This ensures the UI stays in sync with the
//     /// resource values.
//     /// </summary>
//     private void OnEnable()
//     {
//         // Subscribe to the event when the UI is enabled
//         ResourceManager.Instance.OnResourceChanged += UpdateResourceUI;
//     }

//     /// <summary>
//     /// Called when the UI component is disabled.
//     /// Unsubscribes from the ResourceManager's OnResourceChanged event to prevent memory leaks
//     /// or unintended behavior when the UI component is not active.
//     /// </summary>
//     private void OnDisable()
//     {
//         // Unsubscribe to prevent memory leaks when the UI is disabled
//         ResourceManager.Instance.OnResourceChanged -= UpdateResourceUI;
//     }
// }