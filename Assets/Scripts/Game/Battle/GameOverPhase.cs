using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverPhase : IBattlePhase
{
    public void OnEnter()
    {
        Debug.Log("Game Over! You ran out of attacks.");
        // TODO: Display game over screen
    }
    public void OnExit() { }
}
