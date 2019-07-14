using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SpeachBalloon : MonoBehaviour
{

    [Header("Settings")]
    [SerializeField] private float minimumSize = .5f;
    [SerializeField] private float dissapearDuration = 1f;

    //Gets in real time
    private new Camera camera;
    private Transform listener;
    private Transform talker;
    private RectTransform rectTransform;
    private Vector2 screenOffset;
    private float duration;
    private float startTime;
    private float hearingDistance;

    [Header("References")]
    [SerializeField] private Image image;
    [SerializeField] private TextMeshProUGUI textHolder;
    [SerializeField] private Vector3 targetOffSet;



    public void Initialize(Transform origin, string text, float duration, float hearingDistance)
    {
        camera = Camera.main;
        listener = GameObject.Find("Player").transform;
        talker = origin;
        textHolder.text = text;
        rectTransform = GetComponent<RectTransform>();
        screenOffset = rectTransform.rect.size / 2;

        this.duration = duration;
        startTime = Time.time;

        this.hearingDistance = hearingDistance;

        UpdatePosition();

    }

    private void Update()
    {
        UpdatePosition();
        UpdateScale();
        UpdateTransperancy();
    }

    void UpdatePosition()
    {
        if (talker == null)
            return;

        Vector3 positionOnScreen = camera.WorldToScreenPoint(talker.position + targetOffSet);

        if (positionOnScreen.z < 0)
            positionOnScreen *= -1;

        positionOnScreen = new Vector3(Mathf.Clamp(positionOnScreen.x, screenOffset.x * rectTransform.localScale.x, Screen.width - screenOffset.x *rectTransform.localScale.x),
            Mathf.Clamp(positionOnScreen.y, screenOffset.y * rectTransform.localScale.y, Screen.height - screenOffset.y * rectTransform.localScale.y), 0);

        transform.position = positionOnScreen;
    }

    void UpdateScale()
    {
        float distance = Vector3.Distance(talker.position, listener.position);

        rectTransform.localScale = Vector3.one * Mathf.Clamp(hearingDistance / distance, minimumSize, 1f);
    }

    void UpdateTransperancy()
    {
        Color imageColor = image.color;
        Color textColor = textHolder.color;

        float alpha = Mathf.Lerp(1, 0, (Time.time - startTime - duration) / dissapearDuration);

        imageColor.a = alpha;
        image.color = imageColor;

        textColor.a = alpha;
        textHolder.color = textColor;

    }
}
