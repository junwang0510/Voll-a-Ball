using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        // Check
        Debug.Log($"Vertices: {vertices.Length}, " +
                  $"Normals: {normals.Length}, " +
                  $"Primitives (triangles): {primitives.Length / nGon}, " +
                  $"nGon: {nGon}");
        
        // Generate wall vertex pairs
        var wallVertexPairs = MazeGenerator.Generate(generalMesh, "DFS", 0);
        
        // Checks
        Debug.Log($"Wall vertex pairs: {wallVertexPairs.Length}");
    }
}
