using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    [SerializeField] private string parentName = "SpawnedNPCs";
    private Transform parent;
    [SerializeField] private GameObject npcPrefab;

    [SerializeField] private Job[] jobs;
    [SerializeField] private float[] chances;

    [SerializeField] private float spawnRatePerMinute = 10;
    [SerializeField] private float spawnRadius = 3f;
    [SerializeField] private float maxSpawn = 20;
    [SerializeField] private List<GameObject> spawned = new List<GameObject>();

    [Header("Npc Path")]
    [SerializeField] private Transform[] path;

    [SerializeField] private bool visible;
    [SerializeField] private bool inRange;

    private float lastSpawn;

    private void Start()
    {
        parent = GameObject.Find(parentName).transform;
    }

    void Update()
    {
        visible = InViewport(Camera.main);
        inRange = InRange(Camera.main.transform);

        if (visible == true || inRange == true)
            return;

        if(Time.time - lastSpawn >= 60 / spawnRatePerMinute)
        {
            SpawnRandom();
        }

    }

    void SpawnRandom()
    {
        int random = Random.Range(0, 100);
        int randomJob;
        float x = 0;

        for (randomJob = 0; randomJob < chances.Length; randomJob++)
        {
            x += chances[randomJob];
            if (random <= x)
                break;
        }

        Vector2 randomCircle = Random.insideUnitCircle;
        Vector3 randomPosition = new Vector3(randomCircle.x, 0, randomCircle.y) * spawnRadius;

        GameObject go = Instantiate(npcPrefab, transform.position + randomPosition, Quaternion.identity) as GameObject;
        go.transform.SetParent(parent);
        go.GetComponent<Actor>().GenerateActor(jobs[randomJob]);

        NPCBrain brain = go.GetComponent<NPCBrain>();
        brain.SetRoamPath(path);
        brain.SetState(NPCState.ROAM);

        spawned.Add(go);

        lastSpawn = Time.time;
    }

    void Despawn(GameObject npc)
    {
        for (int i = 0; i < spawned.Count; i++)
        {
            if(spawned[i] == npc)
            {
                Destroy(npc);
                spawned.RemoveAt(i);
            }
        }
    }

    private bool InRange(Transform t)
    {
        if(Vector3.Distance(transform.position, t.position) < 10f)
            return true;

        return false;
    }

    private bool InViewport(Camera camera)
    {
        Vector3 position = camera.WorldToViewportPoint(transform.position);

        if (position.x > 0 && position.x < 1 &&
            position.y > 0 && position.y < 1 &&
            position.z > 0)
            return true;
        else
            return false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, spawnRadius);

        for (int i = 0; i < path.Length; i++)
        {
            Gizmos.DrawSphere(path[i].position, .25f);
        }
    }

    private void OnBecameVisible()
    {
        visible = true;
    }

    private void OnBecameInvisible()
    {
        visible = false;
    }
}
