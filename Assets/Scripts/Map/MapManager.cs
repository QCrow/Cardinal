using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Map
{
    public class MapManager : MonoBehaviour
    {
        public static MapManager Instance { get; private set; }
        public List<MapConfig> configs;
        public MapView view;

        public Map CurrentMap { get; private set; }
        public Node CurrentNode { get; private set; } // Track the current node

        private void Start()
        {
            LoadMap();
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        public void LoadMap(){
            // if (PlayerPrefs.HasKey("Map"))
            // {
            //     string mapJson = PlayerPrefs.GetString("Map");
            //     Map map = JsonConvert.DeserializeObject<Map>(mapJson);
            //     // using this instead of .Contains()
            //     if (map.path.Any(p => p.Equals(map.GetBossNode().point)))
            //     {
            //         // payer has already reached the boss, generate a new map
            //         GenerateNewMap();
            //     }
            //     else
            //     {
            //         CurrentMap = map;
            //         // player has not reached the boss yet, load the current map
            //         view.ShowMap(map);
            //     }
            // }
            // else
            // {
            //     GenerateNewMap();
            // }
            GenerateNewMap(GameManager.Instance.CurrentLevel, GameManager.Instance.seed);
            CurrentNode = CurrentMap.GetNode(CurrentMap.path.LastOrDefault()); // Initialize current node
        }

        public void GenerateNewMap(int level, int seed)
        {
            Map map = MapGenerator.GetMap(configs[level-1], seed);
            CurrentMap = map;
            //Debug.Log(map.lastNodeName);
            Debug.Log(map.ToJson());
            view.ShowMap(map);
        }

        public void SaveMap()
        {
            if (CurrentMap == null) return;

            string json = JsonConvert.SerializeObject(CurrentMap, Formatting.Indented,
                new JsonSerializerSettings { ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
            PlayerPrefs.SetString("Map", json);
            PlayerPrefs.Save();
        }

        public bool IsCurrentNodeLast()
        {
            return MapGenerator.IsLastNode(CurrentNode);
        }

        public void SetCurrentNode(Node node)
        {
            CurrentNode = node;
        }

        private void OnApplicationQuit()
        {
            SaveMap();
        }
    }
}
