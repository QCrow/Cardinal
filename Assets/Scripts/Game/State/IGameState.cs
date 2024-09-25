using UnityEngine;

public interface IGameState
{
    void OnEnter(GameManager gameManager);
    void OnExit(GameManager gameManager);
}