using System;
using System.Collections.Generic;
using UnityEngine;

public static class EffectFactory
{
    public static CardEffect CreateEffect(CardEffectData effectData)
    {
        List<string> tokens = new List<string>(effectData.Keyword.Split(' '));
        switch (tokens[0])
        {
            case "Produce":
                return new ProduceCardEffect(effectData.Values[0], tokens[1]);
            default:
                Debug.LogError("Illegal effect keyword");
                break;
        }
        throw new NotImplementedException();
    }
}