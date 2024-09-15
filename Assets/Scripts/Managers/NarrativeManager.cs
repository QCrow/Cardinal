using UnityEngine;
using Yarn.Unity;

public class NarrativeManager : MonoBehaviour
{
    public static NarrativeManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    [SerializeField] private DialogueRunner _dialogueRunner;

    public void StartDialogue(string dialogueNodeName)
    {
        _dialogueRunner.StartDialogue(dialogueNodeName);
    }
}