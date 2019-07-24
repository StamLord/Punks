using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class InteractionText : MonoBehaviour
{
    [SerializeField] private Vector3 offset = new Vector3(0,1,0);
    [SerializeField] private GameObject target;
    [SerializeField] private TextMeshProUGUI displayText;
    private new Camera camera;

    private void Start()
    {
        camera = Camera.main;
        displayText = GetComponent<TextMeshProUGUI>();
    }

    public void ChangeTarget(GameObject newTarget, string text)
    {
        target = newTarget;
        displayText.text = text;
        UpdatePosition();
    }

    private void FixedUpdate()
    {
        UpdatePosition();
    }

    private void UpdatePosition()
    {
        if (target)
            transform.position = camera.WorldToScreenPoint(target.transform.position + offset);
    }

}
