using UnityEngine;
using UnityEditor;

public class SelectedTrisCounter : EditorWindow
{
    private enum CountMode
    {
        ActiveOnly,
        InactiveOnly,
        ActiveAndInactive
    }

    private CountMode countMode = CountMode.ActiveOnly;

    private int totalVertices;
    private int totalTriangles;
    private int totalObjects;
    private int meshObjectCount;

    [MenuItem("Tools/Selected Tris Counter")]
    public static void ShowWindow()
    {
        GetWindow<SelectedTrisCounter>("Tris Counter");
    }

    private void OnGUI()
    {
        GUILayout.Space(10);

        GUILayout.Label(
            "Selected Object Statistics",
            EditorStyles.boldLabel
        );

        GUILayout.Space(5);

        countMode = (CountMode)EditorGUILayout.EnumPopup(
            "Count Mode",
            countMode
        );

        GUILayout.Space(10);

        if (GUILayout.Button("Calculate Selected Objects", GUILayout.Height(30)))
        {
            Calculate();
        }

        GUILayout.Space(15);

        EditorGUILayout.LabelField(
            "Selected Root Objects",
            totalObjects.ToString()
        );

        EditorGUILayout.LabelField(
            "Mesh Objects Counted",
            meshObjectCount.ToString()
        );

        EditorGUILayout.LabelField(
            "Vertices",
            totalVertices.ToString("N0")
        );

        EditorGUILayout.LabelField(
            "Triangles",
            totalTriangles.ToString("N0")
        );

        GUILayout.Space(10);


    }

    private void Calculate()
    {
        totalVertices = 0;
        totalTriangles = 0;
        meshObjectCount = 0;

        GameObject[] selectedObjects = Selection.gameObjects;
        totalObjects = selectedObjects.Length;

        foreach (GameObject selectedObject in selectedObjects)
        {
            // Ambil MeshFilter termasuk child aktif dan tidak aktif
            MeshFilter[] meshFilters =
                selectedObject.GetComponentsInChildren<MeshFilter>(true);

            foreach (MeshFilter meshFilter in meshFilters)
            {
                if (!ShouldCount(meshFilter.gameObject))
                    continue;

                Mesh mesh = meshFilter.sharedMesh;

                if (mesh == null)
                    continue;

                meshObjectCount++;

                totalVertices += mesh.vertexCount;
                totalTriangles += GetTriangleCount(mesh);
            }

            // Ambil SkinnedMeshRenderer
            SkinnedMeshRenderer[] skinnedMeshes =
                selectedObject.GetComponentsInChildren<SkinnedMeshRenderer>(true);

            foreach (SkinnedMeshRenderer skinnedMesh in skinnedMeshes)
            {
                if (!ShouldCount(skinnedMesh.gameObject))
                    continue;

                Mesh mesh = skinnedMesh.sharedMesh;

                if (mesh == null)
                    continue;

                meshObjectCount++;

                totalVertices += mesh.vertexCount;
                totalTriangles += GetTriangleCount(mesh);
            }
        }

        Debug.Log(
            "========== TRIS COUNTER ==========\n" +
            $"Mode: {countMode}\n" +
            $"Selected Root Objects: {totalObjects}\n" +
            $"Mesh Objects Counted: {meshObjectCount}\n" +
            $"Vertices: {totalVertices:N0}\n" +
            $"Triangles: {totalTriangles:N0}\n" +
            "================================="
        );

        Repaint();
    }

    private bool ShouldCount(GameObject obj)
    {
        switch (countMode)
        {
            case CountMode.ActiveOnly:
                return obj.activeInHierarchy;

            case CountMode.InactiveOnly:
                return !obj.activeInHierarchy;

            case CountMode.ActiveAndInactive:
                return true;
        }

        return false;
    }

    private int GetTriangleCount(Mesh mesh)
    {
        int triangles = 0;

        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            if (mesh.GetTopology(i) == MeshTopology.Triangles)
            {
                triangles += (int)mesh.GetIndexCount(i) / 3;
            }
        }

        return triangles;
    }
}