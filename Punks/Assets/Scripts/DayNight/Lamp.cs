using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lamp : MonoBehaviour
{
    new private Light light;
    [SerializeField]
    private int onHour;
    [SerializeField]
    private int offHour;

    private void Awake()
    {
        light = GetComponent<Light>();
    }

    private void Update()
    {
        int hour = DayNightCycle.instance.GetHours();

        if (hour >= onHour || hour < offHour)
        {
            if(light.enabled == false)
                light.enabled = true;
        }
        else if (hour >= offHour && light.enabled)
            light.enabled = false;
    }
}
