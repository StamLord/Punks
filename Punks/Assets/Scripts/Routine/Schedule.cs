using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu( fileName ="Routine", menuName = "Routine")]
public class Schedule : ScriptableObject
{
    public List<Routine> routine = new List<Routine>();
}

[System.Serializable]
public class RoutineAction
{
    public enum RoutineActionType {SPAWN, GO, DESPAWN }

    public RoutineActionType type;
    public string time;
    public string destination;

    [System.Xml.Serialization.XmlIgnore]
    public bool done;
}

[System.Serializable]
public class Routine
{
    public Day[] days;
    public List<RoutineAction> _actions = new List<RoutineAction>();

}

