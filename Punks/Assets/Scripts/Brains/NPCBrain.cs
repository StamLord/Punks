using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum NPCState { IDLE, ROAM, SCHEDULE, FOLLOW, FIGHT, FLEE}

[RequireComponent(typeof(NavMeshAgent))]
public class NPCBrain : Brain, IInteractable
{
    private NavMeshAgent agent;
    private LineRenderer lr;

    private bool isSpawned;

    [SerializeField] private NPCState state;
    [SerializeField] private NPCState lastState;

    //Pathfinding
    private float lastCalculation;
    private Queue<Vector3> path = new Queue<Vector3>();
    [SerializeField] private Transform goal;
    [SerializeField] private Transform lastGoal;

    //Schedule
    [SerializeField] private Routine todaysRoutine;

    //Roaming
    private Transform[] roamPath;
    private int currentRoamPoint;

    //Fighting
    private List<Actor> enemies = new List<Actor>();

    protected override void Start()
    {
        base.Start();
        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        lr = GetComponent<LineRenderer>();

        FindTodaysRoutine();
    }

    private void Update()
    {
        if(actor.isDead)
        {
            SetState(NPCState.IDLE);
        }
        else if (state != NPCState.FIGHT &&
            enemies.Count > 0)
            SetState(NPCState.FIGHT);

        switch (state)
        {
            case NPCState.IDLE:
                break;
            case NPCState.ROAM:
                Roam();
                break;
            case NPCState.SCHEDULE:
                Schedule();
                break;
            case NPCState.FOLLOW:
                if (goal)
                    UpdatePath();
                break;
            case NPCState.FIGHT:
                Fight();
                break;
            case NPCState.FLEE:
                break;
        }
    }

    private void UpdatePath()
    {
        if (Vector3.Distance(transform.position, goal.position) > agent.stoppingDistance)
        {
            agent.SetDestination(goal.position);
            path = new Queue<Vector3>(agent.path.corners);

            lastCalculation = Time.time;
        }

        DrawPath(path);
    }

    private void DrawPath(Queue<Vector3> path)
    {
        Vector3[] array = path.ToArray();

        if(path != null)
        {
            lr.positionCount = array.Length;
            for (int i = 0; i < array.Length; i++)
            {
                lr.SetPosition(i, array[i]);
            }
        }
    }

    private void LateUpdate()
    {
        if (schedule != null && isSpawned == false)
        {
            actor.Move(Vector3.zero);
            return;
        }

        if(path.Count > 0 && Vector3.Distance(transform.position, path.Peek()) <= agent.stoppingDistance)
        {
            path.Dequeue();
        }

        if (path.Count > 0 && state != NPCState.IDLE)
        {
            Vector3 direction = (path.Peek() - transform.position).normalized;
            direction.y = 0;

            float distance = agent.remainingDistance;
            if (state == NPCState.ROAM || state == NPCState.SCHEDULE)
                direction *= .3f;
            else if (state == NPCState.FOLLOW && distance < 8f)
            {
                direction *= Mathf.Clamp(1f / (8f / distance), .3f, 1f);
            }

            actor.Move(direction);
        }
        else
        {
            actor.Move(Vector3.zero);
        }
    }

    public void SetRoamPath(Transform[] path)
    {
        roamPath = path;
    }

    private void Schedule()
    {
        if (schedule == null)
            return;


        for (int i = 0; i < todaysRoutine._actions.Count; i++)
        {
            //Find first not done
            if(todaysRoutine._actions[i].done == false)
            {
                HourMinutes time = new HourMinutes(todaysRoutine._actions[i].time);

                //Check if time to start
                if(DayNightCycle.instance.GetTime().hours >= time.hours &&
                    DayNightCycle.instance.GetTime().minutes >= time.minutes)
                {
                    //Check location exists in current map
                    if (LocationsManager.instance.LocationExists(todaysRoutine._actions[i].destination))
                    {
                        //Set path
                        goal = LocationsManager.instance.GetLocation(todaysRoutine._actions[i].destination);

                        if (todaysRoutine._actions[i].type == RoutineAction.RoutineActionType.SPAWN)
                            Spawn(goal.position);

                        UpdatePath();

                        //Set done when close enough
                        if (Vector3.Distance(transform.position, goal.position) <= agent.stoppingDistance)
                        {
                            todaysRoutine._actions[i].done = true;

                            if (todaysRoutine._actions[i].type == RoutineAction.RoutineActionType.DESPAWN)
                            {
                                Despawn();
                            }
                        }
                    }
                }

                return;
            }
        }
    }

    private void FindTodaysRoutine()
    {
        if (schedule == null)
            return;

        for (int i = 0; i < schedule.routine.Count; i++)
        {
            for (int j = 0; j < schedule.routine[i].days.Length; j++)
            {
                if (DayNightCycle.instance.currentDay == schedule.routine[i].days[j])
                {
                    todaysRoutine = schedule.routine[i];
                    return;
                }
            }

        }
    }

    private void Roam()
    {
        if (roamPath == null)
            return;

        if(Vector3.Distance(transform.position, roamPath[currentRoamPoint].position) <= agent.stoppingDistance)
        {
            currentRoamPoint++;
            currentRoamPoint %= roamPath.Length;
        }

        goal = roamPath[currentRoamPoint];

        UpdatePath();

    }

    private void Fight()
    {
        RemoveDeadEnemies();
        Actor closest = FindClosestEnemy();

        if (closest == null)
        {
            goal = lastGoal;
            SetState(lastState);
        }
        else
        {
            goal = closest.transform;
            UpdatePath();
        }

        if(Vector3.Distance(transform.position, goal.position) <= agent.stoppingDistance)
        {
            StartCoroutine(actor.RotateOverTime(goal.position - transform.position, .25f));

            if (Random.Range(0, 2) == 1)
                actor.MainAttack();
            else
                actor.SecondaryAttack();
        }

        //Update nearby members
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

    public override void AddEnemy(Actor enemy)
    {
        base.AddEnemy(enemy);
        if (enemies.Contains(enemy) == false &&
            enemy != actor)
            enemies.Add(enemy);
    }

    private Actor FindClosestEnemy()
    {
        Actor closest = null;
        float distance = Mathf.Infinity;

        for (int i = 0; i < enemies.Count; i++)
        {
            if (enemies[i].isDead == false)
            {
                float dist = Vector3.Distance(transform.position, enemies[i].transform.position);
                if (dist < distance)
                {
                    closest = enemies[i];
                    distance = dist;
                }
            }
        }

        return closest;
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

    public void SetState (NPCState newState)
    {
        if (state == newState)
            return;

        lastState = state;
        lastGoal = goal;
        state = newState;
    }

    public override void StartInteraction()
    {
        base.StartInteraction();
        SetState(NPCState.IDLE);
    }

    public override void EndInteraction()
    {
        base.EndInteraction();
        SetState(lastState);
    }

    public bool Interact(Actor interactor)
    {
        Debug.Log(actor + "::Interacts with::" + this);

        StartInteraction();

        //SpeachBaloonManager.instance.CreateBalloon(transform, Random.Range(0,100).ToString(), 2f, 5f);
        //DialogueManager.instance.StartDialogue(actor.GetDialogue);
        StartCoroutine(actor.RotateOverTime(interactor.transform.position - transform.position, .25f));

        if (InGang(interactor.GetActorData().gang))
            InteractionManager.instance.OpenInteractionMenu(new InteractionType[]
            {
                InteractionType.Talk,
                InteractionType.Order,
                InteractionType.Stats,
                InteractionType.Trade
            }, new Brain[]{
                interactor.GetComponent<Brain>(),
                this });
        else
            InteractionManager.instance.OpenInteractionMenu(new InteractionType[]
            {
                InteractionType.Talk
            }, new Brain[]{
                interactor.GetComponent<Brain>(),
                this });

        return true;
    }

    public string DisplayText()
    {
        return "Talk";
    }

    protected override void OnDamaged(Actor attacker)
    {
        base.OnDamaged(attacker);

        string gang = actor.GetActorData().gang;
        if (string.IsNullOrEmpty(gang) == false && gang == attacker.GetActorData().gang)
        {
            return;
        }

        AddEnemy(attacker);
    }

    protected override void OnAttacking(Actor enemy)
    {
        base.OnAttacking(enemy);
    }

    private void Spawn(Vector3 position)
    {
        agent.Warp(position);
        transform.SetParent(CharacterManager.instance.spawnParent);
        isSpawned = true;
    }

    private void Despawn()
    {
        agent.Warp(CharacterManager.instance.despawnZone.position);
        transform.SetParent(CharacterManager.instance.despawnZone);
        isSpawned = false;
    }


}
