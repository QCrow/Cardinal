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
        Debug.Log("Entering Waiting State");
    }

    public void OnExit(GameManager gameManager)
    {
        Debug.Log("Exiting Waiting State");
    }
}

public class RewardState : IGameState
{
    public void OnEnter(GameManager gameManager)
    {
        Debug.Log("Entering Select State");
    }

    public void OnExit(GameManager gameManager)
    {
        Debug.Log("Exiting Select State");
    }
}

public class PreparationState : IGameState
{
    public void OnEnter(GameManager gameManager)
    {
        Debug.Log("Entering Preparation State");
    }

    public void OnExit(GameManager gameManager)
    {
        Debug.Log("Exiting Preparation State");
    }
}
