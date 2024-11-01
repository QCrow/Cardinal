using UnityEngine;
using UnityEngine.UI;

public class WaitPhase : IBattlePhase
{
    public void OnEnter()
    {
        Debug.Log("Entering Wait Phase");
        // Activate the deploy button, but hiding the redeploy counter and changing the text to "Deploy"
        // since the player is making the first deployment of the turn
        BattleManager.Instance.ResetButtonTextField.text = "DEPLOY";
        BattleManager.Instance.ToggleRedeployButton(false);

        BattleManager.Instance.ResetButton.interactable = true;

        // Setting the attack button to be inactive
        BattleManager.Instance.ToggleAttackButton(false);

        GameManager.Instance.CanMove = false;
    }

    public void OnExit()
    {
    }
}