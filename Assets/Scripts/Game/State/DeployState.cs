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

            // Apply the card effect if it is a deploy trigger
            if (card.Trigger == TriggerType.OnDeploy)
            {
                card.ApplyEffect();
            }
            _cards.Add(card);
        }

        Board.Instance.DeployedCards = _cards;
    }

    private IEnumerator AnimationCoroutine()
    {
        yield return new WaitForSeconds(1f);
        GameManager.Instance.ChangeState(new ControlState());
    }
}