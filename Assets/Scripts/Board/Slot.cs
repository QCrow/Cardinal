using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class Slot : SerializedMonoBehaviour
{
    #region Position
    public int Row { get; private set; }
    public int Col { get; private set; }
    public void Initialize(int row, int col)
    {
        Row = row;
        Col = col;
    }
    public List<Slot> Neighbors => Board.Instance.GetAdjacentSlots(this);
    public Slot Up => Board.Instance.GetSlotAtPosition(Row - 1, Col);
    public Slot Down => Board.Instance.GetSlotAtPosition(Row + 1, Col);
    public Slot Left => Board.Instance.GetSlotAtPosition(Row, Col - 1);
    public Slot Right => Board.Instance.GetSlotAtPosition(Row, Col + 1);
    #endregion

    [SerializeField] private Card _card;
    public Card Card { get => _card; set => _card = value; }

    public bool IsPosition(PositionType position)
    {
        return position switch
        {
            PositionType.Front => Row == 0,
            PositionType.Middle => Row == 1,
            PositionType.Back => Row == 2,
            PositionType.Center => Row == 1 && Col == 1,
            _ => false,
        };
    }
}