using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Represents the board in the game, responsible for initializing slots
/// and providing methods to retrieve slots based on position or offset.
/// </summary>
public class Board : MonoBehaviour
{
    [SerializeField] private int _unitHeight;  // The number of rows on the board
    [SerializeField] private int _unitWidth;   // The number of columns on the board

    [SerializeField] private GameObject _slotPrefab;  // Prefab for creating slot objects

    [SerializeField] private int _slotHeight;  // Height of each slot in pixels
    [SerializeField] private int _slotWidth;   // Width of each slot in pixels
    [SerializeField] private int _slotGap;     // Gap between slots in pixels

    private List<List<Slot>> _slots;  // 2D list to store slot references

    private RectTransform _rectTransform;  // Used to adjust the size of the board

    private void OnEnable()
    {
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
        _slots = new List<List<Slot>>(_unitHeight);

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
}
