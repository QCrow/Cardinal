using UnityEngine;

public class DeployButton : MonoBehaviour
{
    private void OnEnable()
    {
        if (GameManager.Instance == null)
        {
            throw new System.Exception("GameManager not found in scene.");
        }
        GameManager.Instance.OnStateChange.AddListener(HandleGameStateChanged);
    }

    private void OnDisable()
    {
        GameManager.Instance.OnStateChange.RemoveListener(HandleGameStateChanged);
    }

    private void HandleGameStateChanged(IGameState previousState, IGameState newState)
    {
        if (newState is WaitState)
        {
            GetComponent<UnityEngine.UI.Button>().interactable = true;
        }
    }

    public void OnClick()
    {
        GameManager.Instance.ChangeState(new ResolutionState());
        GetComponent<UnityEngine.UI.Button>().interactable = false;
    }
}