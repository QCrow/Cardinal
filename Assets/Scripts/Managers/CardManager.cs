// using System.Collections.Generic;
// using System.IO;
// using Newtonsoft.Json;
// using UnityEngine;
// using System.Threading.Tasks;

// /// <summary>
// /// Manages the loading and retrieval of card data from a JSON file.
// /// Implements a singleton pattern to ensure a single instance across the application.
// /// </summary>
// public class CardManager : MonoBehaviour
// {
//     // Singleton instance of the CardManager
//     private static CardManager _instance;
//     public static CardManager Instance
//     {
//         get
//         {
//             if (_instance == null)
//             {
//                 _instance = FindObjectOfType<CardManager>();

//                 if (_instance == null)
//                 {
//                     GameObject singletonObject = new(nameof(CardManager));
//                     _instance = singletonObject.AddComponent<CardManager>();
//                     DontDestroyOnLoad(singletonObject); // Make sure the singleton object persists across scenes
//                 }
//             }
//             return _instance;
//         }
//     }

//     // Dictionary to store card data with the card ID as the key
//     private Dictionary<int, CardData> _cardDictionary = new();

//     /// <summary>
//     /// Ensures that only one instance of the CardManager exists.
//     /// Destroys any additional instances found during initialization.
//     /// </summary>
//     private void Awake()
//     {
//         if (_instance == null)
//         {
//             _instance = this;
//             DontDestroyOnLoad(gameObject); // Persist the CardManager across scenes
//         }
//         else if (_instance != this)
//         {
//             Destroy(gameObject); // Destroy duplicate instances
//         }
//     }

// #nullable enable
//     /// <summary>
//     /// Asynchronously loads card data from a JSON file into a dictionary for quick lookup.
//     /// </summary>
//     public async Task LoadCardsAsync()
//     {
//         // Construct the path to the JSON file
//         string jsonPath = Path.Combine(Application.streamingAssetsPath, "Cards", "Cards.json");

//         // Check if the file exists before attempting to read
//         if (!File.Exists(jsonPath))
//         {
//             Debug.LogError($"Card data file not found at path: {jsonPath}");
//             return;
//         }

//         // Read the JSON file asynchronously
//         string json = await Task.Run(() => File.ReadAllText(jsonPath));

//         // Deserialize the JSON data into a list of CardData objects
//         List<CardData>? cardDatas = JsonConvert.DeserializeObject<List<CardData>>(json, JsonSettingsProvider.CardJsonSerializerSettings);

//         if (cardDatas == null)
//         {
//             Debug.LogError("Failed to deserialize card data.");
//             return;
//         }

//         // Populate the dictionary with card data
//         foreach (CardData data in cardDatas)
//         {
//             if (!_cardDictionary.ContainsKey(data.CardID))
//             {
//                 _cardDictionary[data.CardID] = data;
//             }
//             else
//             {
//                 Debug.LogWarning($"Duplicate card ID {data.CardID} found. Skipping this entry.");
//             }
//         }
//     }

//     /// <summary>
//     /// Retrieves the card data associated with the specified ID.
//     /// </summary>
//     /// <param name="id">The ID of the card to retrieve.</param>
//     /// <returns>
//     /// The <see cref="CardData"/> associated with the specified ID, or <c>null</c> if the ID is not found.
//     /// </returns>
//     public CardData? GetCardDataByID(int id)
//     {
//         if (_cardDictionary.TryGetValue(id, out CardData? data))
//         {
//             return data;
//         }

//         Debug.LogError($"Card with ID {id} not found.");
//         return null;
//     }
// }