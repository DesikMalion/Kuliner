using System.Collections.Generic;
using UnityEngine;

public class Rope : MonoBehaviour
{
    [Header("RigidBodies")]
    public List<Transform> points = new List<Transform>();
    
    [Header("Appearance")]
    public Material lineMaterial;
    public bool showLineRenderers = true;

    private LineRenderer lr;

    public void Init(List<Transform> pts, Material lineMaterial)
    {
        points = pts;
        this.lineMaterial = lineMaterial;
    }
    private void Awake()
    {
        if (!showLineRenderers)
            return;

        lr = gameObject.AddComponent<LineRenderer>();
        lr.useWorldSpace = true;
        lr.positionCount = points.Count;
        lr.material = lineMaterial;
        lr.startWidth = 0.01f;
        lr.endWidth = 0.01f;
        lr.generateLightingData = true;
    }
    void LateUpdate()
    {
        if (!showLineRenderers || lr == null || points == null || points.Count == 0)
            return;

        UpdateLineRenderer();
        points[0].transform.localPosition = Vector3.zero;
    }
    
    void UpdateLineRenderer()
    {
        for (int i = 0; i < points.Count; i++)
            lr.SetPosition(i, points[i].position);
    }
    
}
