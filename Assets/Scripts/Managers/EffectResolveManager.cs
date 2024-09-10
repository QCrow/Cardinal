using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

/// <summary>
/// Manages the resolution of card effects within the game. This class is responsible for
/// resolving effects at different stages, such as when a turn ends or when a card is played.
/// Implements a singleton pattern to ensure a single instance across the application.
/// </summary>
public class EffectResolveManager : MonoBehaviour
{
    // Singleton instance of the EffectResolveManager
    private static EffectResolveManager _instance;
    public static EffectResolveManager Instance
    {
        get
        {
            if (_instance == null)
            {
                // Try to find an existing instance in the scene
                _instance = FindObjectOfType<EffectResolveManager>();
                if (_instance == null)
                {
                    // If no instance exists, create a new one
                    GameObject singletonObject = new(typeof(EffectResolveManager).Name);
                    _instance = singletonObject.AddComponent<EffectResolveManager>();
                    DontDestroyOnLoad(singletonObject); // Ensure the singleton persists across scenes
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject); // Persist the EffectResolveManager across scenes
        }
        else if (_instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
    }

    /// <summary>
    /// Resolves all effects that are triggered at the end of the turn.
    /// </summary>
    public void ResolveEndTurnEffects()
    {
        ResolveEffects(CardEffectTriggerType.OnTurnEnd);
    }

    /// <summary>
    /// Resolves all effects that are triggered when a card is played.
    /// </summary>
    /// <param name="card">The card that was played, triggering the effects.</param>
    public void ResolveOnPlayEffects(Card card)
    {
        ResolveEffects(card, CardEffectTriggerType.OnPlay);
        ResolveWhileInPlayEffects(card, true);
    }

    /// <summary>
    /// Resolves all effects that are triggered when a card is drawn.
    /// </summary>
    /// <param name="card">The card that was removed from game, triggering the effects.</param>
    public void ResolveOnRemoveEffects(Card card)
    {
        ResolveEffects(card, CardEffectTriggerType.OnRemove);
        ResolveWhileInPlayEffects(card, false);
    }

    /// <summary>
    /// Resolves effects for all cards on the board that match the specified trigger type.
    /// </summary>
    /// <param name="triggerType">The type of trigger that initiates the effect resolution (e.g., OnTurnEnd, OnPlay).</param>
    private void ResolveEffects(CardEffectTriggerType triggerType)
    {
        // Get all slots on the board
        List<List<Slot>> slots = Board.Instance.GetAllSlots();
        foreach (var row in slots)
        {
            foreach (var slot in row)
            {
                if (slot.Card)
                {
                    // Resolve effects for the card in this slot
                    ResolveEffects(slot.Card, triggerType);
                }
            }
        }
    }

    /// <summary>
    /// Resolves effects for a specific card based on the provided trigger type.
    /// </summary>
    /// <param name="card">The card whose effects need to be resolved.</param>
    /// <param name="triggerType">The type of trigger that initiates the effect resolution (e.g., OnTurnEnd, OnPlay).</param>
    private void ResolveEffects(Card card, CardEffectTriggerType triggerType)
    {
        // Check if the card has any conditions with effects that match the trigger type
        if (card.ConditionalEffects.TryGetValue(triggerType, out var conditions))
        {
            foreach (var condition in conditions)
            {
                if (condition is ClusterCondition clusterCondition)
                {
                    if (clusterCondition.Validate(card.Slot.Row, card.Slot.Col))
                    {
                        clusterCondition.TriggerEffects(new()); // Assuming this triggers effects on appropriate targets
                    }
                }
                else
                {
                    // Validate the condition and trigger its effects if valid
                    if (condition.Validate())
                    {
                        condition.TriggerEffects(new()); // Assuming this triggers effects on appropriate targets
                    }
                }
            }
        }
    }

    private void ResolveWhileInPlayEffects(Card card, bool inPlay)
    {

        // Check if the card has any conditions with effects that match the trigger type
        if (card.ConditionalEffects.TryGetValue(CardEffectTriggerType.WhileInPlay, out var conditions))
        {
            foreach (var condition in conditions)
            {
                if (condition is ClusterCondition clusterCondition)
                {
                    if (clusterCondition.Validate(card.Slot.Row, card.Slot.Col))
                    {
                        if (inPlay)
                        {
                            clusterCondition.TriggerEffects(new()); // Assuming this triggers effects on appropriate targets
                        }
                        else
                        {
                            clusterCondition.TriggerCounterEffects(new()); // Assuming this triggers effects on appropriate targets
                        }
                    }
                }
                else
                {
                    // Validate the condition and trigger its effects if valid
                    if (condition.Validate())
                    {
                        if (inPlay)
                        {
                            condition.TriggerEffects(new()); // Assuming this triggers effects on appropriate targets
                        }
                        else
                        {
                            condition.TriggerCounterEffects(new()); // Assuming this triggers effects on appropriate targets
                        }
                    }
                }
            }
        }
    }
}
