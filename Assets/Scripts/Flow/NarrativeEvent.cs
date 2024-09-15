using UnityEngine;

public class NarrativeEvent : IEvent
{
    public string DialogueNodeName;
    public void Execute()
    {
        NarrativeManager.Instance.StartDialogue(DialogueNodeName);
    }
}