using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

[System.Serializable]
public class DialogueTree
{
    public List<Switch> switchList = new List<Switch>();

    [XmlIgnore]
    public Dictionary<string, bool> switches;

    public string name;
    public DialogueNode[] nodes;
    public int nextNode;

    private void InitializeDictionary()
    {
        switches = new Dictionary<string, bool>();

        for (int i = 0; i < switchList.Count; i++)
        {
            switches.Add(switchList[i].switchName, switchList[i].state);
        }
    }

    public void SaveState()
    {
        if (switches == null)
            return;

        for (int i = 0; i < switchList.Count; i++)
        {
            if (switches.ContainsKey(switchList[i].switchName))
                switchList[i].state = switches[switchList[i].switchName];
        }
    }

    public DialogueNode GetNextNode()
    {
        if (switches == null)
            InitializeDictionary();

        for (int i = nextNode; i < nodes.Length; i++)
        {
            if (nodes[i].isViable(switches))
            {
                nextNode = i + 1;
                return nodes[i];
            }
        }

        return null;
    }

    public void Reset()
    {
        nextNode = 0;
    }

    public void ActivateSwitches(DialogueNode node)
    {
        if (switches == null)
            InitializeDictionary();

        for (int i = 0; i < node.switches.Length; i++)
        {
            string[] nameState = node.switches[i].Split('=');

            if (switches.ContainsKey(nameState[0]))
            {
                if (nameState.Length == 1 || nameState[1] == "TRUE" || nameState[1] == "true" || nameState[1] == "True")
                    switches[nameState[0]] = true;
                else if (nameState[1] == "FALSE" || nameState[1] == "false" || nameState[1] == "False")
                    switches[nameState[0]] = false;
                else
                    Debug.Log("Node::" + node + "::" + node.switches[i] +"::Tried to set an unfamiliar state::" + nameState[1]);
            }
            else
                Debug.Log("Node::" + node + "::Tried to use a non existent switch.");
        }
    }

    public void SetSwitch(string switchName, bool state)
    {
        if (switches == null)
            InitializeDictionary();

        if(switches.ContainsKey(switchName))
        {
            switches[switchName] = state;
        }
    }
}

[System.Serializable]
public class Switch
{
    [XmlAttribute("name")]
    public string switchName;
    [XmlAttribute("state")]
    public bool state;
}

[System.Serializable]
public class DialogueNode
{
    //List of switches that need to be True for node to be viable
    //Format "Switch Name = TRUE/FALSE"
    public string[] conditions;

    //Time within the dialogue appears
    public string fromTime;
    public string toTime;

    //Text to be presented
    public string text;

    //List of switches that need will be changed after text is presented
    //Format "Switch Name = TRUE/FALSE"
    [XmlArrayItem("set")]
    public string[] switches;

    public bool isViable(Dictionary<string,bool> switches)
    {
        for (int i = 0; i < conditions.Length; i++)
        {
            string[] switchState = conditions[i].Split('=');

            if (switches.ContainsKey(switchState[0]))
            {
                bool state = false;

                if (switchState.Length == 1 || switchState[1] == "TRUE" || switchState[1] == "true" || switchState[1] == "True")
                    state = true;
                else if (switchState[1] == "FALSE" || switchState[1] == "false" || switchState[1] == "False")
                    state = false;


                if (switches[switchState[0]] != state)
                {
                    Debug.Log("DialogueNode validation failed - Switch is false");
                    return false;
                }
            }
            else
            {
                Debug.Log("DialogueNode validation failed - Switch does not exist");
                return false;
            }
        }

        Debug.Log("DialogueNode validation success");
        return true;
    }
}
