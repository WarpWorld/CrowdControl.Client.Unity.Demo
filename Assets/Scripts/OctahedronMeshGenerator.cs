#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(MeshFilter))]
public class OctahedronMeshGenerator : MonoBehaviour
{
    [ContextMenu("Generate Octahedron Mesh Asset")]
    public void Generate()
    {
        Mesh mesh = new Mesh();
        mesh.name = "Octahedron";

        Vector3[] vertices = new Vector3[]
        {
            new Vector3(0, 1, 0),
            new Vector3(1, 0, 0),
            new Vector3(0, 0, 1),
            new Vector3(-1, 0, 0),
            new Vector3(0, 0, -1),
            new Vector3(0, -1, 0),
        };

        int[] triangles = new int[]
        {
            0, 2, 1,
            0, 3, 2,
            0, 4, 3,
            0, 1, 4,

            5, 1, 2,
            5, 2, 3,
            5, 3, 4,
            5, 4, 1
        };

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }
}

[CustomEditor(typeof(OctahedronMeshGenerator))]
public sealed class OctahedronMeshGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Generate Octahedron Mesh"))
            ((OctahedronMeshGenerator)target).Generate();

    }
}
#endif