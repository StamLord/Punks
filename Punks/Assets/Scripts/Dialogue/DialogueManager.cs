using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{

    [SerializeField] private GameObject dialogueWindow;
    [SerializeField] private TextMeshProUGUI nameHolder;
    [SerializeField] private TextMeshProUGUI textHolder;

    [SerializeField] private DialogueTree currentDialogue;
    [SerializeField] private DialogueNode currentNode;

    [SerializeField]
    private bool _inDialogue;
    public bool inDialogue { get { return _inDialogue; } }

    public static DialogueManager instance;

    [SerializeField]
    private Brain[] interacting;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogWarning("More than 1 instance of DialogueManager exists!");
    }

    public void StartDialogue(DialogueTree dialogue, Brain[] interactors)
    {
        _inDialogue = true;

        interacting = interactors;

        dialogueWindow.SetActive(true);

        nameHolder.text = "[ " + interacting[0].actor.GetActorData().firstName + " " + interacting[0].actor.GetActorData().lastName + " ]";

        currentDialogue = dialogue;

        DisplayNextNode();
    }

    public void DisplayNextNode()
    {
        currentNode = currentDialogue.GetNextNode();

        if (currentNode == null)
        {
            EndDialogue();
            return;
        }

        textHolder.text = currentNode.text;

        currentDialogue.ActivateSwitches(currentNode);
    }

    public void EndDialogue()
    {
        _inDialogue = false;
        dialogueWindow.SetActive(false);
        currentDialogue.Reset();

        EndAllInteractions();
    }

    private void EndAllInteractions()
    {
        for (int i = 0; i < interacting.Length; i++)
        {
            interacting[i].EndInteraction();
        }

        interacting = null;
    }
}
