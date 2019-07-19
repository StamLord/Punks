using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Territory : MonoBehaviour
{

    [SerializeField] private Vector2[] corners = new Vector2[4];
    [SerializeField] private Gang rulingGang;
    public Gang GetGang { get { return rulingGang; } }

    [SerializeField] private List<ActorData> gangMembersPresent;


    [SerializeField] private float takeOverThreshold = 20f;
    [SerializeField] private float takeOverProgress;
    [Range(0,1)]
    [SerializeField] private float _takeOverMeter;
    public float takeOverMeter { get { return _takeOverMeter; } }
    [SerializeField] private bool _beingTakenOver;
    public bool beingTakenOver { get { return _beingTakenOver; } }

    [Range(0,100)]
    [SerializeField] private float influence;
    [SerializeField] private float influenceOverTime;
    [SerializeField] private float influencePerMember;

    private MeshRenderer minimapBoundries;
    [SerializeField] private Material minimapMaterial;

    private void Start()
    {
        MiniMapBoundries();
    }

    private void Update()
    {
        if(rulingGang == null)
        {
            CalculateTakeover();
        }
        else
        {
            CalculateInfluence();
        }
    }

    private void CalculateTakeover()
    {
        List<Gang> gangsPresent = new List<Gang>();

        int actorsWithoutGang = 0;

        for (int i = 0; i < gangMembersPresent.Count; i++)
        {
            Gang g = GangManager.instance.GetGang(gangMembersPresent[i].gang);

            if (g != null && gangsPresent.Contains(g) == false)
            {
                gangsPresent.Add(g);
            }
        }

        if (gangsPresent.Count == 1)
        {
            _beingTakenOver = true;
            takeOverProgress += Time.deltaTime * (gangMembersPresent.Count - actorsWithoutGang);
            _takeOverMeter = Mathf.Clamp(takeOverProgress / takeOverThreshold, 0 , 1);
        }
        else
        {
            _beingTakenOver = false;
        }

        if(takeOverMeter == 1f)
        {
            rulingGang = gangsPresent[0];
            takeOverProgress = _takeOverMeter = 0f;
            _beingTakenOver = false;
            influence = 10;
            UpdateMiniMapColor(rulingGang.gangColor);
        }
    }

    private void CalculateInfluence()
    {
        if (gangMembersPresent.Count < 1)
            influence += influenceOverTime * Time.deltaTime;
        else
        {
            for (int i = 0; i < gangMembersPresent.Count; i++)
            {
                if (gangMembersPresent[i].gang == rulingGang.gangName)
                    influence += influencePerMember * Time.deltaTime;
                else
                    influence -= influencePerMember * Time.deltaTime;
            }
        }

        influence = Mathf.Clamp(influence, 0, 100);

        if(influence == 0)
        {
            rulingGang = null;
            UpdateMiniMapColor(Color.black);
        }

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(AdjustVector(corners[0]), AdjustVector(corners[1]));
        Gizmos.DrawLine(AdjustVector(corners[1]), AdjustVector(corners[2]));
        Gizmos.DrawLine(AdjustVector(corners[2]), AdjustVector(corners[3]));
        Gizmos.DrawLine(AdjustVector(corners[3]), AdjustVector(corners[0]));
    }

    private Vector3 AdjustVector(Vector2 vector)
    {
        return new Vector3(vector.x, 0, vector.y);
    }

    public bool InTerritory(Vector2 position)
    {
        float minX = Mathf.Infinity;
        float maxX = -Mathf.Infinity;
        float minY = Mathf.Infinity;
        float maxY = -Mathf.Infinity;

        for (int i = 0; i < corners.Length; i++)
        {
            if (corners[i].x < minX)
                minX = corners[i].x;
            if (corners[i].x > maxX)
                maxX = corners[i].x;
            if (corners[i].y < minY)
                minY = corners[i].y;
            if (corners[i].y > maxY)
                maxY = corners[i].y;
        }

        if (position.x < minX ||
            position.x > maxX ||
            position.y < minY ||
            position.y > maxY)
            return false;

        return true;
    }

    public void AddActor(ActorData actor)
    {
        gangMembersPresent.Add(actor);
    }

    public void RemoveActor(ActorData actor)
    {
        gangMembersPresent.Remove(actor);
    }

    private void MiniMapBoundries()
    {
        Vector3[] newVertices = new Vector3[]{AdjustVector(corners[0]) + Vector3.up * 100, AdjustVector(corners[1]) + Vector3.up * 100, AdjustVector(corners[2]) + Vector3.up * 100, AdjustVector(corners[3]) + Vector3.up * 100 };
        //Vector2[] newUV = new Vector2[];
        int[] newTriangles = new int[] { 0, 2, 1, 0, 3, 2 };

        Mesh mesh = new Mesh();

        minimapBoundries = gameObject.AddComponent<MeshRenderer>();
        minimapBoundries.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        minimapBoundries.material = minimapMaterial;
        MeshFilter m = gameObject.AddComponent<MeshFilter>();
        m.mesh = mesh;

        mesh.vertices = newVertices;
        mesh.triangles = newTriangles;
        //mesh.RecalculateNormals();

    }

    private void UpdateMiniMapColor(Color color)
    {
        Color newColor = color;
        newColor.a = .5f;

        minimapBoundries.material.color = newColor;
    }
}
