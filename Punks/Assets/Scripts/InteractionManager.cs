using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public enum InteractionType { Talk, Order, Stats, Trade}

public class InteractionManager : MonoBehaviour
{
    [SerializeField] private GameObject interactionMenu;

    [SerializeField] private GameObject talk;
    [SerializeField] private GameObject order;
    [SerializeField] private GameObject stats;
    [SerializeField] private GameObject trade;

    [SerializeField] private Color selectionColor;
    private Color baseColor;

    private Image talkImage;
    private Image orderImage;
    private Image statsImage;
    private Image tradeImage;

    private List<GameObject> selectable = new List<GameObject>();

    [SerializeField] private Brain[] interacting;

    [SerializeField] private int currentSelection;

    private bool _isOpen;
    public bool isOpen { get { return _isOpen; } }

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
        if (Input.GetAxisRaw("Horizontal") == 1)
            SelectionChange(1);
        else if (Input.GetAxisRaw("Horizontal") == -1)
            SelectionChange(3);
        else if (Input.GetAxisRaw("Vertical") == 1)
            SelectionChange(0);
        else if (Input.GetAxisRaw("Vertical") == -1)
            SelectionChange(2);
    }

    private void SelectionChange(int newSelection)
    {
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
                    selectable.Insert(0, talk);
                    break;
                case InteractionType.Order:
                    order.SetActive(true);
                    selectable.Insert(1, order);
                    break;
                case InteractionType.Stats:
                    stats.SetActive(true);
                    selectable.Insert(2, stats);
                    break;
                case InteractionType.Trade:
                    trade.SetActive(true);
                    selectable.Insert(3, trade);
                    break;
            }
        }

        currentSelection = 0;

        interactionMenu.SetActive(true);
        _isOpen = true;
    }


    public void CloseInteractionMenu()
    {
        DisableAllWindows();
        interactionMenu.SetActive(false);

        for (int i = 0; i < interacting.Length; i++)
        {
            interacting[i].EndInteraction();
        }
        interacting = null;

        _isOpen = false;
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
