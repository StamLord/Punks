using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class CharacterManager : MonoBehaviour
{
    public Schedule routine;
    [SerializeField] private GameObject _npcPrefab;
    [SerializeField] private Transform _spawnParent;
    public Transform spawnParent { get { return _spawnParent; } }
    [SerializeField] private Transform _despawnZone;
    public Transform despawnZone { get { return _despawnZone; } }

    private string charactersDirectory = "Characters";
    private string dialoguesDirectory = "Dialogues";
    [SerializeField] private List<Character> characters = new List<Character>();

    public static CharacterManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogWarning("More than 1 instance of CharacterManager exists!");
    }

    void Start()
    {
        LoadAllCharacters();
        SpawnCharacter(characters[1]);

        SaveCharacter(BaseCharacter());
    }

    private Character BaseCharacter()
    {
        Character character = new Character();
        character.actorData = new ActorData {
            firstName = "Base",
            lastName = "Template",
            stats = new ActorStats(),
            gang = "",
            money = 0,
            appearance = new AppearanceData() };

        character.schedule = new Schedule();

        character.schedule.routine.Add(new Routine() { days = new Day[]{
            Day.Sunday,
            Day.Monday,
            Day.Tuesday,
            Day.Wednesday,
            Day.Thursday,
            Day.Friday,
            Day.Saturday } });

        character.schedule.routine[0]._actions.Add(new RoutineAction() {
            time = "",
            destination = "" });

        character.dialogue = new DialogueTree()
        {
            switchList = new List<Switch>()
            {
                new Switch(){ switchName = "Switch1",  state = false },
                new Switch(){ switchName = "Switch2", state = false}
            },

            nodes = new DialogueNode[]
            {
                new DialogueNode()
                {
                    conditions = new string[]{ },
                    fromTime = "00:00",
                    toTime = "23:59",
                    text = "First sentenece to be displayed. Will turn Switch1 on.",
                    switches = new string[] {"Switch1=TRUE"}
                },
                new DialogueNode()
                {
                    conditions = new string[]{ "Switch1"},
                    fromTime = "00:00",
                    toTime = "23:59",
                    text = "Second sentenece to be displayed when Switch1 is on.",
                    switches = new string[] {}
                }
            }

        };

        return character;
    }

    private void SpawnCharacter(Character character)
    {
        GameObject go = Instantiate(_npcPrefab, _despawnZone.position, Quaternion.identity) as GameObject;
        go.transform.SetParent(_despawnZone);

        //Load data onto object
        Actor actor = go.GetComponent<Actor>();
        actor.LoadActor(character.actorData);

        //Set dialogue
        actor.SetDialogue(character.dialogue);

        //Set routine
        NPCBrain brain = go.GetComponent<NPCBrain>();
        brain.SetState(NPCState.SCHEDULE);
        brain.SetRoutine(character.schedule);
    }

    private void LoadAllCharacters()
    {
        if(Directory.Exists(Application.dataPath + "/" + charactersDirectory) == false)
        {
            Directory.CreateDirectory(Application.dataPath + "/" + charactersDirectory);
        }

        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/" + charactersDirectory);
        FileInfo[] info = dir.GetFiles("*.xml");

        for (int i = 0; i < info.Length; i++)
        {
            Character character = XML.Deserialize<Character>(info[i].FullName);
            character.dialogue = LoadDialogue(character.actorData.firstName+character.actorData.lastName);

            characters.Add(character);
            Debug.Log("Loaded Character::" + info[i].FullName);
        }
    }

    private void SaveCharacter(Character character)
    {
        XML.Serialize(character, Application.dataPath + "/" + charactersDirectory + "/" + character.actorData.firstName + character.actorData.lastName + ".xml");
        if (character.dialogue != null)
        {
            character.dialogue.SaveState();
            SaveDialogue(character.dialogue, character);
        }
    }

    public void SaveAllCharacters()
    {
        for (int i = 0; i < characters.Count; i++)
        {
            SaveCharacter(characters[i]);
        }
    }

    public DialogueTree LoadDialogue(string FirstnameLastname)
    {
        if (Directory.Exists(Application.dataPath + "/" + dialoguesDirectory) == false)
        {
            Directory.CreateDirectory(Application.dataPath + "/" + dialoguesDirectory);
        }

        DirectoryInfo dir = new DirectoryInfo(Application.dataPath + "/" + dialoguesDirectory);
        FileInfo[] info = dir.GetFiles(FirstnameLastname + ".xml");

        if(info.Length > 1)
        {
            Debug.Log("Found 2 dialogue files for::" + FirstnameLastname);
        }

        else if (info.Length < 1)
        {
            Debug.Log("No dialogue file exists for::" + FirstnameLastname);
            return null;
        }

        return XML.Deserialize<DialogueTree>(info[0].FullName);
    }

    public void SaveDialogue(DialogueTree dialogue, Character character)
    {
        XML.Serialize(dialogue, Application.dataPath + "/" + dialoguesDirectory + "/" + character.actorData.firstName + character.actorData.lastName + ".xml");
    }
}

[System.Serializable]
public class Character
{
    public ActorData actorData;
    public Schedule schedule;

    [System.Xml.Serialization.XmlIgnoreAttribute]
    public DialogueTree dialogue;
}
