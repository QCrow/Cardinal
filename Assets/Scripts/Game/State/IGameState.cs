using UnityEngine;

public interface IGameState
{
    void OnEnter(GameManager gameManager);
    void OnExit(GameManager gameManager);
}

public class WaitState : IGameState
{
    public void OnEnter(GameManager gameManager)
    {
    }

    public void OnExit(GameManager gameManager)
    {
    }
}

public class RewardState : IGameState
{
    public void OnEnter(GameManager gameManager)
    {
    }

    public void OnExit(GameManager gameManager)
    {
    }
}

public class PreparationState : IGameState
{
    public void OnEnter(GameManager gameManager)
    {
    }

    public void OnExit(GameManager gameManager)
    {
    }
}
