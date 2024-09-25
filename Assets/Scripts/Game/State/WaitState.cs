public class WaitState : IGameState
{
    public void OnEnter(GameManager gameManager)
    {
        UIManager.Instance.DeployButton.onClick.AddListener(OnControlButtonPressed);
    }

    public void OnExit(GameManager gameManager)
    {
        UIManager.Instance.DeployButton.onClick.RemoveListener(OnControlButtonPressed);
    }

    private void OnControlButtonPressed()
    {
        GameManager.Instance.ChangeState(new DeployState());
    }
}