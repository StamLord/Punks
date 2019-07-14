using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GangSpawner : MonoBehaviour
{
    [SerializeField] private GameObject npcPrefab;

    [SerializeField] private float spawnRatePerMinute = 10;
    [SerializeField] private float spawnRadius = 3f;
    [SerializeField] private float maxSpawn = 5;
    [SerializeField] private List<GameObject> spawned = new List<GameObject>();
    private float lastSpawn;

    [SerializeField] private Gang gang;

    void Update()
    {
        if (Time.time - lastSpawn >= 60 / spawnRatePerMinute)
        {
            AssignGang();
            SpawnMember();
        }
    }

    private void AssignGang()
    {
        Territory territory = TerritoryManager.instance.FindTerritory(transform.position);

        if (territory)
            gang = territory.GetGang;
        else
            gang = null;
    }

    private void SpawnMember()
    {
        if (gang == null)
            return;

        if (GangManager.instance.AnyFreeMembers(gang) == false)
            return;

        ActorData memberToSpawn = GangManager.instance.GetRandomMember(gang);

        Vector2 randomCircle = Random.insideUnitCircle;
        Vector3 randomPosition = new Vector3(randomCircle.x, 0, randomCircle.y) * spawnRadius;

        GameObject go = Instantiate(npcPrefab, transform.position + randomPosition, Quaternion.identity) as GameObject;
        go.transform.SetParent(transform);
        go.GetComponent<Actor>().LoadActor(memberToSpawn);

        NPCBrain brain = go.GetComponent<NPCBrain>();
        brain.SetState(NPCState.IDLE);

        //IMPORTANT!
        GangManager.instance.AddToSpawned(memberToSpawn);

        lastSpawn = Time.time;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, spawnRadius);
    }
}
