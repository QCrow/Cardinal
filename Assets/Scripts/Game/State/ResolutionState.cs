using UnityEngine;
using DG.Tweening;
using System;
using System.Collections;

public class ResolutionState : IGameState
{
    public void OnEnter(GameManager gameManager)
    {
        Debug.Log("Entering Resolution State");
        // Example coroutine call
        gameManager.StartCoroutine(CountdownCoroutine(gameManager));
    }

    public void OnExit(GameManager gameManager)
    {
        Debug.Log("Exiting Resolution State");
    }

    // TODO: Remove this as it is a just a placeholder
    private IEnumerator CountdownCoroutine(GameManager gameManager)
    {
        // Example coroutine logic
        yield return new WaitForSeconds(1);
        Debug.Log("1 second passed");
        yield return new WaitForSeconds(1);
        Debug.Log("2 seconds passed");
        yield return new WaitForSeconds(1);
        Debug.Log("3 seconds passed");
        Debug.Log("Countdown finished");
        gameManager.ChangeState(new WaitState());
    }
}