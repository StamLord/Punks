using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GangManager : MonoBehaviour
{
    [Header("Gangs")]
    [SerializeField]
    private Gang[] gangs;
    private DayNightCycle timeSystem;

    [Header("Auto Recruiting")]
    [SerializeField]
    private Job[] autoRecruitJobs;
    private bool autoRecruitFlag;

    private Dictionary<Gang, List<ActorData>> spawnedMembers = new Dictionary<Gang, List<ActorData>>();

    public static GangManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogWarning("More than 1 instance of GangManager exists!");
    }

    void Start()
    {
        timeSystem = DayNightCycle.instance;

        InitializeSpawned();
    }

    private void InitializeSpawned()
    {
        for (int i = 0; i < gangs.Length; i++)
        {
            spawnedMembers.Add(gangs[i], new List<ActorData>());
        }
    }

    void Update()
    {
        if (timeSystem.GetHours() == 12)
        {
            if (autoRecruitFlag == false)
            {
                AutoRecruit();
                autoRecruitFlag = true;
            }
        }
        else if (autoRecruitFlag == true)
            autoRecruitFlag = false;
    }

    public void AutoRecruit()
    {
        for (int i = 0; i < gangs.Length; i++)
        {
            if(gangs[i].autoRecruit)
            {
                int newRecruits = Mathf.FloorToInt((timeSystem.dayNumber - gangs[i].lastRecruitDay) * gangs[i].recruitPerDay);
                for (int j = 0; j < newRecruits; j++)
                {
                    RandomRecruit(gangs[i]);
                }
            }
        }
    }

    public void Recruit(ActorData actor, Gang gang)
    {
        for (int i = 0; i < gangs.Length; i++)
        {
            if(gang.gangName == gangs[i].gangName)
            {
                gang.AttemptRecruit(actor);
            }
        }
    }

    private void RandomRecruit(Gang gang)
    {
        ActorData newActor = ActorFactory.GenerateActor(
            autoRecruitJobs[Random.Range(0, autoRecruitJobs.Length)], 
            null);

        Recruit(newActor, gang);
    }

    public bool AnyFreeMembers(Gang gang)
    {
        if (gang.members.Count > spawnedMembers[gang].Count)
            return true;

        return false;
    }

    public ActorData GetRandomMember(Gang gang)
    {
        List<ActorData> notSpawned = gang.members;

        for (int i = 0; i < spawnedMembers[gang].Count; i++)
        {
            if (notSpawned.Contains(spawnedMembers[gang][i]))
                notSpawned.Remove(spawnedMembers[gang][i]);
        }

        return notSpawned[Random.Range(0, notSpawned.Count)];
    }

    public void AddToSpawned(ActorData member)
    {
        spawnedMembers[member.gang].Add(member);
    }
}
