using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoseState : IGameState
{
    public void OnEnter(GameManager gameManager)
    {
        Debug.Log("Game Over! You ran out of attacks.");
        // TODO: Display game over screen
    }
    public void OnExit(GameManager gameManager) { }
}
