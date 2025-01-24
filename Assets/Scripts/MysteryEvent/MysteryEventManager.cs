using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

public class MysteryEventManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI eventDescriptionTMP;
    [SerializeField] private Transform optionContainer;
    [SerializeField] private GameObject optionButtonPrefab;
    [SerializeField] private TextMeshProUGUI outcomeDescriptionTMP;

    [Header("Event Database Reference")]
    public MysteryEventDatabase eventDatabase;

    public List<int> availableEventIds = new List<int>();

    public MysteryEvent currentEvent;

    public static MysteryEventManager Instance { get; private set; }

    // Keep references to all spawned option buttons
    private List<Button> spawnedButtons = new List<Button>();

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    // Load a random event from the database
    public void LoadRandomEventByWeight()
    {
        Random.InitState(GameManager.Instance.Seed); // Initialize the random number generator with the given seed

        if (eventDatabase == null || eventDatabase.events == null || eventDatabase.events.Count == 0)
        {
            Debug.LogWarning("EventDatabase is missing or empty.");
            return;
        }

        // Filter the events based on unlocked IDs
        List<MysteryEvent> unlockedEvents = eventDatabase.events
            .Where(e => availableEventIds.Contains(e.id))
            .ToList();

        if (unlockedEvents.Count == 0)
        {
            Debug.LogWarning("No unlocked events are available.");
            return;
        }

        // Calculate the total weight of unlocked events
        int totalWeight = unlockedEvents.Sum(e => e.weight);
        Debug.Log("Total weight of unlocked events: " + totalWeight);

        if (totalWeight <= 0)
        {
            Debug.LogWarning("Total weight is zero or negative. Cannot load random event.");
            return;
        }

        // Generate a random value between 0 and total weight
        int randomValue = Random.Range(0, totalWeight);
        Debug.Log("Random value: " + randomValue);

        // Find the event corresponding to the random value
        int cumulativeWeight = 0;
        foreach (MysteryEvent mysteryEvent in unlockedEvents)
        {
            cumulativeWeight += mysteryEvent.weight;
            if (randomValue < cumulativeWeight)
            {
                LoadEvent(mysteryEvent);
                availableEventIds.Remove(mysteryEvent.id);
                Debug.Log($"Event ID {mysteryEvent.id} removed from unlockedEventIds.");
                return;
            }
        }

        Debug.LogError("Failed to select a random event. This should never happen if weights are correct.");
    }


    // Load a specific event and set up UI
    public void LoadEvent(MysteryEvent newEvent)
    {
        currentEvent = newEvent;
        ClearOptionsUI();

        // Display the event description
        eventDescriptionTMP.text = currentEvent.description;
        Debug.Log("description updated");

        bool anyInteractable = false; // Track if at least one option is clickable
        spawnedButtons.Clear();

        // Create buttons for each option
        foreach (var option in currentEvent.options)
        {
            GameObject buttonGO = Instantiate(optionButtonPrefab, optionContainer);
            Button btn = buttonGO.GetComponent<Button>();

            // Set button text
            TextMeshProUGUI btnText = buttonGO.GetComponentInChildren<TextMeshProUGUI>();
            if (btnText != null)
            {
                btnText.text = option.optionText;
            }

            // Condition check for initial interactability
            bool canSelect = (option.rootConditionNode == null) || option.rootConditionNode.Evaluate();
            btn.interactable = canSelect;
            if (canSelect) anyInteractable = true;

            // Store the button
            spawnedButtons.Add(btn);

            // Pass both btn and option into the callback
            btn.onClick.AddListener(() => ChooseOption(btn, option));
        }

        // If no button is interactable, automatically return to map after 2s
        if (!anyInteractable)
        {
            outcomeDescriptionTMP.text = "No available choices...";
            Invoke(nameof(GoBackToMap), 2f);
        }

        // Clear any old outcome description
        outcomeDescriptionTMP.text = "";
    }

    // Load an event by its ID
    public void LoadEventById(int eventId)
    {
        if (eventDatabase == null || eventDatabase.events == null || eventDatabase.events.Count == 0)
        {
            Debug.LogWarning("EventDatabase is missing or empty.");
            return;
        }

        // Find the event with the specified ID
        MysteryEvent targetEvent = eventDatabase.events.Find(e => e.id == eventId);

        if (targetEvent != null)
        {
            // Load the found event
            LoadEvent(targetEvent);
        }
        else
        {
            Debug.LogWarning($"Event with ID {eventId} not found in the database.");
        }
    }


    // When the player picks an option
    public void ChooseOption(Button clickedButton, EventOption option)
    {
        // Double-check condition
        if (option.rootConditionNode != null && !option.rootConditionNode.Evaluate())
        {
            return; // Do nothing if conditions fail
        }

        // Check if there's a LoadEventOutcome
        bool hasLoadEventOutcome = option.outcomes.Exists(o => o is LoadEventOutcome);

        // Apply outcomes
        List<string> outcomeDescriptions = new List<string>();
        foreach (var outcome in option.outcomes)
        {
            outcome.ApplyOutcome();
            if (!string.IsNullOrEmpty(outcome.outcomeDescription))
            {
                outcomeDescriptions.Add(outcome.outcomeDescription);
            }
        }

        outcomeDescriptionTMP.text = string.Join("\n", outcomeDescriptions);

        // If there's no LoadEventOutcome, go back to map after 2s
        if (!hasLoadEventOutcome)
        {
            // Disable ALL buttons once any option is chosen
            foreach (Button btn in spawnedButtons)
            {
                btn.interactable = false;
            }

            Invoke(nameof(GoBackToMap), 2f);
        }
    }


    private void GoBackToMap()
    {
        GameManager.Instance.ChangeGameState(GameState.Map);
    }

    private void ClearOptionsUI()
    {
        // Remove old buttons from the container
        for (int i = optionContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(optionContainer.GetChild(i).gameObject);
        }

        spawnedButtons.Clear();
    }
}
