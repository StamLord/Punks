using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueManager : MonoBehaviour
{

    [SerializeField] private GameObject dialogueWindow;
    [SerializeField] private TextMeshProUGUI nameHolder;
    [SerializeField] private TextMeshProUGUI textHolder;

    [SerializeField]
    private Queue<string> sentences;

    [SerializeField]
    private bool _inDialogue;
    public bool inDialogue { get { return _inDialogue; } }

    public static DialogueManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogWarning("More than 1 instance of DialogueManager exists!");
    }

    void Start()
    {
        sentences = new Queue<string>();
    }

    void Update()
    {
        
    }

    public void StartDialogue(Dialogue dialogue)
    {
        _inDialogue = true;

        dialogueWindow.SetActive(true);

        nameHolder.text = "[ " + dialogue.name + " ]";

        sentences.Clear();

        for (int i = 0; i < dialogue.sentences.Length; i++)
        {
            sentences.Enqueue(dialogue.sentences[i]);
        }

        DisplayNextSentence();
    }

    public void DisplayNextSentence()
    {
        if (sentences.Count < 1)
        {
            EndDialogue();
            return;
        }

        string sentence = sentences.Dequeue();

        textHolder.text = sentence;

    }

    public void EndDialogue()
    {
        _inDialogue = false;
        dialogueWindow.SetActive(false);
        Debug.Log("end");
    }
}
