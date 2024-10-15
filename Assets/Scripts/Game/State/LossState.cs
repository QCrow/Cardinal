using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LossState : IGameState
{
    public void OnEnter(GameManager gameManager)
    {
        Debug.Log("Game Over! You ran out of attacks.");
    }
    public void OnExit(GameManager gameManager) { }
}
