using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class StatsWindow : MonoBehaviour
{
    [SerializeField] private Vector3 offset = new Vector3(1.3f, 1f, 0f);

    [SerializeField] private GameObject parent;

    [SerializeField] private new TextMeshProUGUI name;
    [SerializeField] private TextMeshProUGUI hp;
    [SerializeField] private TextMeshProUGUI atk;
    [SerializeField] private TextMeshProUGUI def;
    [SerializeField] private TextMeshProUGUI spd;

    [SerializeField] private Image atkProgress;
    [SerializeField] private Image defProgress;
    [SerializeField] private Image spdProgress;

    private Camera camera;

    private bool _isOpen;
    public bool isOpen { get { return _isOpen; } }

    public static StatsWindow instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogWarning("More than 1 instance of StatsWindow exists!");
    }

    void Start()
    {
        camera = Camera.main;
    }

    void Update()
    {
        
    }

    public void OpenStats(Actor actor)
    {
        parent.SetActive(true);

        ActorData data = actor.GetActorData();

        name.text = data.firstName + " " + data.lastName;

        hp.text = actor.GetStats.health.ToString();
        atk.text = actor.GetStats.attack.ToString();
        def.text = actor.GetStats.defense.ToString();
        spd.text = actor.GetStats.defense.ToString();

        atkProgress.fillAmount = actor.GetStats.attackProgress;
        defProgress.fillAmount = actor.GetStats.defenseProgress;
        spdProgress.fillAmount = actor.GetStats.speedProgress;

        _isOpen = true;
    }

    public void CloseStats()
    {
        parent.SetActive(false);
        _isOpen = false;
    }
}
