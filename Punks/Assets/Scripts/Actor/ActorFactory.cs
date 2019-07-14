using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ActorFactory
{
    public static ActorData GenerateActor(Job job, Gang gang)
    {
        ActorData newActor = new ActorData();

        newActor.gang = gang;

        if (job)
        {
            Customization.instance.GenerateCharacter(ref newActor.appearance, job.limitAppearance);

            newActor.stats.health = Random.Range(job.minStats.health, job.maxStats.health);
            newActor.stats.attack = Random.Range(job.minStats.attack, job.maxStats.attack);
            newActor.stats.defense = Random.Range(job.minStats.defense, job.maxStats.defense);
        }
        else
        {
            Customization.instance.GenerateCharacter(ref newActor.appearance, new LimitAppearance());

            newActor.stats.health = 100;
            newActor.stats.attack = 5;
            newActor.stats.defense = 5;
        }

        return newActor;
    }
}
