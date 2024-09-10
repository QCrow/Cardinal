using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Represents the board in the game, responsible for initializing slots
/// and providing methods to retrieve slots based on position or offset.
/// </summary>
public class Board : MonoBehaviour
{
    private static Board _instance;
    public static Board Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<Board>();
                if (_instance == null)
                {
                    Debug.LogError("Board object not found in the scene.");
                }
            }
            return _instance;
        }
    }

    [SerializeField] private int _unitHeight;  // The number of rows on the board
    [SerializeField] private int _unitWidth;   // The number of columns on the board

    [SerializeField] private GameObject _slotPrefab;  // Prefab for creating slot objects

    [SerializeField] private int _slotHeight;  // Height of each slot in pixels
    [SerializeField] private int _slotWidth;   // Width of each slot in pixels
    [SerializeField] private int _slotGap;     // Gap between slots in pixels

    private List<List<Slot>> _slots;  // 2D list to store slot references

    private RectTransform _rectTransform;  // Used to adjust the size of the board

    private void OnAwake()
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

    private void OnEnable()
    {
        if (_slotPrefab == null)
        {
            Debug.LogError("Slot prefab is not assigned in the inspector!");
            return;
        }

        _rectTransform = GetComponent<RectTransform>();
        Initialize();
    }


    /// <summary>
    /// Initializes the board's size in pixel and generates slots based on the board's dimensions.
    /// </summary>
    public void Initialize()
    {
        // Calculate and set the board's size in pixels based on the number of slots and gaps
        int pixelWidth = _unitWidth * _slotWidth + (_unitWidth - 1) * _slotGap;
        int pixelHeight = _unitHeight * _slotHeight + (_unitHeight - 1) * _slotGap;
        _rectTransform.sizeDelta = new Vector2(pixelWidth, pixelHeight);

        // Initialize the list of slots
        _slots = new();
        // Populate the board with slots, arranging them in a grid
        for (int row = 0; row < _unitHeight; row++)
        {
            List<Slot> slotRow = new();
            for (int col = 0; col < _unitWidth; col++)
            {
                // Instantiate and name each slot based on its position in the grid
                GameObject newSlot = Instantiate(_slotPrefab, transform);
                newSlot.name = $"Slot {row} {col}";

                // Set the slot's row and column values and add it to the row list
                Slot slotScript = newSlot.GetComponent<Slot>();
                slotScript.Row = row;
                slotScript.Col = col;

                slotRow.Add(slotScript);
            }
            _slots.Add(slotRow);
        }
    }

    public List<List<Slot>> GetAllSlots()
    {
        return _slots;
    }

#nullable enable
    /// <summary>
    /// Retrieves the slot at the specified row and column position
    /// Returns null if the position is out of bounds
    /// </summary>
    public Slot? GetSlotAtPosition(int row, int col)
    {
        if (row < 0 || col < 0 || row >= _unitHeight || col >= _unitWidth)
        {
            return null;  // Return null for invalid positions
        }

        return _slots[row][col];
    }

    /// <summary>
    /// Retrieves the slot relative to a given position based on offsets
    /// Useful for finding neighboring slots
    /// </summary>
    public Slot? GetSlotByOffset(int row, int col, int rowOffset, int colOffset)
    {
        return GetSlotAtPosition(row + rowOffset, col + colOffset);
    }

    public Card? GetCardAtPosition(int row, int col)
    {
        if (row < 0 || col < 0 || row >= _unitHeight || col >= _unitWidth)
        {
            return null;  // Return null for invalid positions
        }

        return _slots[row][col].Card;
    }

    public List<Slot> GetNeighbors(Slot slot)
    {
        List<Slot> neighbors = new();

        // Define the row and column offsets for the 4 directions
        int[] rowOffsets = { -1, 1, 0, 0 };
        int[] colOffsets = { 0, 0, -1, 1 };

        for (int i = 0; i < rowOffsets.Length; i++)
        {
            int rowOffset = rowOffsets[i];
            int colOffset = colOffsets[i];

            Slot? neighbor = GetSlotByOffset(slot.Row, slot.Col, rowOffset, colOffset);
            if (neighbor != null)
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

    public void SetClusterAtPosition(int row, int col)
    {
        if (row < 0 || col < 0 || row >= _unitHeight || col >= _unitWidth)
        {
            return;  // Return null for invalid positions
        }
    }

    public int GetClusterSize(int row, int col, HashSet<(int, int)> visited)
    {
        // Check for out-of-bounds or invalid positions
        if (row < 0 || col < 0 || row >= _unitHeight || col >= _unitWidth)
        {
            return 0;
        }

        // Check if the slot has already been visited
        if (visited.Contains((row, col)))
        {
            return 0;
        }

        // Mark the current position as visited
        visited.Add((row, col));

        Card card = _slots[row][col].Card;
        if (card == null || !HasCluster(card))
        {
            return 0;
        }

        // Recursively calculate cluster size
        return 1
            + GetClusterSize(row + 1, col, visited)
            + GetClusterSize(row - 1, col, visited)
            + GetClusterSize(row, col + 1, visited)
            + GetClusterSize(row, col - 1, visited);
    }

    private bool HasCluster(Card card)
    {
        // Iterate through each list of conditions in the dictionary
        foreach (var conditionList in card.ConditionalEffects.Values)
        {
            // Check if any condition in the list has the ConditionType of Cluster
            if (conditionList.Any(condition => condition is ClusterCondition))
            {
                return true;
            }
        }
        return false;
    }
}
