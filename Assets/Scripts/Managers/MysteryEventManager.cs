using UnityEngine;
using TMPro;
using UnityEngine.UI; // For Button, if you're using Unity UI
using System.Collections.Generic;

public class MysteryEventManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI eventDescriptionTMP;
    [SerializeField] private Transform optionContainer;   // Parent for choice buttons
    [SerializeField] private GameObject optionButtonPrefab; // Prefab for each choice button
    [SerializeField] private TextMeshProUGUI outcomeDescriptionTMP;

    public MysteryEvent currentEvent;

    // 1. Load the event and set up UI
    public void LoadEvent(MysteryEvent newEvent)
    {
        currentEvent = newEvent;

        // Clear existing UI
        ClearOptionsUI();

        // Display the event description
        eventDescriptionTMP.text = currentEvent.description;

        // Create buttons for each option
        foreach (var option in currentEvent.options)
        {
            // Instantiate the button prefab under optionContainer
            GameObject buttonGO = Instantiate(optionButtonPrefab, optionContainer);

            // Get the Button component (or whatever script handles clicks)
            Button btn = buttonGO.GetComponent<Button>();

            // Set the button text to the option's text
            TextMeshProUGUI btnText = buttonGO.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null)
            {
                btnText.text = option.optionText;
            }

            // Determine if the option can be selected (condition check)
            bool canSelect = (option.rootConditionNode == null)
                             || option.rootConditionNode.Evaluate();

            // Enable/Disable the button accordingly
            btn.interactable = canSelect;

            // Add listener to handle click
            btn.onClick.AddListener(() => ChooseOption(option));
        }

        // Clear any previous outcome description
        outcomeDescriptionTMP.text = "";
    }

    // 2. When the player picks an option
    public void ChooseOption(EventOption option)
    {
        // Double-check condition in case something changed
        if (option.rootConditionNode != null && !option.rootConditionNode.Evaluate())
        {
            // Option is invalid now, do nothing or show a message
            return;
        }

        // Apply outcomes
        List<string> outcomeDescriptions = new List<string>();
        foreach (var outcome in option.outcomes)
        {
            // Apply the effect
            outcome.ApplyOutcome();

            // Gather the outcome's description text
            if (!string.IsNullOrEmpty(outcome.outcomeDescription))
            {
                outcomeDescriptions.Add(outcome.outcomeDescription);
            }
        }

        // Display all outcome descriptions (joined or formatted nicely)
        outcomeDescriptionTMP.text = string.Join("\n", outcomeDescriptions);

        // Optionally hide the options or proceed to next step, etc.
        // ...
    }

    private void ClearOptionsUI()
    {
        // Remove old buttons from the container
        for (int i = optionContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(optionContainer.GetChild(i).gameObject);
        }
    }
}
