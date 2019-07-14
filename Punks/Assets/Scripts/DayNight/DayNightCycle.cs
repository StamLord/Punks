using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    [Header("Time Settings")]
    [SerializeField]
    private float _targetDayLength; //In minutes
    public float targetDayLength { get { return _targetDayLength; } }

    [SerializeField]
    [Range(0, 1)]
    private float _timeOfDay;
    public float timeOfDay { get { return _timeOfDay; } }

    [SerializeField]
    private int _dayNumber = 0;
    public int dayNumber { get { return _dayNumber; } }

    [SerializeField]
    private int _yearNumber = 0;
    public int yearNumber { get { return _yearNumber; } }

    private float _timeScale = 100f;
    private float _yearLength = 1000f;
    public bool pause = false;

    [Header("Sun Light")]
    [SerializeField]
    private Transform dailyRotation;
    [SerializeField]
    private Light sun;
    private float intensity;
    [SerializeField]
    private float sunBaseIntensity = 1f;
    [SerializeField]
    private float sunVariation = 1.5f;
    [SerializeField]
    private Gradient sunColor;

    public static DayNightCycle instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogWarning("More than 1 instance of DayNightCycle exists!");
    }

    private void Update()
    {
        if (!pause)
        {
            UpdateTimeScale();
            UpdateTime();
        }

        AdjustSunRotation();
        SunIntensity();
        AdjustSunColor();
    }

    private void UpdateTimeScale()
    {
        _timeScale = 24 / (_targetDayLength / 60);
    }

    private void UpdateTime()
    {
        _timeOfDay += Time.deltaTime * _timeScale / 86400; //Sceonds in day

        //New Day
        if (_timeOfDay > 1)
        {
            _dayNumber++;
            _timeOfDay -= 1;

            //New Year
            if (_dayNumber > _yearLength)
            {
                _yearNumber++;
                _dayNumber = 0;
            }
        }
    }

    private void AdjustSunRotation()
    {
        float sunAngle = timeOfDay * 360f;
        dailyRotation.localRotation = Quaternion.Euler(new Vector3(0, 0, sunAngle));
    }

    private void SunIntensity()
    {
        intensity = Vector3.Dot(sun.transform.forward, Vector3.down);
        intensity = Mathf.Clamp(intensity, 0, 1);

        sun.intensity = intensity * sunVariation + sunBaseIntensity;
    }

    private void AdjustSunColor()
    {
        sun.color = sunColor.Evaluate(intensity);
    }


    public int GetMinutes()
    {
        return (int)(1440 * _timeOfDay);
    }

    public int GetHours()
    {
        return GetMinutes() / 60;
    }
}
