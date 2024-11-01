using UnityEngine.UI;

public class ControlPhase : IBattlePhase
{
    public void OnEnter()
    {
        // Set up the UI for Redeploy
        BattleManager.Instance.ResetButtonTextField.text = "RESET";

        // BattleManager.Instance.RedeployButton.interactable = true;
        BattleManager.Instance.ToggleRedeployButton(true);

        // Set up the UI for Attack
        BattleManager.Instance.ToggleAttackButton(true);

        // Set up the UI for Move
        BattleManager.Instance.ResetMoveCounter();
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