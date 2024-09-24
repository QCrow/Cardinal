using UnityEngine;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;

public class CombatState : IGameState
{
    private List<Card> _cards = new();

    public void OnEnter(GameManager gameManager)
    {
        // Before entering the combat state, clear the board
        Board.Instance.ClearBoard();
        CardManager.Instance.Reshuffle();

        Deploy();
        AfterDeploy();
        Attack();
        GameManager.Instance.ChangeState(new WaitState());
    }

    public void OnExit(GameManager gameManager)
    {
    }

    private void Deploy()
    {
        // Place cards randomly on the board
        while (true)
        {
            Slot slot = Board.Instance.GetRandomEmptySlot();
            if (slot == null) break;
            // Instantiate the card and bind it to the slot
            Card card = CardManager.Instance.DrawCard();
            if (card == null) break;
            card.Reset();
            card.BindToSlot(slot);
            // Debug.Log(card.Name + " is deployed to " + slot.Row + ", " + slot.Col);
            // Apply the card effect if it is a deploy trigger
            if (card.Trigger == TriggerType.OnDeploy)
            {
                card.ApplyEffect();
            }
            _cards.Add(card);
        }

        _cards.Sort((card1, card2) =>
        {
            int rowComparison = card1.Slot.Row.CompareTo(card2.Slot.Row);
            if (rowComparison == 0)
            {
                return card1.Slot.Col.CompareTo(card2.Slot.Col);
            }
            return rowComparison;
        });
    }

    private void AfterDeploy()
    {
        _cards.ForEach(card =>
        {
            if (card.Trigger == TriggerType.AfterDeploy)
            {
                Debug.Log("Applying AfterDeploy effect for " + card.Name);
                card.ApplyEffect();
            }
        });
    }

    private void Attack()
    {
        _cards.ForEach(card =>
        {
            if (card.Trigger == TriggerType.OnAttack)
            {
                card.ApplyEffect();
            }
            GameManager.Instance.AttackMonster(card.TotalAttack);
        });
    }
}