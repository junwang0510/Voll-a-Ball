using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Extract vertices, normals, and primitives from a mesh
/// </summary>
public class MeshExtractor : MonoBehaviour
{
    private void OnEnable()
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            Debug.LogError("MeshFilter not found on this GameObject!");
            return;
        }

        // Create GeneralMesh based on the current mesh
        Mesh mesh = meshFilter.mesh;
        Vector3[] vertices = mesh.vertices;
        Vector3[] normals = mesh.normals;
        int[] primitives = mesh.triangles;
        int nGon = 3;
        GeneralMesh generalMesh = new GeneralMesh(vertices, normals, primitives, nGon);

        // Generate wall vertex pairs
        var wallVertexPairs = MazeGenerator.Generate(generalMesh, "DFS", 0);

        // Checks
        Debug.Log($"Vertices: {vertices.Length}, " +
                  $"Normals: {normals.Length}, " +
                  $"Primitives (triangles): {primitives.Length / nGon}, " +
                  $"nGon: {nGon}");
        Debug.Log($"Wall vertex pairs: {wallVertexPairs.Length}");
    }
}
