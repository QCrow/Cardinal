using System.Collections.Generic;
using UnityEngine;

public class EffectResolveManager : MonoBehaviour
{
    private static EffectResolveManager _instance;
    public static EffectResolveManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<EffectResolveManager>();
                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(EffectResolveManager).Name);
                    _instance = singletonObject.AddComponent<EffectResolveManager>();
                    DontDestroyOnLoad(singletonObject);
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
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

    private Board _board;

    private void Start()
    {
        _board = FindObjectOfType<Board>();
        if (_board == null)
        {
            Debug.LogError("Board object not found in the scene.");
        }
    }

    public void ResolveEndTurnEffects()
    {
        List<List<Slot>> slots = _board.GetAllSlots();
        foreach (List<Slot> row in slots)
        {
            foreach (Slot slot in row)
            {
                if (slot.Card)
                {
                    if (slot.Card.ConditionsWithEffects.ContainsKey(CardEffectTriggerType.OnTurnEnd))
                    {
                        List<CardCondition> conditions = slot.Card.ConditionsWithEffects[CardEffectTriggerType.OnTurnEnd];
                        foreach (CardCondition condition in conditions)
                        {
                            if (condition.Validate())
                            {
                                condition.TriggerEffects(new());
                            }
                        }
                    }
                }
            }
        }
    }

    //TODO: Need to pass in possible targets
    public void ResolveOnPlayEffects(Card card)
    {
        if (card.ConditionsWithEffects.ContainsKey(CardEffectTriggerType.OnPlay))
        {
            List<CardCondition> conditions = card.ConditionsWithEffects[CardEffectTriggerType.OnPlay];
            foreach (CardCondition condition in conditions)
            {
                if (condition.Validate())
                {
                    condition.TriggerEffects(new());
                }
            }
        }
    }
}