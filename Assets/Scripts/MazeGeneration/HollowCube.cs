using System.Collections.Generic;
using UnityEngine;

public class HollowCube
{
  private float Length, Width, Height;

  public void GenerateHollowCube(GameObject BaseObj, MeshFilter meshFilter, MeshRenderer meshRenderer, GameObject parent = null)
  {
    Length = 4.55f;
    Width = 4.55f;
    Height = 4.55f;

    Mesh outerMesh = GenerateCubeMesh(Length, Width, Height);
    Mesh innerMesh = GenerateCubeMesh(Length - 1.8f, Width - 1.8f, Height - 1.8f, true);

    Mesh combinedMesh = CombineMeshes(outerMesh, innerMesh);
    meshFilter.mesh = combinedMesh;

    AddMeshCollider(combinedMesh, false, meshFilter.transform);

    meshFilter.transform.position = BaseObj.transform.position;

    // Set the parent
    if (parent != null)
    {
      meshFilter.transform.SetParent(parent.transform);
    }
  }

  private Mesh GenerateCubeMesh(float length, float width, float height, bool invert = false)
  {
    Mesh mesh = new Mesh();

    float halfLength = length * 0.5f;
    float halfWidth = width * 0.5f;
    float halfHeight = height * 0.5f;

    Vector3[] vertices = new Vector3[]
    {
            new Vector3(-halfLength, -halfHeight, -halfWidth),
            new Vector3(halfLength, -halfHeight, -halfWidth),
            new Vector3(halfLength, halfHeight, -halfWidth),
            new Vector3(-halfLength, halfHeight, -halfWidth),
            new Vector3(-halfLength, -halfHeight, halfWidth),
            new Vector3(halfLength, -halfHeight, halfWidth),
            new Vector3(halfLength, halfHeight, halfWidth),
            new Vector3(-halfLength, halfHeight, halfWidth)
    };

    int[] triangles = new int[]
    {
            0, 2, 1, 0, 3, 2, // Front face
            2, 3, 6, 6, 3, 7, // Top face
            0, 7, 3, 0, 4, 7, // Left face
            1, 6, 5, 1, 2, 6, // Right face
            4, 5, 6, 4, 6, 7, // Back face
            0, 1, 5, 0, 5, 4  // Bottom face
    };

    if (!invert)
    {
      for (int i = 0; i < triangles.Length; i += 3)
      {
        // Swap two vertices to change the winding order
        int temp = triangles[i];
        triangles[i] = triangles[i + 1];
        triangles[i + 1] = temp;
      }
    }

    mesh.vertices = vertices;
    mesh.triangles = triangles;
    mesh.RecalculateNormals();

    return mesh;
  }

  private Mesh CombineMeshes(Mesh outerMesh, Mesh innerMesh)
  {
    Mesh combinedMesh = new Mesh();

    List<Vector3> vertices = new List<Vector3>();
    List<int> triangles = new List<int>();

    vertices.AddRange(outerMesh.vertices);
    int outerVertexCount = outerMesh.vertexCount;
    for (int i = 0; i < outerMesh.triangles.Length; i++)
    {
      triangles.Add(outerMesh.triangles[i]);
    }

    int innerOffset = outerVertexCount;
    vertices.AddRange(innerMesh.vertices);
    for (int i = 0; i < innerMesh.triangles.Length; i++)
    {
      triangles.Add(innerMesh.triangles[i] + innerOffset);
    }

    combinedMesh.vertices = vertices.ToArray();
    combinedMesh.triangles = triangles.ToArray();
    combinedMesh.RecalculateNormals();

    return combinedMesh;
  }

  private void AddMeshCollider(Mesh mesh, bool isTrigger, Transform transform)
  {
    MeshCollider meshCollider = transform.gameObject.AddComponent<MeshCollider>();
    meshCollider.sharedMesh = mesh;
    meshCollider.convex = false;
    meshCollider.isTrigger = isTrigger;
  }
}
