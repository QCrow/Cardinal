using System;
using System.Collections.Generic;
using UnityEngine;
//using Sirenix.OdinInspector; // Make sure you have this
using Sirenix.Serialization;

[Serializable]
public class MysteryEvent
{
    public int id;                      // Unique ID for this event
    public string eventName;            // Title or short name of the event
    [TextArea] public string description;  // Story or main text
    public int weight;

    public List<EventOption> options;   // Possible choices the player can make
}

[System.Serializable]
public class EventOption
{
    public string optionText;
    //[SerializeReference]
    public ConditionNode rootConditionNode;

    // Odin can serialize and display this polymorphic list
    [SerializeReference]
    public List<BaseOutcome> outcomes;
}

