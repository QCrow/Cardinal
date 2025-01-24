using System.Collections.Generic;
using Newtonsoft.Json;
using UnityEngine;

public class LocalizationHandler : MonoBehaviour
{
    public static LocalizationHandler Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            LoadCardLocalization();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    [SerializeField] private string _currentLanguage = "en";

    public string CurrentLanguage => _currentLanguage;


    public string GetCardName(int cardID)
    {
        // return _cardsLocalizationData[CurrentLanguage][cardID.ToString()].name;
        _cardsLocalizationData.TryGetValue(_currentLanguage, out var cardEntries);

        if (cardEntries.TryGetValue(cardID.ToString(), out var cardData))
        {
            return cardData.name;
        }

        return $"[MISSING CARD NAME: {cardID}]";
    }

    private Dictionary<string, Dictionary<string, CardLocalizationData>> _cardsLocalizationData;

    public void LoadCardLocalization()
    {
        // Load JSON file from Resources
        TextAsset jsonFile = Resources.Load<TextAsset>("Localization/Cards");
        if (jsonFile == null)
        {
            Debug.LogError("Localization file not found at Resources/Localization/Cards.json");
            return;
        }

        // Deserialize JSON using Newtonsoft.Json
        _cardsLocalizationData = JsonConvert.DeserializeObject<CardLocalizationRoot>(jsonFile.text).data;
        Debug.Log(_cardsLocalizationData.Count + " languages loaded.");
        // Print out the cards under the selected language
        if (_cardsLocalizationData.TryGetValue(_currentLanguage, out var cardEntries))
        {
            foreach (var cardEntry in cardEntries)
            {
                Debug.Log($"Card ID: {cardEntry.Key}, Name: {cardEntry.Value.name}, Description: {cardEntry.Value.description}");
            }
        }
        else
        {
            Debug.LogError($"No card entries found for language '{_currentLanguage}'.");
        }
    }

    public void SetLanguage(string languageCode)
    {
        if (_cardsLocalizationData.ContainsKey(languageCode))
        {
            _currentLanguage = languageCode;
        }
        else
        {
            Debug.LogError($"Language '{languageCode}' not found in localization data.");
        }
    }

    public string GetCardDescription(int cardID)
    {
        if (_cardsLocalizationData.TryGetValue(_currentLanguage, out var cardEntries) &&
            cardEntries.TryGetValue(cardID.ToString(), out var cardData))
        {
            return cardData.description;
        }
        return $"[MISSING CARD DESCRIPTION: {cardID}]";
    }

    public string ParseDynamicDescription(string description, AbstractCard template)
    {
        // Replace placeholders with dynamic values
        foreach (var baseValue in template.BaseValues)
        {
            string key = $"<{baseValue.Key}>";

            // Dynamic value coloring
            string styledValue = $"<color=\"#FFFFFF\">{baseValue.Value}</color=>";

            description = description.Replace(key, styledValue);
        }

        // Replace <keyword> tags with styled text
        description = description.Replace("<keyword>", "<color=#FFD700><u>"); // Yellow underline start
        description = description.Replace("</keyword>", "</u></color>");     // Yellow underline end

        return description;
    }

    public string ParseDynamicDescription(string description, CardInstance instance)
    {
        // Replace placeholders with dynamic values
        foreach (var baseValue in instance.Template.BaseValues)
        {
            string key = $"<{baseValue.Key}>";
            int effectiveValue = instance.GetEffectiveValue(baseValue.Key);

            // Dynamic value coloring
            string color = effectiveValue > baseValue.Value ? "#00FF00" : (effectiveValue < baseValue.Value ? "#FF0000" : "#FFFFFF"); // Green if buffed, red if debuffed, white if equal
            string styledValue = $"<color={color}>{effectiveValue}</color>";

            description = description.Replace(key, styledValue);
        }

        // Replace <keyword> tags with styled text
        description = description.Replace("<keyword>", "<color=#FFD700><u>"); // Yellow underline start
        description = description.Replace("</keyword>", "</u></color>");     // Yellow underline end

        return description;
    }
}

// Helper Classes for Deserializing JSON
[System.Serializable]
public class CardLocalizationData
{
    public string name;
    public string description;
}

[System.Serializable]
public class CardLocalizationRoot
{
    public Dictionary<string, Dictionary<string, CardLocalizationData>> data;
}
