using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Represents the board in the game, responsible for initializing slots
/// and providing methods to retrieve slots based on position or offset.
/// </summary>
public class Board : MonoBehaviour
{
    private static Board _instance;
    public static Board Instance => _instance;

    #region Board Dimensions
    [SerializeField] private int _unitHeight;  // The number of rows on the board
    [SerializeField] private int _unitWidth;   // The number of columns on the board

    [SerializeField] private GameObject _slotPrefab;  // Prefab for creating slot objects
    [SerializeField] private int _slotHeight;  // Height of each slot in pixels
    [SerializeField] private int _slotWidth;   // Width of each slot in pixels
    [SerializeField] private int _slotGap;     // Gap between slots in pixels
    #endregion

    private List<List<Slot>> _slots;  // 2D list to store slot references

    private List<Card> _deployedCards;  // List of cards currently deployed on the board
    public List<Card> DeployedCards
    {
        get { return _deployedCards; }
        set { SetDeployedCards(value); }
    }
    private List<List<Card>> _cardsSnapshot = new();  // List of snapshots of the board

    private RectTransform _rectTransform;  // Used to adjust the size of the board

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        Initialize();
    }

    /// <summary>
    /// Initializes the board's size in pixels and populates the slot list by retrieving existing child slots.
    /// </summary>
    public void Initialize()
    {
        // Calculate and set the board's size in pixels based on the number of slots and gaps
        int pixelWidth = _unitWidth * _slotWidth + (_unitWidth - 1) * _slotGap;
        int pixelHeight = _unitHeight * _slotHeight + (_unitHeight - 1) * _slotGap;
        _rectTransform.sizeDelta = new Vector2(pixelWidth, pixelHeight);

        GridLayoutGroup gridLayout = GetComponent<GridLayoutGroup>();
        gridLayout.cellSize = new Vector2(_slotWidth, _slotHeight);
        gridLayout.spacing = new Vector2(_slotGap, _slotGap);

        // Initialize the list of slots
        _slots = new List<List<Slot>>();

        // Populate the board by retrieving existing slot GameObjects from the children
        int childIndex = 0;
        for (int row = 0; row < _unitHeight; row++)
        {
            List<Slot> slotRow = new List<Slot>();
            for (int col = 0; col < _unitWidth; col++)
            {
                // Get the child slot at the corresponding index
                Transform slotTransform = transform.GetChild(childIndex++);
                Slot slot = slotTransform.GetComponent<Slot>();

                if (slot != null)
                {
                    // Initialize slot with its row and column values
                    slot.Initialize(row, col);

                    // Add the slot to the current row list
                    slotRow.Add(slot);
                }
                else
                {
                    Debug.LogWarning($"Slot component missing on child at {row}, {col}");
                }
            }
            _slots.Add(slotRow);
        }
    }

    #region Slot Management
    public List<List<Slot>> GetAllSlots()
    {
        return _slots;
    }

    public List<Slot> GetRow(int row)
    {
        return _slots[row];
    }

    public List<Slot> GetColumn(int col)
    {
        return _slots.Select(row => row[col]).ToList();
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

    /// <summary>
    /// Retrieves a random empty slot from the board
    /// </summary>
    /// <returns>
    /// A random empty slot if one exists, otherwise null
    /// </returns>
    public Slot? GetRandomEmptySlot()
    {
        List<Slot> allSlots = _slots.SelectMany(row => row).ToList();
        List<Slot> emptySlots = allSlots.Where(slot => slot.Card == null).ToList();

        if (emptySlots.Count == 0)
        {
            return null;
        }

        int randomIndex = Random.Range(0, emptySlots.Count);
        return emptySlots[randomIndex];
    }

    public List<Slot> GetAdjacentSlots(Slot slot)
    {
        List<Slot> orthogonalSlots = new();

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
                orthogonalSlots.Add(neighbor);
            }
        }

        return orthogonalSlots;
    }
    #endregion

    #region Card Management
    private void SetDeployedCards(List<Card> cards)
    {
        _deployedCards = cards;
        SortDeployedCards();
    }

    private void SortDeployedCards()
    {
        _deployedCards.Sort((card1, card2) =>
        {
            int rowComparison = card1.Slot.Row.CompareTo(card2.Slot.Row);
            if (rowComparison == 0)
            {
                return card1.Slot.Col.CompareTo(card2.Slot.Col);
            }
            return rowComparison;
        });
    }

    public Card? GetCardAtPosition(int row, int col)
    {
        if (row < 0 || col < 0 || row >= _unitHeight || col >= _unitWidth)
        {
            return null;  // Return null for invalid positions
        }

        return _slots[row][col].Card;
    }

    /// <summary>
    /// Clears the board by unbinding all cards from their slots
    /// </summary>
    public void ClearBoard()
    {
        foreach (List<Slot> row in _slots)
        {
            foreach (Slot slot in row)
            {
                if (slot.Card != null)
                {
                    slot.Card.ResetTemporaryState();
                    slot.Card.UnbindFromSlot();
                }
            }
        }
    }

    public void ApplyMovement(Direction direction, int index, int magnitude)
    {
        switch (direction)
        {
            case Direction.Up:
                ShiftCardsOnColumn(index, -magnitude);
                break;
            case Direction.Down:
                ShiftCardsOnColumn(index, magnitude);
                break;
            case Direction.Left:
                ShiftCardsOnRow(index, -magnitude);
                break;
            case Direction.Right:
                ShiftCardsOnRow(index, magnitude);
                break;
            case Direction.Clockwise:
                RotateCardsClockwise();
                break;
            case Direction.CounterClockwise:
                RotateCardsCounterClockwise();
                break;
        }
    }

    public void ShiftCardsOnRow(int row, int shiftMagnitude)
    {
        Card?[] tempRow = new Card?[3];  // Temporary storage for cards in the row

        // First pass: store the new positions of the cards in the tempRow
        List<Card> cards = GetRow(row).Select(slot => slot.Card).Where(card => card != null).ToList();
        foreach (Card card in cards)
        {
            int col = card.Slot.Col;
            int newCol = (col + shiftMagnitude + 3) % 3;

            // Store the card in its new position in the tempRow
            tempRow[newCol] = card;
            card.UnbindFromSlot();  // Unbind from the current slot
        }

        // Second pass: Unbind and reattach the cards to their new slots
        for (int col = 0; col < 3; col++)
        {
            Slot? newSlot = GetSlotAtPosition(row, col);
            Card? card = tempRow[col];

            if (card != null && newSlot != null)
            {
                card.BindToSlot(newSlot);  // Bind to the new slot
                card.ApplyEffect(TriggerType.OnMove);
            }
        }
    }

    public void ShiftCardsOnColumn(int col, int shiftMagnitude)
    {
        Card?[] tempCol = new Card?[3];  // Temporary storage for cards in the column

        // First pass: store the new positions of the cards in the tempCol
        List<Card> cards = GetColumn(col).Select(slot => slot.Card).Where(card => card != null).ToList();
        foreach (Card card in cards)
        {
            int row = card.Slot.Row;
            int newRow = (row + shiftMagnitude + 3) % 3;

            // Store the card in its new position in the tempCol
            tempCol[newRow] = card;
            card.UnbindFromSlot();  // Unbind from the current slot
        }

        // Second pass: Unbind and reattach the cards to their new slots
        for (int row = 0; row < 3; row++)
        {
            Slot? newSlot = GetSlotAtPosition(row, col);
            Card? card = tempCol[row];

            if (card != null && newSlot != null)
            {
                card.BindToSlot(newSlot);  // Bind to the new slot
                card.ApplyEffect(TriggerType.OnMove);
            }
        }
    }

    public void RotateCardsClockwise()
    {
        Card?[,] tempGrid = new Card?[3, 3];  // Temporary storage for cards

        // First pass: store the new positions of the cards in the tempGrid
        List<Card> cards = _slots.SelectMany(row => row).Select(slot => slot.Card).Where(card => card != null).ToList();
        foreach (Card card in cards)
        {
            int currentRow = card.Slot.Row;
            int currentCol = card.Slot.Col;

            // Calculate the new position after 90-degree clockwise rotation
            int newRow = currentCol;
            int newCol = 2 - currentRow;

            // Store the card in its new position in the tempGrid
            tempGrid[newRow, newCol] = card;
            card.UnbindFromSlot();  // Unbind from the current slot
        }

        // Second pass: Unbind and reattach the cards to their new slots
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                Slot? newSlot = GetSlotAtPosition(row, col);
                Card? card = tempGrid[row, col];

                if (card != null && newSlot != null)
                {
                    card.BindToSlot(newSlot);  // Bind to the new slot
                    if (row != 1 || col != 1)
                    {
                        card.ApplyEffect(TriggerType.OnMove); // Apply OnMove effect for all cards except the center card
                    }
                }
            }
        }
    }

    public void RotateCardsCounterClockwise()
    {
        Card?[,] tempGrid = new Card?[3, 3];  // Temporary storage for cards

        // First pass: store the new positions of the cards in the tempGrid
        List<Card> cards = _slots.SelectMany(row => row).Select(slot => slot.Card).Where(card => card != null).ToList();
        foreach (Card card in cards)
        {
            int currentRow = card.Slot.Row;
            int currentCol = card.Slot.Col;

            // Calculate the new position after 90-degree counterclockwise rotation
            int newRow = 2 - currentCol;
            int newCol = currentRow;

            // Store the card in its new position in the tempGrid
            tempGrid[newRow, newCol] = card;
            card.UnbindFromSlot();  // Unbind from the current slot
        }

        // Second pass: Unbind and reattach the cards to their new slots
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                Slot? newSlot = GetSlotAtPosition(row, col);
                Card? card = tempGrid[row, col];

                if (card != null && newSlot != null)
                {
                    card.BindToSlot(newSlot);  // Bind to the new slot
                    if (row != 1 || col != 1)
                    {
                        card.ApplyEffect(TriggerType.OnMove); // Apply OnMove effect for all cards except the center card
                    }
                }
            }
        }
    }

    public void SaveSnapshot()
    {
        _cardsSnapshot.Clear();
        foreach (List<Slot> row in _slots)
        {
            List<Card> cards = row.Select(slot => slot.Card).ToList();
            _cardsSnapshot.Add(cards);
        }
    }

    public void RestoreFromSnapshot()
    {
        if (_cardsSnapshot.Count == 0)
        {
            return;
        }
        foreach (List<Slot> row in _slots)
        {
            foreach (Slot slot in row)
            {
                slot.Card = null;
            }
        }

        for (int row = 0; row < _unitHeight; row++)
        {
            for (int col = 0; col < _unitWidth; col++)
            {
                Slot slot = _slots[row][col];
                Card card = _cardsSnapshot[row][col];
                if (card != null)
                {
                    card.BindToSlot(slot);
                }
            }
        }
    }
    #endregion

    #region Archive
    // public void SetClusterAtPosition(int row, int col)
    // {
    //     if (row < 0 || col < 0 || row >= _unitHeight || col >= _unitWidth)
    //     {
    //         return;  // Return null for invalid positions
    //     }
    // }

    // public int GetClusterSize(int row, int col, HashSet<(int, int)> visited)
    // {
    //     // Check for out-of-bounds or invalid positions
    //     if (row < 0 || col < 0 || row >= _unitHeight || col >= _unitWidth)
    //     {
    //         return 0;
    //     }

    //     // Check if the slot has already been visited
    //     if (visited.Contains((row, col)))
    //     {
    //         return 0;
    //     }

    //     // Mark the current position as visited
    //     visited.Add((row, col));

    //     Card card = _slots[row][col].Card;
    //     if (card == null || !HasCluster(card))
    //     {
    //         return 0;
    //     }

    //     // Recursively calculate cluster size
    //     return 1
    //         + GetClusterSize(row + 1, col, visited)
    //         + GetClusterSize(row - 1, col, visited)
    //         + GetClusterSize(row, col + 1, visited)
    //         + GetClusterSize(row, col - 1, visited);
    // }

    // private bool HasCluster(Card card)
    // {
    //     // Iterate through each list of conditions in the dictionary
    //     foreach (var conditionList in card.ConditionalEffects.Values)
    //     {
    //         // Check if any condition in the list has the ConditionType of Cluster
    //         if (conditionList.Any(condition => condition is ClusterCondition))
    //         {
    //             return true;
    //         }
    //     }
    //     return false;
    // }
    #endregion

    #region Debugging
    private void OnDrawGizmos()
    {
        if (_slots == null) return;  // Return if the slots haven't been initialized yet

        Gizmos.color = Color.green;  // Set gizmo color

        for (int row = 0; row < _unitHeight; row++)
        {
            for (int col = 0; col < _unitWidth; col++)
            {
                Slot slot = _slots[row][col];
                Vector3 slotPosition = slot.transform.position;  // Get the world position of the slot
#if UNITY_EDITOR
                GUIStyle style = new();
                style.normal.textColor = Color.black;  // Set the label color to black
                style.fontStyle = FontStyle.Bold;      // Make the font bold
                style.fontSize = 16;                   // Increase the font size

                // Display the row and column as text at the slot's position with the custom style
                Handles.Label(slotPosition, $"({row}, {col})", style);
#endif
            }
        }
    }
    #endregion
}
