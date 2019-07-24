using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class GangManager : MonoBehaviour
{
    private string directory = "Gangs";

    [Header("Gangs")]
    [SerializeField]
    private List<Gang> gangs = new List<Gang>();
    private DayNightCycle timeSystem;

    [Header("Auto Recruiting")]
    [SerializeField]
    private Job[] autoRecruitJobs;
    private bool autoRecruitFlag;

    private Dictionary<Gang, List<ActorData>> spawnedMembers = new Dictionary<Gang, List<ActorData>>();

    [Header("Template Gang")]
    [SerializeField] private Gang template;

    #region Signleton

    public static GangManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogWarning("More than 1 instance of GangManager exists!");
    }

    #endregion

    void Start()
    {
        timeSystem = DayNightCycle.instance;
        LoadAllGangs();
        InitializeSpawned();
        SaveGang(template);
    }

    void CreateDirectory()
    {
        if (Directory.Exists(Application.dataPath + "/" + directory) == false)
        {
            Directory.CreateDirectory(Application.dataPath + "/" + directory);
        }
    }

    void LoadAllGangs()
    {
        CreateDirectory();

        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/" + directory);
        FileInfo[] info = dir.GetFiles("*.xml");

        for (int i = 0; i < info.Length; i++)
        {
            Gang loaded = XML.Deserialize<Gang>(info[i].FullName);
            gangs.Add(loaded);

            for (int j = 0; j < loaded.territories.Count; j++)
            {
                TerritoryManager.instance.SetTerritoryValues(loaded.territories[j].territoryName, loaded, loaded.territories[j].influence);
            }

            Debug.Log("Loaded Gang::" + info[i].FullName);
        }
    }

    public void SaveAllGangs()
    {
        for (int i = 0; i < gangs.Count; i++)
        {
            SaveGang(gangs[i]);
            Debug.Log("Saved Gang::" + gangs[i]);
        }
    }

    void SaveGang(Gang gang)
    {
        CreateDirectory();

        //Update territories under control

        List<Territory> territories = TerritoryManager.instance.GetTerritories(gang);
        gang.territories.Clear();

        for (int i = 0; i < territories.Count; i++)
        {
            gang.territories.Add(new TerritoryData
            {
                territoryName = territories[i].name,
                influence = territories[i].influence
            });
        }

        XML.Serialize(gang, Application.dataPath + "/" + directory + "/" + gang.gangName + ".xml");
    }

    private void InitializeSpawned()
    {
        for (int i = 0; i < gangs.Count; i++)
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
        for (int i = 0; i < gangs.Count; i++)
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
        for (int i = 0; i < gangs.Count; i++)
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

        notSpawned.RemoveAll(IsHospitalized);

        return notSpawned[Random.Range(0, notSpawned.Count)];
    }

    public bool IsHospitalized(ActorData actorData)
    {
        if (actorData.hospitalizedDay + actorData.hospitalizedDuration > DayNightCycle.instance.dayNumber)
            return true;

        return false;
    }

    public void AddToSpawned(ActorData member)
    {
        Gang gang = GetGang(member.gang);
        if(gang)
            spawnedMembers[gang].Add(member);
    }

    public Gang GetGang(string gangName)
    {
        if (string.IsNullOrEmpty(gangName) == false)
        {
            for (int i = 0; i < gangs.Count; i++)
            {
                if (gangName == gangs[i].gangName)
                {
                    return gangs[i];
                }
            }
        }

        return null;
    }
}
