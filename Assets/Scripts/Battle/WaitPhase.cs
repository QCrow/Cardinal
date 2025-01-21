using UnityEngine;
using UnityEngine.UI;

public class WaitPhase : IBattlePhase
{
    public void OnEnter()
    {
        BattleManager.Instance.ResetButton.interactable = false;
        BattleManager.Instance.ControlButtonTextField.text = "DEPLOY";

        GameManager.Instance.CanMove = false;
    }

    public void OnExit()
    {
    }
}