using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerController : Brain
{
    [Header("References")]
    private new Camera camera;
    public Image healthBar;
    public TextMeshProUGUI moneyDisplay;
    public TextMeshProUGUI territoryName;
    public InteractionText interaction;
    public GameObject takeOverParent;
    public Image territoryTakeoverBar;

    //Fighting
    private List<Actor> enemies = new List<Actor>();

    //Graffiti
    public bool inGraffitiMode;
    public Cinemachine.CinemachineVirtualCamera graffitiCamera;
    public LayerMask graffitiMask;
    public LineRenderer currentGraffiti;
    public List<LineRenderer> graffiti;
    public Material graffitiMaterial;

    //Map
    public GameObject map;

    protected override void Start()
    {
        base.Start();
        camera = Camera.main;
    }

    private void Update()
    {
        FindAllInteractables();
        FindNearestInteractble();
        UpdateInteractionText();

        if (Input.GetButtonDown("Interact"))
        {
            if (DialogueManager.instance.inDialogue)
            {
                DialogueManager.instance.DisplayNextNode();
                return;
            }

            else if(InteractionManager.instance.isOpen)
            {
                InteractionManager.instance.CloseInteractionMenu();
                return;
            }

            if(nearestInteractable != null)
            {
                nearestInteractable.Interact(actor);
            }
        }

        if (Input.GetButtonDown("Jump"))
        {
            actor.Jump();
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            if (inGraffitiMode)
                StopGraffitiMode();
            else
                StartGraffitiMode();
        }

        if (inGraffitiMode)
            GraffitiMode();
        else
        {
            if (Input.GetMouseButtonDown(0))
            {
                actor.MainAttack();
            }

            else if (Input.GetMouseButtonDown(1))
            {
                actor.SecondaryAttack();
            }

        }

        if (Input.GetButtonDown("Drop"))
        {
            actor.DropWeapon();
        }

        if(Input.GetButtonDown("Map"))
        {
            map.SetActive(!map.activeSelf);
        }

        UpdateTerritoryName();
        UpdateHealthBar();
    }

    void FixedUpdate()
    {
        Vector3 input = new Vector3(
            Input.GetAxisRaw("Horizontal"),
            0,
            Input.GetAxisRaw("Vertical"));

        Vector3 flatCameraForward = Vector3.ProjectOnPlane(camera.transform.forward, Vector3.up);
        input = Quaternion.LookRotation(flatCameraForward) * input;

        input = input.normalized;

        if(Input.GetKey(KeyCode.LeftShift))
        {
            input *= 0.5f;
        }

        if (inInteraction)
            input = Vector3.zero;

        actor.Move(input);

        //Update nearby gang members of current enemies
        RemoveDeadEnemies();
        UpdateNearbyAllies();
    }

    protected override void OnDamaged(Actor attacker)
    {
        base.OnDamaged(attacker);

        if (attacker.GetActorData().gang != actor.GetActorData().gang)
            AddEnemy(attacker);
    }

    protected override void OnAttacking(Actor enemy)
    {
        base.OnAttacking(enemy);

        if (enemy.GetActorData().gang != actor.GetActorData().gang)
            AddEnemy(enemy);
    }

    public override void AddEnemy(Actor enemy)
    {
        base.AddEnemy(enemy);
        if (enemies.Contains(enemy) == false &&
            enemy != actor)
        {
            enemies.Add(enemy);
        }

    }

    private void RemoveDeadEnemies()
    {
        for (int i = enemies.Count - 1; i >= 0; i--)
        {
            if (enemies[i].isDead)
            {
                enemies.RemoveAt(i);
            }
        }
    }

    private void UpdateNearbyAllies()
    {
        if (GangManager.instance.GetGang(actor.GetActorData().gang))
        {
            List<Brain> gangMembers = FindGangMembers(10f);
            for (int i = 0; i < gangMembers.Count; i++)
            {
                for (int j = 0; j < enemies.Count; j++)
                {
                    gangMembers[i].AddEnemy(enemies[j]);
                }
            }
        }
    }

    private void UpdateTerritoryName()
    {
        Territory current = actor.GetTerritory;

        if (current)
        {
            territoryName.text = current.name;

            if (current.beingTakenOver)
            {
                takeOverParent.SetActive(true);
                territoryTakeoverBar.fillAmount = current.takeOverMeter;
            }
            else
                takeOverParent.SetActive(false);
        }
        else
        {
            territoryName.text = "";
            takeOverParent.SetActive(false);
        }
    }

    private void UpdateHealthBar()
    {
        if(healthBar)
            healthBar.fillAmount = (float)actor.GetStats.health / (float)actor.GetActorData().stats.health;
    }

    private void UpdateInteractionText()
    {
        if (nearestInteractable == null)
            interaction.ChangeTarget(null, "");
        else
            interaction.ChangeTarget(nearestInteractableObject, (InteractionManager.instance.isOpen)? "" : "[ E ] " + nearestInteractable.DisplayText());

    }

    #region Graffiti

    public void StartGraffitiMode()
    {
        graffitiCamera.Priority = 20;
        inGraffitiMode = true;

    }

    private void StartNewLine(Vector3 position, Vector3 normal)
    {
        GameObject go = new GameObject("Graffiti");
        go.transform.position = position;
        go.transform.forward = normal;
        currentGraffiti = go.AddComponent<LineRenderer>();
        currentGraffiti.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        currentGraffiti.SetPosition(0, position);
        currentGraffiti.SetPosition(1, position);
        currentGraffiti.startWidth = 0.05f;
        currentGraffiti.endWidth = 0.03f;
        currentGraffiti.startColor = Color.red;
        currentGraffiti.endColor = Color.red;
        currentGraffiti.material = graffitiMaterial;

        graffiti.Add(currentGraffiti);
    }

    private void GraffitiMode()
    {
        RaycastHit hit;
        bool hitWall = Physics.Raycast(camera.ScreenPointToRay(Input.mousePosition), out hit, 5f, graffitiMask);

        Vector3 mouseWorldPos = hit.point;

        if (Input.GetMouseButtonDown(0) && hitWall)
            StartNewLine(mouseWorldPos, hit.normal);

        if(Input.GetMouseButton(0) && hitWall)
        {
            if (currentGraffiti)
            {
                currentGraffiti.positionCount = currentGraffiti.positionCount+1;
                currentGraffiti.SetPosition(currentGraffiti.positionCount -1, mouseWorldPos);
            }
        }

    }

    public void StopGraffitiMode()
    {
        graffitiCamera.Priority = 0;
        inGraffitiMode = false;
        currentGraffiti = null;
    }

    #endregion
}
