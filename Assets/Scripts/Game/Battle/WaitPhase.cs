using UnityEngine.UI;

public class WaitPhase : IBattlePhase
{
    public void OnEnter()
    {
        // Activate the deploy button, but hiding the redeploy counter and changing the text to "Deploy"
        // since the player is making the first deployment of the turn
        BattleManager.Instance.DeployButtonTextField.text = "DEPLOY";
        BattleManager.Instance.DeployButton.interactable = true;
        BattleManager.Instance.DeployButton.GetComponent<Image>().sprite = BattleManager.Instance.ButtonWithoutCounterSprite;
        BattleManager.Instance.RedeployCounterTextField.gameObject.SetActive(false);

        // Setting the attack button to be inactive
        BattleManager.Instance.AttackButton.interactable = false;

        GameManager.Instance.CanMove = false;
    }

    public void OnExit()
    {
    }
}