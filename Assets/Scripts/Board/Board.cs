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

    private List<CardView> _deployedCards;  // List of cards currently deployed on the board
    public List<CardView> DeployedCards
    {
        get { return _deployedCards; }
        set { SetDeployedCards(value); }
    }
    private List<CardSaveData> _cardsSnapshot = new();  // List of snapshots of the board
    private List<SlotSaveData> _slotsSnapshot = new();  // List of snapshots of the slots

    private RectTransform _rectTransform;  // Used to adjust the size of the board

    public int SlotSeed;

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            _rectTransform = GetComponent<RectTransform>();
            Initialize();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Initializes the board's size in pixels and populates the slot list by retrieving existing child slots.
    /// </summary>
    public void Initialize()
    {
        // // Calculate and set the board's size in pixels based on the number of slots and gaps
        // int pixelWidth = _unitWidth * _slotWidth + (_unitWidth - 1) * _slotGap;
        // int pixelHeight = _unitHeight * _slotHeight + (_unitHeight - 1) * _slotGap;
        // _rectTransform.sizeDelta = new Vector2(pixelWidth, pixelHeight);

        // GridLayoutGroup gridLayout = GetComponent<GridLayoutGroup>();
        // gridLayout.cellSize = new Vector2(_slotWidth, _slotHeight);
        // gridLayout.spacing = new Vector2(_slotGap, _slotGap);

        SlotSeed = GameManager.Instance.seed;
        // Initialize the list of slots
        _slots = new List<List<Slot>>();

        // Populate the board by retrieving existing slot GameObjects from the children
        int childIndex = 0;
        for (int row = 0; row < _unitHeight; row++)
        {
            List<Slot> slotRow = new();
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
        //int SlotRandomSeed = GameManager.Instance.seed
        SlotSeed = GameManager.Instance.GetDerivedSeedWithPosition(SlotSeed, 524287, 65537);
        Random.InitState(SlotSeed);
        List<Slot> allSlots = _slots.SelectMany(row => row).ToList();
        List<Slot> emptySlots = allSlots.Where(slot => slot.Content == null).ToList();

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
    private void SetDeployedCards(List<CardView> cards)
    {
        _deployedCards = cards;
        SortDeployedCards();
    }

    public void UpdateDeployedCards()
    {
        List<Slot> allSlots = _slots.SelectMany(row => row).ToList();
        List<CardView> cards = allSlots.Select(slot => slot.Content as CardView).Where(card => card != null).Select(card => card!).ToList();
        _deployedCards = cards;
        SortDeployedCards();
    }

    private void SortDeployedCards()
    {
        _deployedCards.Sort((card1, card2) =>
        {
            int rowComparison = card1.CurrentSlot.Row.CompareTo(card2.CurrentSlot.Row);
            if (rowComparison == 0)
            {
                return card1.CurrentSlot.Col.CompareTo(card2.CurrentSlot.Col);
            }
            return rowComparison;
        });
    }

    public SlotContent? GetContentAtPosition(int row, int col)
    {
        if (row < 0 || col < 0 || row >= _unitHeight || col >= _unitWidth)
        {
            return null;  // Return null for invalid positions
        }

        return _slots[row][col].Content;
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
                if (slot.Content != null)
                {
                    slot.Content.UnbindFromSlot();
                }
            }
        }
    }

    public void ClearBoardSlotsModifiers(ModifierPersistenceType persistenceType)
    {
        foreach (List<Slot> row in _slots)
        {
            foreach (Slot slot in row)
            {
                slot.ClearModifiers(persistenceType);
            }
        }
    }

    public void ApplyMovement(Direction direction, int index, int magnitude)
    {
        switch (direction)
        {
            case Direction.Up:
                ShiftSlotContentsOnColumn(index, -magnitude);
                break;
            case Direction.Down:
                ShiftSlotContentsOnColumn(index, magnitude);
                break;
            case Direction.Left:
                ShiftSlotContentsOnRow(index, -magnitude);
                break;
            case Direction.Right:
                ShiftSlotContentsOnRow(index, magnitude);
                break;
            case Direction.Clockwise:
                RotateSlotContentsClockwise();
                break;
            case Direction.CounterClockwise:
                RotateSlotContentsCounterClockwise();
                break;
        }
    }

    public void ShiftSlotContentsOnRow(int row, int shiftMagnitude)
    {
        SlotContent?[] tempRow = new SlotContent?[3];  // Temporary storage for cards in the row

        // First pass: store the new positions of the cards in the tempRow
        List<SlotContent> contents = GetRow(row).Select(slot => slot.Content).Where(card => card != null).ToList();
        foreach (SlotContent content in contents)
        {
            int col = content.CurrentSlot.Col;
            int newCol = (col + shiftMagnitude + 3) % 3;

            // Store the card in its new position in the tempRow
            tempRow[newCol] = content;
            content.UnbindFromSlot();  // Unbind from the current slot
        }

        // Second pass: Unbind and reattach the cards to their new slots
        for (int col = 0; col < 3; col++)
        {
            Slot? newSlot = GetSlotAtPosition(row, col);
            SlotContent? content = tempRow[col];

            if (content != null && newSlot != null)
            {
                if (content is CardView)
                {
                    CardView card = (CardView)content;
                    card.BindToSlot(newSlot);  // Bind to the new slot
                    card.MoveToAndSetParent(newSlot.ContentContainer.GetComponent<RectTransform>(), () => card.ApplyEffect(TriggerType.OnMove));
                }
                else
                {
                    content.BindToSlot(newSlot);
                    content.MoveToAndSetParent(newSlot.ContentContainer.GetComponent<RectTransform>());
                }
            }
        }
    }

    public void ShiftSlotContentsOnColumn(int col, int shiftMagnitude)
    {
        SlotContent?[] tempCol = new SlotContent?[3];  // Temporary storage for cards in the column

        // First pass: store the new positions of the cards in the tempCol
        List<SlotContent> contents = GetColumn(col).Select(slot => slot.Content).Where(card => card != null).ToList();
        foreach (SlotContent content in contents)
        {
            int row = content.CurrentSlot.Row;
            int newRow = (row + shiftMagnitude + 3) % 3;

            // Store the card in its new position in the tempCol
            tempCol[newRow] = content;
            content.UnbindFromSlot();  // Unbind from the current slot
        }

        // Second pass: Unbind and reattach the cards to their new slots
        for (int row = 0; row < 3; row++)
        {
            Slot? newSlot = GetSlotAtPosition(row, col);
            SlotContent? content = tempCol[row];

            if (content != null && newSlot != null)
            {
                if (content is CardView)
                {
                    CardView card = (CardView)content;
                    card.BindToSlot(newSlot);  // Bind to the new slot
                    card.MoveToAndSetParent(newSlot.ContentContainer.GetComponent<RectTransform>(), () => card.ApplyEffect(TriggerType.OnMove));
                }
                else
                {
                    content.BindToSlot(newSlot);
                    content.MoveToAndSetParent(newSlot.ContentContainer.GetComponent<RectTransform>());
                }
            }
        }
    }

    public void RotateSlotContentsClockwise()
    {
        SlotContent?[,] tempGrid = new SlotContent?[3, 3];  // Temporary storage for cards

        // First pass: store the new positions of the cards in the tempGrid
        List<SlotContent> cards = _slots.SelectMany(row => row).Select(slot => slot.Content).Where(card => card != null).ToList();
        foreach (SlotContent card in cards)
        {
            int currentRow = card.CurrentSlot.Row;
            int currentCol = card.CurrentSlot.Col;

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
                SlotContent? content = tempGrid[row, col];

                if (content != null && newSlot != null)
                {
                    if (content is CardView)
                    {
                        CardView card = (CardView)content;
                        card.BindToSlot(newSlot);  // Bind to the new slot
                        if (row != 1 || col != 1)
                        {
                            card.MoveToAndSetParent(newSlot.ContentContainer.GetComponent<RectTransform>(), () => card.ApplyEffect(TriggerType.OnMove));
                        }
                    }
                    else
                    {
                        content.BindToSlot(newSlot);
                        content.MoveToAndSetParent(newSlot.ContentContainer.GetComponent<RectTransform>());
                    }
                }
            }
        }
    }

    public void RotateSlotContentsCounterClockwise()
    {
        SlotContent?[,] tempGrid = new SlotContent?[3, 3];  // Temporary storage for cards

        // First pass: store the new positions of the cards in the tempGrid
        List<SlotContent> contents = _slots.SelectMany(row => row).Select(slot => slot.Content).Where(card => card != null).ToList();

        foreach (SlotContent content in contents)
        {
            int currentRow = content.CurrentSlot.Row;
            int currentCol = content.CurrentSlot.Col;

            // Calculate the new position after 90-degree counterclockwise rotation
            int newRow = 2 - currentCol;
            int newCol = currentRow;

            // Store the card in its new position in the tempGrid
            tempGrid[newRow, newCol] = content;
            content.UnbindFromSlot();  // Unbind from the current slot
        }

        // Second pass: Unbind and reattach the cards to their new slots
        for (int row = 0; row < 3; row++)
        {
            for (int col = 0; col < 3; col++)
            {
                Slot? newSlot = GetSlotAtPosition(row, col);
                SlotContent? content = tempGrid[row, col];

                if (content != null && newSlot != null)
                {
                    if (content is CardView)
                    {
                        CardView card = (CardView)content;
                        card.BindToSlot(newSlot);  // Bind to the new slot
                        if (row != 1 || col != 1)
                        {
                            card.MoveToAndSetParent(newSlot.ContentContainer.GetComponent<RectTransform>(), () => card.ApplyEffect(TriggerType.OnMove));
                        }
                    }
                    else
                    {
                        content.BindToSlot(newSlot);
                        content.MoveToAndSetParent(newSlot.ContentContainer.GetComponent<RectTransform>());
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
            List<CardView> cards = row.Select(slot => slot.Content as CardView).Where(card => card != null).Select(card => card!).ToList();
            foreach (CardView card in cards)
            {
                _cardsSnapshot.Add(card.GetSaveData());
            }
            foreach (Slot slot in row)
            {
                _slotsSnapshot.Add(slot.GetSaveData());
            }
        }
    }

    public void RestoreFromSnapshot()
    {
        foreach (List<Slot> row in _slots)
        {
            foreach (Slot slot in row)
            {
                slot.Content = null;
                slot.LoadSaveData(_slotsSnapshot.FirstOrDefault(s => s.Row == slot.Row && s.Col == slot.Col));
            }
        }

        foreach (CardSaveData cardData in _cardsSnapshot)
        {
            CardView card = DeployedCards.FirstOrDefault(c => c.gameObject.GetInstanceID() == cardData.GUID);
            if (card != null)
            {
                Slot? slot = GetSlotAtPosition(cardData.Row, cardData.Col);
                if (slot != null)
                {
                    card.LoadFromSaveData(cardData);
                    card.BindToSlot(slot);
                    card.MoveToAndSetParent(slot.ContentContainer.GetComponent<RectTransform>());
                }
            }
        }
    }
    #endregion
}
