using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeployState : IGameState
{
    public void OnEnter(GameManager gameManager)
    {
        Board.Instance.ClearBoard();
        CardManager.Instance.Reshuffle();

        Deploy();
        Board.Instance.SaveSnapshot();

        GameManager.Instance.ChangeState(new ControlState());
    }

    public void OnExit(GameManager gameManager)
    {
    }

    private void Deploy()
    {
        List<Card> _cards = new();

        // Place cards randomly on the board
        while (true)
        {
            Slot slot = Board.Instance.GetRandomEmptySlot();
            if (slot == null) break;

            // Instantiate the card and bind it to the slot
            Card card = CardManager.Instance.DrawCard();
            if (card == null) break;

            card.ResetTemporaryState();
            card.BindToSlot(slot);

            // Apply the deploy trigger effects
            card.ApplyEffect(TriggerType.OnDeploy);
            _cards.Add(card);
        }

        Board.Instance.DeployedCards = _cards;
    }
}