#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class MeshFilterSaver : MonoBehaviour
{
    [SerializeField] private MeshFilter meshFilter;

    public void SaveMeshAsset()
    {
        if (meshFilter == null)
        {
            Debug.LogError("A MeshFilter must be assigned before saving.", this);
            return;
        }

        Mesh sourceMesh = meshFilter.sharedMesh != null ? meshFilter.sharedMesh : meshFilter.mesh;
        if (sourceMesh == null)
        {
            Debug.LogError("The assigned MeshFilter does not contain a mesh to save.", this);
            return;
        }

        const string meshesFolder = "Assets/Meshes";
        EnsureFolderExists(meshesFolder);

        Mesh meshAsset = Object.Instantiate(sourceMesh);
        meshAsset.name = string.IsNullOrWhiteSpace(sourceMesh.name) ? meshFilter.gameObject.name : sourceMesh.name;

        string assetPath = AssetDatabase.GenerateUniqueAssetPath($"{meshesFolder}/{meshAsset.name}.asset");
        AssetDatabase.CreateAsset(meshAsset, assetPath);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        EditorGUIUtility.PingObject(meshAsset);
        Debug.Log($"Saved mesh asset to {assetPath}", meshAsset);
    }

    private static void EnsureFolderExists(string folderPath)
    {
        string[] folders = folderPath.Split('/');
        string currentPath = folders[0];

        for (int i = 1; i < folders.Length; i++)
        {
            string nextPath = $"{currentPath}/{folders[i]}";
            if (!AssetDatabase.IsValidFolder(nextPath))
                AssetDatabase.CreateFolder(currentPath, folders[i]);

            currentPath = nextPath;
        }
    }
}

[CustomEditor(typeof(MeshFilterSaver))]
public sealed class MeshFilterMeshAssetSaverEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Save Mesh To Assets/Meshes"))
            ((MeshFilterSaver)target).SaveMeshAsset();
    }
}
#endif
