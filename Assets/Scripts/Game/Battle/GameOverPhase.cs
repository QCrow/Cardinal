using UnityEngine;

public class GameOverPhase : IBattlePhase
{
    public void OnEnter()
    {
        Debug.Log("Game Over! You ran out of attacks.");
        // TODO: Display game over screen
        SceneManager.Instance.LoadScene("Game Over");
    }
    public void OnExit() { }
}
