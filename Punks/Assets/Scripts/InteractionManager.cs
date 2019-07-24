using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum InteractionType { Talk, Order, Stats, Trade}

public class InteractionManager : MonoBehaviour
{
    [Header("Main Interaction Menu")]
    [SerializeField] private GameObject interactionMenu;

    [SerializeField] private GameObject talk;
    [SerializeField] private GameObject order;
    [SerializeField] private GameObject stats;
    [SerializeField] private GameObject trade;

    private Image talkImage;
    private Image orderImage;
    private Image statsImage;
    private Image tradeImage;

    [Header("Order Menu")]
    [SerializeField] private GameObject orderMenu;

    [SerializeField] private GameObject stayHere;
    [SerializeField] private GameObject follow;
    [SerializeField] private GameObject go;
    [SerializeField] private GameObject attack;

    [SerializeField] private Color selectionColor;
    private Color baseColor;


    private List<int> selectable = new List<int>();
    private List<GameObject> subSelectable = new List<GameObject>();

    [SerializeField] private Brain[] interacting;

    [SerializeField] private int currentSelection;
    [SerializeField] private int currentSubSelection;

    private bool _isOpen;
    public bool isOpen { get { return _isOpen; } }

    private bool _isSubOpen;
    public bool isSubOpen { get { return _isSubOpen; } }

    public static InteractionManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogWarning("More than 1 instance of InteractionManager exists!");
    }

    void Start()
    {
        talkImage = talk.GetComponentInChildren<Image>();
        orderImage = order.GetComponentInChildren<Image>();
        statsImage = stats.GetComponentInChildren<Image>();
        tradeImage = trade.GetComponentInChildren<Image>();

        baseColor = talkImage.color;
    }

    void Update()
    {
        if (_isSubOpen == false)
        {
            if (Input.GetAxisRaw("Horizontal") == 1)
                SelectionChange(1);
            else if (Input.GetAxisRaw("Horizontal") == -1)
                SelectionChange(3);
            else if (Input.GetAxisRaw("Vertical") == 1)
                SelectionChange(0);
            else if (Input.GetAxisRaw("Vertical") == -1)
                SelectionChange(2);
        }
        else
        {
            if (Input.GetAxisRaw("Horizontal") == 1)
                SubSelectionChange(Mathf.Clamp(currentSubSelection--, 0, subSelectable.Count));
            else if (Input.GetAxisRaw("Horizontal") == -1)
                SubSelectionChange(Mathf.Clamp(currentSubSelection++, 0, subSelectable.Count));
        }
    }

    private void SelectionChange(int newSelection)
    {
        if (selectable.Contains(newSelection) == false)
            return;

        currentSelection = newSelection;

        talkImage.color = baseColor;
        orderImage.color = baseColor;
        statsImage.color = baseColor;
        tradeImage.color = baseColor;

        switch (currentSelection)
        {
            case 0:
                talkImage.color = selectionColor;
                break;
            case 1:
                orderImage.color = selectionColor;
                break;
            case 2:
                statsImage.color = selectionColor;
                break;
            case 3:
                tradeImage.color = selectionColor;
                break;
        }
    }

    public void Select()
    {
        if (isSubOpen == false)
        {
            switch (currentSelection)
            {
                case 0:
                    DialogueManager.instance.StartDialogue(interacting[0].actor.GetDialogue, interacting);
                    CloseInteractionMenu(false);
                    return;
                case 1:
                    OpenOrderMenu();
                    break;
                case 2:
                    StatsWindow.instance.OpenStats(interacting[0].actor);
                    CloseInteractionMenu(true);
                    break;
                case 3:
                    break;
            }
        }
        else
        {
            switch(currentSubSelection)
            {
                case 0:
                    CloseOrderMenu();
                    CloseInteractionMenu(true);
                    interacting[0].transform.GetComponent<NPCBrain>().SetState(NPCState.FOLLOW);
                    break;
                case 1:
                    CloseOrderMenu();
                    CloseInteractionMenu(true);
                    interacting[0].transform.GetComponent<NPCBrain>().SetState(NPCState.IDLE);
                    break;
            }
        }
        
    }

    public void OpenInteractionMenu(InteractionType[] options, Brain[] interactors)
    {
        interacting = interactors;

        for (int i = 0; i < interacting.Length; i++)
        {
            interactors[i].StartInteraction();
        }

        DisableAllWindows();
        options = options.Distinct().ToArray();

        for (int i = 0; i < options.Length; i++)
        {
            switch(options[i])
            {
                case InteractionType.Talk:
                    talk.SetActive(true);
                    selectable.Add(0);
                    break;
                case InteractionType.Order:
                    order.SetActive(true);
                    selectable.Add(1);
                    break;
                case InteractionType.Stats:
                    stats.SetActive(true);
                    selectable.Add(2);
                    break;
                case InteractionType.Trade:
                    trade.SetActive(true);
                    selectable.Add(3);
                    break;
            }
        }

        SelectionChange(0);

        interactionMenu.SetActive(true);
        _isOpen = true;
    }

    public void CloseInteractionMenu(bool endInteractions)
    {
        DisableAllWindows();
        interactionMenu.SetActive(false);

        if(endInteractions)
            EndAllInteractions();

        _isOpen = false;
    }

    public void OpenOrderMenu()
    {
        _isSubOpen = true;
        orderMenu.SetActive(true);
        SubSelectionChange(0);
    }

    public void CloseOrderMenu()
    {
        _isSubOpen = false;
        orderMenu.SetActive(false);
    }

    public void SubSelectionChange(int selection)
    {
        currentSubSelection = selection;
    }

    private void EndAllInteractions()
    {
        for (int i = 0; i < interacting.Length; i++)
        {
            interacting[i].EndInteraction();
        }

        interacting = null;
    }

    private void DisableAllWindows()
    {
        talk.SetActive(false);
        order.SetActive(false);
        stats.SetActive(false);
        trade.SetActive(false);

        selectable.Clear();
    }


}
