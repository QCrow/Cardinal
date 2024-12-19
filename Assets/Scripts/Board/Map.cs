using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

//public class Map : MonoBehaviour
//{
//    public static Map Instance { get; private set; }

//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }

//    private void Start()
//    {
//        GameManager.Instance.ChangeGameState(GameState.Battle);
//        BattleManager.Instance.StartBattleAgainstEnemy("NormalEnemy");
//    }

//    public void GoNext()
//    {
//        BattleManager.Instance.StartBattleAgainstEnemy("NormalEnemy");
//    }
//    // private MapNode[,] _mapNodes = new MapNode[3, 3];
//    // public MapNode[,] MapNodes => _mapNodes;

//    // Vector2Int _currentPosition = new(0, 0);
//    // [SerializeField] Transform _positionIndicator;

//    // [SerializeField] List<GameObject> _nodeVisuals = new();

//    // private Dictionary<MapNodeType, int> _generationSettings = new Dictionary<MapNodeType, int>
//    // {
//    //     { MapNodeType.NormalBattle, 2 },  // Ensure we have at least 2 to place one at (0, 0)
//    //     { MapNodeType.EliteBattle, 1 },
//    //     { MapNodeType.Shop, 1 },
//    //     { MapNodeType.RestSite, 1 },
//    //     { MapNodeType.Mystery, 4 },
//    // };

//    // private void Start()
//    // {
//    //     GenerateMap();
//    // }

//    // public void GenerateMap()
//    // {
//    //     // Create a list to hold all the nodes except the one at (0, 0)
//    //     List<MapNode> nodes = new List<MapNode>();

//    //     // Decrease the count of NormalBattle nodes by 1, since we'll place one at (0, 0)
//    //     _generationSettings[MapNodeType.NormalBattle]--;

//    //     // Populate the list with the remaining nodes according to the generation settings
//    //     foreach (var entry in _generationSettings)
//    //     {
//    //         for (int i = 0; i < entry.Value; i++)
//    //         {
//    //             nodes.Add(new MapNode(entry.Key));
//    //         }
//    //     }

//    //     // Shuffle the list to randomize the node distribution
//    //     Shuffle(nodes);

//    //     // Assign the battle node to (0, 0)
//    //     _mapNodes[0, 0] = new MapNode(MapNodeType.NormalBattle);
//    //     InitializeNodeVisual(0, 0, MapNodeType.NormalBattle);

//    //     // Fill the rest of the 3x3 matrix with the shuffled nodes
//    //     int index = 0;
//    //     for (int row = 0; row < 3; row++)
//    //     {
//    //         for (int col = 0; col < 3; col++)
//    //         {
//    //             if (row == 0 && col == 0) continue;

//    //             _mapNodes[row, col] = nodes[index++];
//    //             InitializeNodeVisual(row, col, _mapNodes[row, col].NodeType);
//    //         }
//    //     }

//    //     MoveTo(0, 0);
//    // }

//    // public void MoveTo(int row, int col)
//    // {
//    //     _currentPosition = new Vector2Int(row, col);

//    //     _positionIndicator.SetParent(_nodeVisuals[row * 3 + col].transform, false);
//    //     _positionIndicator.localPosition = Vector3.zero;
//    //     _positionIndicator.localScale = new Vector3(1, 1, 1);

//    //     MapNode currentNode = _mapNodes[row, col];
//    //     if (currentNode.IsVisited) return;
//    //     else currentNode.OnEnter();
//    // }

//    // // Fisher-Yates shuffle to ensure fair random distribution
//    // private void Shuffle<T>(List<T> list)
//    // {
//    //     System.Random random = new System.Random();
//    //     for (int i = list.Count - 1; i > 0; i--)
//    //     {
//    //         int j = random.Next(0, i + 1);
//    //         (list[i], list[j]) = (list[j], list[i]); // Swap elements
//    //     }
//    // }

//    // public void InitializeNodeVisual(int row, int col, MapNodeType nodeType)
//    // {
//    //     GameObject visual = _nodeVisuals[row * 3 + col];
//    //     visual.GetComponent<Image>().color = GetNodeColor(nodeType);
//    // }

//    // public Color GetNodeColor(MapNodeType nodeType)
//    // {
//    //     switch (nodeType)
//    //     {
//    //         case MapNodeType.NormalBattle:
//    //             return HexToColor("#EB886D"); // Red
//    //         case MapNodeType.EliteBattle:
//    //             return HexToColor("#EB154C"); // Yellow
//    //         case MapNodeType.Shop:
//    //             return HexToColor("#EBE215"); // Green
//    //         case MapNodeType.RestSite:
//    //             return HexToColor("#7BF5A6"); // Blue
//    //         case MapNodeType.Mystery:
//    //             return HexToColor("#AF8BF5"); // Magenta
//    //         default:
//    //             return HexToColor("#FFFFFF"); // White
//    //     }
//    // }

//    // private Color HexToColor(string hex)
//    // {
//    //     if (ColorUtility.TryParseHtmlString(hex, out Color color))
//    //     {
//    //         return color;
//    //     }
//    //     else
//    //     {
//    //         Debug.LogWarning($"Invalid hex color string: {hex}");
//    //         return Color.white;
//    //     }
//    // }

//    // public void ApplyMovement(Direction direction, int index, int magnitude)
//    // {
//    //     switch (direction)
//    //     {
//    //         case Direction.Up:
//    //             MoveOnColumn(index, -magnitude);
//    //             break;
//    //         case Direction.Down:
//    //             MoveOnColumn(index, magnitude);
//    //             break;
//    //         case Direction.Left:
//    //             MoveOnRow(index, -magnitude);
//    //             break;
//    //         case Direction.Right:
//    //             MoveOnRow(index, magnitude);
//    //             break;
//    //         case Direction.Clockwise:
//    //             MoveClockwise();
//    //             break;
//    //         case Direction.CounterClockwise:
//    //             MoveCounterClockwise();
//    //             break;
//    //     }

//    //     MoveTo(_currentPosition.x, _currentPosition.y);
//    // }

//    // private void MoveOnRow(int row, int magnitude)
//    // {
//    //     _currentPosition.y = (_currentPosition.y + magnitude + 3) % 3;
//    // }

//    // private void MoveOnColumn(int col, int magnitude)
//    // {
//    //     _currentPosition.x = (_currentPosition.x + magnitude + 3) % 3;
//    // }

//    // private void MoveClockwise()
//    // {
//    //     _currentPosition = new Vector2Int(_currentPosition.y, 2 - _currentPosition.x);
//    // }

//    // private void MoveCounterClockwise()
//    // {
//    //     _currentPosition = new Vector2Int(2 - _currentPosition.y, _currentPosition.x);
//    // }
//}
