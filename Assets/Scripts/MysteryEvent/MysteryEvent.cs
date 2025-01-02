using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MysteryEvent
{
    public int id;                      // Unique ID for this event
    public string eventName;            // Title or short name of the event
    [TextArea] public string description;  // Story or main text

    public List<EventOption> options;   // Possible choices the player can make
}

[Serializable]
public class EventOption
{
    public string optionText;                      // Text displayed on the choice button
    public ConditionNode rootConditionNode;
    public List<BaseOutcome> outcomes;                 // Consequences if this option is chosen
}


