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

    private void Awake()
    {
        Button button = GetComponent<Button>();
        Debug.Assert(button != null, "Button component not found on ArrowButton object.");

        button.onClick.RemoveListener(OnClick);
        button.onClick.AddListener(OnClick);
    }

    private void Start()
    {
        UIManager.Instance.AddArrowButton(GetComponent<Button>());
    }

    public void OnClick()
    {
        // TODO: Reduce coupling by using events
        if (GameManager.Instance.CurrentGameState == GameState.Map)
        {
            Map.Instance.ApplyMovement(_direction, _index, _magnitude);
        }
        if (BattleManager.Instance.CurrentBattlePhase is ControlPhase phase)
        {
            phase.ApplyMovement(_direction, _index, _magnitude);
        }
    }
}