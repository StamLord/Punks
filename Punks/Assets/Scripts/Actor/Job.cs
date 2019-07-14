using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName ="New Job", menuName = "Job")]
public class Job : ScriptableObject
{
    [Header("Appearance")]
    public LimitAppearance limitAppearance;

    [Header("Stats Range")]
    public ActorStats minStats;
    public ActorStats maxStats;
}
