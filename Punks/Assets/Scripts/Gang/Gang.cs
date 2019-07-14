using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Gang", menuName ="Gang")]
public class Gang : ScriptableObject
{
    public string gangName;
    public Color gangColor;

    public AppearanceData uniform;

    public ActorData leader;
    public int maxCapacity = 10;
    public List<ActorData> members;

    public Gang[] allies;
    public Gang[] enemies;

    public bool autoRecruit;
    public float recruitPerDay;
    public int lastRecruitDay { get; private set; }

    public bool AttemptRecruit(ActorData actor)
    {
        if(members.Count <  maxCapacity)
        {
            members.Add(actor);
            lastRecruitDay = DayNightCycle.instance.dayNumber;
            return true;
        }

        return false;
    }
}
