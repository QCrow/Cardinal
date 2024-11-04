using UnityEngine;

public class ResetButton : MonoBehaviour
{
    public void OnClick()
    {
        BattleManager.Instance.OnResetButtonPressed();
    }
}