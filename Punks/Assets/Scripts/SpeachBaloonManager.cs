using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpeachBaloonManager : MonoBehaviour
{
    [SerializeField] private Canvas canvas;
    [SerializeField] private GameObject balloonPrefab;

    [SerializeField] private Dictionary<Transform, GameObject> spawned = new Dictionary<Transform, GameObject>();

    public static SpeachBaloonManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogWarning("More than 1 instance of SpeachBalloonManager exists!");
    }

    void Update()
    {
        
    }

    public void CreateBalloon(Transform origin, string text, float duration, float hearingDistance)
    {
        GameObject go;

        if (spawned.ContainsKey(origin))
        {
            if(spawned[origin] != null)
                go = spawned[origin];
            else
                go = Instantiate(balloonPrefab, canvas.transform) as GameObject;
        }
        else
        {
            go = Instantiate(balloonPrefab, canvas.transform) as GameObject;
            spawned.Add(origin, go);

        }

        SpeachBalloon balloon = go.GetComponent<SpeachBalloon>();

        if (balloon)
        {
            balloon.Initialize(origin, text, duration, hearingDistance);
        }

        
    }
}


