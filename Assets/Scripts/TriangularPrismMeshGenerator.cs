#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;

[RequireComponent(typeof(MeshFilter))]
public class TriangularPrismMeshGenerator : MonoBehaviour
{
    [ContextMenu("Generate Triangular Prism Mesh Asset")]
    public void Generate()
    {
        // 1x1x1 triangular prism (ramp): a unit cube split diagonally on one face.
        // Coordinates are centered on the origin like Unity's built-in cube (extents 0.5).
        // The ramp rises from y=-0.5 to y=+0.5 across z=-0.5 to z=+0.5.

        Mesh mesh = new Mesh();
        mesh.name = "TriangularPrism";

        // Use non-shared vertices per face to keep hard edges (avoid averaged normals artifacts).
        Vector3[] vertices = new Vector3[]
        {
            // Bottom (y-)
            new Vector3(-0.5f, -0.5f, -0.5f), // 0
            new Vector3( 0.5f, -0.5f, -0.5f), // 1
            new Vector3( 0.5f, -0.5f,  0.5f), // 2
            new Vector3(-0.5f, -0.5f,  0.5f), // 3

            // Back (z-)
            new Vector3(-0.5f, -0.5f, -0.5f), // 4
            new Vector3( 0.5f, -0.5f, -0.5f), // 5
            new Vector3( 0.5f,  0.5f,  0.5f), // 6
            new Vector3(-0.5f,  0.5f,  0.5f), // 7

            // Left (x-)
            new Vector3(-0.5f, -0.5f, -0.5f), // 8
            new Vector3(-0.5f, -0.5f,  0.5f), // 9
            new Vector3(-0.5f,  0.5f,  0.5f), // 10

            // Right (x+)
            new Vector3( 0.5f, -0.5f, -0.5f), // 11
            new Vector3( 0.5f, -0.5f,  0.5f), // 12
            new Vector3( 0.5f,  0.5f,  0.5f), // 13

            // Front vertical (z+)
            new Vector3(-0.5f, -0.5f,  0.5f), // 14
            new Vector3( 0.5f, -0.5f,  0.5f), // 15
            new Vector3( 0.5f,  0.5f,  0.5f), // 16
            new Vector3(-0.5f,  0.5f,  0.5f), // 17

            // Ramp (diagonal)
            new Vector3(-0.5f, -0.5f, -0.5f), // 18
            new Vector3( 0.5f, -0.5f, -0.5f), // 19
            new Vector3( 0.5f,  0.5f,  0.5f), // 20
            new Vector3(-0.5f,  0.5f,  0.5f), // 21
        };

        Vector2[] uvs = new Vector2[]
        {
            // Bottom
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
            // Back
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
            // Left
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1),
            // Right
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1),
            // Front
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
            // Ramp
            new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
        };

        int[] triangles = new int[]
        {
            // Bottom (y-)
            0, 1, 2,
            0, 2, 3,

            // Back (z-)
            4, 5, 6,
            4, 6, 7,

            // Left (x-)
            8, 9, 10,

            // Right (x+)
            11, 13, 12,

            // Front (z+)
            14, 15, 16,
            14, 16, 17,

            // Ramp (diagonal face)
            18, 20, 19,
            18, 21, 20,
        };

        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = mesh;
    }
}

[CustomEditor(typeof(TriangularPrismMeshGenerator))]
public sealed class TriangularPrismMeshGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Generate Triangular Prism Mesh"))
            ((TriangularPrismMeshGenerator)target).Generate();
    }
}
#endif
