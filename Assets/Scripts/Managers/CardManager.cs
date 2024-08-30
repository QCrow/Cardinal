using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using UnityEngine;

public class CardManager : MonoBehaviour
{
    private static CardManager _instance;
    public static CardManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<CardManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(CardManager).Name);
                    _instance = singletonObject.AddComponent<CardManager>();
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }

    private Dictionary<int, CardData> _cardDictionary = new();

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    #region JSON Settings
    private class CustomContractResolver : DefaultContractResolver
    {
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (property.PropertyName == "name" || property.PropertyName == "hideFlags")
            {
                property.ShouldSerialize = instance => false;
            }
            return property;
        }
    }

    readonly JsonSerializerSettings _jsonSettings = new JsonSerializerSettings
    {
        ContractResolver = new CustomContractResolver(),
        Formatting = Formatting.Indented,
        TypeNameHandling = TypeNameHandling.Auto
    };
    #endregion

    /// <summary>
    /// Load Cards.json into look-up dictionary using card id as key.
    /// </summary>
    public void LoadCards()
    {
        string jsonPath = Path.Combine(Application.streamingAssetsPath, "Cards", "Cards.json");
        string json = File.ReadAllText(jsonPath);
        List<CardData> cardDatas = JsonConvert.DeserializeObject<List<CardData>>(json, _jsonSettings);
        foreach (CardData data in cardDatas)
        {
            _cardDictionary[data.ID] = data;
        }
    }

#nullable enable
    /// <summary>
    /// Retrieves the card data associated with the specified ID.
    /// </summary>
    /// <param name="id">The ID of the card to retrieve.</param>
    /// <returns>
    /// The <see cref="CardData"/> associated with the specified ID, or <c>null</c> if the ID is not found.
    /// </returns>
    /// <remarks>
    /// Logs an error message if the specified ID is not found in the dictionary.
    /// </remarks>
    public CardData? GetCardDataByID(int id)
    {
        if (_cardDictionary.TryGetValue(id, out CardData data))
        {
            return data;
        }

        Debug.LogError($"Card with id {id} not found.");
        return null;
    }
}
