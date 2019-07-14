using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CustomizationObject
{
    public string name;
    public GameObject gameObject;
}

[System.Serializable]
public struct AppearanceData
{
    public int head;
    public int torso;
    public int legs;
    public int shoes;
    public int accessory;
    public int color;
}

[System.Serializable]
public struct LimitAppearance
{
    public int[] pHeads;
    public int[] pTorsos;
    public int[] pLegs;
    public int[] pShoes;
    public int[] pAccesories;
    public int[] pColors;
}

public class Customization : MonoBehaviour
{
    [SerializeField]
    private List<CustomizationObject> head = new List<CustomizationObject>();
    [SerializeField]
    private List<CustomizationObject> torso = new List<CustomizationObject>();
    [SerializeField]
    private List<CustomizationObject> legs = new List<CustomizationObject>();
    [SerializeField]
    private List<CustomizationObject> shoes = new List<CustomizationObject>();
    [SerializeField]
    private List<CustomizationObject> accessory = new List<CustomizationObject>();
    [SerializeField]
    private List<Color> colors = new List<Color>();


    public static Customization instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogWarning("More than 1 instance of Customization exists!");
    }

    public void GenerateCharacter(ref AppearanceData appearance, LimitAppearance limit)
    {
        if (limit.pHeads == null || limit.pHeads.Length == 0)
            appearance.head = Random.Range(0, head.Count - 1);
        else
            appearance.head = limit.pHeads[Random.Range(0, limit.pHeads.Length - 1)];

        if (limit.pTorsos == null || limit.pTorsos.Length == 0)
            appearance.torso = Random.Range(0, torso.Count - 1);
        else
            appearance.torso = limit.pTorsos[Random.Range(0, limit.pTorsos.Length - 1)];

        if (limit.pLegs == null || limit.pLegs.Length == 0)
            appearance.legs = Random.Range(0, legs.Count - 1);
        else
            appearance.legs = limit.pLegs[Random.Range(0, limit.pLegs.Length - 1)];

        if (limit.pShoes == null || limit.pShoes.Length == 0)
            appearance.shoes = Random.Range(0, shoes.Count - 1);
        else
            appearance.shoes = limit.pShoes[Random.Range(0, limit.pShoes.Length - 1)];

        if (limit.pAccesories == null || limit.pAccesories.Length == 0)
            appearance.accessory = Random.Range(0, accessory.Count - 1);
        else
            appearance.accessory = limit.pAccesories[Random.Range(0, limit.pAccesories.Length - 1)];

        if (limit.pColors == null || limit.pColors.Length == 0)
            appearance.color = Random.Range(0, colors.Count - 1);
        else
            appearance.color = limit.pColors[Random.Range(0, limit.pColors.Length - 1)];
    }

    public void DressCharacter(Transform head, Transform torso, Transform legs, Transform shoes, SkinnedMeshRenderer renderer, AppearanceData appearance)
    {
        renderer.materials[5].color = colors[appearance.color];
    }

    public void DressCharacter(Transform head, Transform torso, Transform legs, Transform shoes, SkinnedMeshRenderer renderer, AppearanceData appearance, AppearanceData gangUniform)
    {

    }
}


