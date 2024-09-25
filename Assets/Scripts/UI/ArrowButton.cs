using UnityEngine;
using UnityEngine.UI;

public enum Direction
{
    Up,
    Down,
    Left,
    Right,
    Clockwise,
    CounterClockwise
}

public class ArrowButton : MonoBehaviour
{
    [SerializeField] private Direction _direction;
    [SerializeField] private int _index = 0;
    [SerializeField] private int _magnitude = 1;

    private void Start()
    {
        UIManager.Instance.AddArrowButton(GetComponent<Button>());
    }

    public void OnClick()
    {
        if (GameManager.Instance.CurrentState is ControlState)
        {
            ControlState controlState = (ControlState)GameManager.Instance.CurrentState;
            controlState.ApplyMovement(_direction, _index, _magnitude);
        }
    }
}