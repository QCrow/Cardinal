public class ControlPhase : IBattlePhase
{
    public void OnEnter()
    {
        // Set up the UI for Redeploy
        BattleManager.Instance.DeployButtonTextField.text = "Redeploy";
        BattleManager.Instance.ResetRedeployCounter();
        BattleManager.Instance.RedeployCounter.SetActive(true);

        // Set up the UI for Attack
        BattleManager.Instance.AttackButton.interactable = true;

        // Set up the UI for Move
        BattleManager.Instance.ResetMoveCounter();
        BattleManager.Instance.ResetButton.interactable = true;
        GameManager.Instance.CanMove = true;
    }

    public void OnExit()
    {
        BattleManager.Instance.ResetButton.interactable = false;
    }

    public void ApplyMovement(Direction direction, int index, int magnitude)
    {
        BattleManager.Instance.RevertWhileInPlayEffects();
        BattleManager.Instance.DecrementMoveCounter();
        Board.Instance.ApplyMovement(direction, index, magnitude);
        BattleManager.Instance.ApplyWhileInPlayEffects();
        BattleManager.Instance.SetTotalAttack();
    }
}