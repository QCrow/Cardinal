using UnityEngine;

public class DeployButton : MonoBehaviour
{
    public void OnClick()
    {
        if (GameManager.Instance.CurrentState is not WaitState) return;
        GameManager.Instance.ChangeState(new CombatState());
    }
}
