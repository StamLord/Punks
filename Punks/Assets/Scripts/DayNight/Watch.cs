using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Watch : MonoBehaviour
{
    private TextMeshProUGUI text;

    void Start()
    {
        text = GetComponent<TextMeshProUGUI>();
    }

    // Update is called once per frame
    void Update()
    {
        HourMinutes time = DayNightCycle.instance.GetTime();
        string hours = (time.hours < 10) ? "0" + time.hours : time.hours.ToString();
        string minutes = (time.minutes < 10) ? "0" + time.minutes : time.minutes.ToString();
        text.text = hours + ":" + minutes;
    }
}
