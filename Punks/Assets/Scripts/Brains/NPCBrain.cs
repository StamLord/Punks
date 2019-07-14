using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public enum NPCState { IDLE, ROAM, FOLLOW, FIGHT, FLEE}

[RequireComponent(typeof(NavMeshAgent))]
public class NPCBrain : Brain, IInteractable
{
    private NavMeshAgent agent;
    private LineRenderer lr;

    [SerializeField] private NPCState state;
    [SerializeField] NPCState lastState;

    //Pathfinding
    private float lastCalculation;
    private Queue<Vector3> path = new Queue<Vector3>();
    [SerializeField] private Transform goal;
    [SerializeField] private Transform lastGoal;

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
                Idle();
                break;
            case NPCState.ROAM:
                Roam();
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

        if(path.Count > 0 && Vector3.Distance(transform.position, path.Peek()) <= agent.stoppingDistance)
        {
            path.Dequeue();
        }

        if (path.Count > 0 && state != NPCState.IDLE)
        {
            Vector3 direction = (path.Peek() - transform.position).normalized;
            direction.y = 0;

            float distance = agent.remainingDistance;
            if (state == NPCState.ROAM)
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

    private void Idle()
    {
        
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
            actor.RotateTowards(goal.position - transform.position);

            if (Random.Range(0, 2) == 1)
                actor.MainAttack();
            else
                actor.SecondaryAttack();
        }

        //Update nearby members
        if (actor.GetActorData().gang != null)
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

    public bool Interact(Actor interactor)
    {
        Debug.Log(actor + "::Interacts with::" + this);

        if (state != NPCState.IDLE)
            SetState(NPCState.IDLE);
        else
            SetState(lastState);

        //SpeachBaloonManager.instance.CreateBalloon(transform, Random.Range(0,100).ToString(), 2f, 5f);
        DialogueManager.instance.StartDialogue(actor.GetDialogue);

        return true;
    }

    protected override void OnDamaged(Actor attacker)
    {
        base.OnDamaged(attacker);

        Gang gang = actor.GetActorData().gang;
        if (gang && gang == attacker.GetActorData().gang)
        {
            return;
        }

        AddEnemy(attacker);
    }

    protected override void OnAttacking(Actor enemy)
    {
        base.OnAttacking(enemy);
    }

}
