using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ExtrudeOnClick : MonoBehaviour
{

    public int segments = 1;
    public bool invertFaces = false;

    public float extrusionLength = 1.0f;

    public bool autoCalculateOrientation = true;


    private Mesh srcMesh;
    private MeshExtrusion.Edge[] precomputedEdges;


    private class ExtrudedSection
    {
        public Vector3 point;
        public Matrix4x4 matrix;
    }

    void Start()
    {
        srcMesh = GetComponent<MeshFilter>().sharedMesh;
        precomputedEdges = MeshExtrusion.BuildManifoldEdges(srcMesh);
    }


    void OnMouseDown()
    {
        Extrude();
    }

    void Extrude()
    {
        Vector3 extrusionNormal = transform.right;

        List<ExtrudedSection> sections = new List<ExtrudedSection>();


        ExtrudedSection section = new ExtrudedSection();
        section.point = transform.position;
        section.matrix = transform.localToWorldMatrix;
        sections.Insert(0, section);

        for (int i = 0; i < segments; i++)
        {
            transform.position = transform.position + extrusionNormal * 1.0f / segments;
            section = new ExtrudedSection();
            section.point = transform.position;
            section.matrix = transform.localToWorldMatrix;
            sections.Insert(0, section);
        }

        Matrix4x4 worldToLocal = transform.worldToLocalMatrix;
        Quaternion rotation = Quaternion.LookRotation(-extrusionNormal, Vector3.up);
        Matrix4x4[] finalSections = new Matrix4x4[sections.Count];
        for (int i = 0; i < sections.Count; i++)
        {
            finalSections[i] = worldToLocal * Matrix4x4.TRS(sections[i].point, rotation, Vector3.one);
        }

        MeshExtrusion.ExtrudeMesh(srcMesh, GetComponent<MeshFilter>().mesh, finalSections, precomputedEdges, invertFaces);

    }
}