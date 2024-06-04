using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CubeMeshGenerator : MonoBehaviour
{
  private MeshFilter meshFilter;
  private MeshRenderer meshRenderer;
  private Mesh mesh;

  [SerializeField]
  private int gridSize = 8;

  public void GenerateMesh(GameObject baseObj)
  {
    // Add or get the MeshFilter
    if (!baseObj.TryGetComponent<MeshFilter>(out meshFilter))
    {
      meshFilter = baseObj.AddComponent<MeshFilter>();
    }

    // Add or get the MeshRenderer
    if (!baseObj.TryGetComponent<MeshRenderer>(out meshRenderer))
    {
      meshRenderer = baseObj.AddComponent<MeshRenderer>();
    }

    // Create the mesh
    mesh = new Mesh();
    meshFilter.mesh = mesh;

    List<Vector3> vertices = new();
    List<int> triangles = new();
    List<Vector3> normals = new();

    // Six faces of the cube
    for (int i = 0; i < 6; i++)
    {
      CreateFace(i, vertices, triangles, normals);
    }

    mesh.vertices = vertices.ToArray();
    mesh.triangles = triangles.ToArray();
    mesh.normals = normals.ToArray();
    mesh.RecalculateNormals();
  }

  private void CreateFace(int faceIndex, List<Vector3> vertices, List<int> triangles, List<Vector3> normals)
  {
    Vector3 normal;
    Vector3 up;
    Vector3 right;

    // Determine the face orientation
    switch (faceIndex)
    {
      case 0: // Front
        normal = Vector3.back;
        up = Vector3.up;
        right = Vector3.right;
        break;
      case 1: // Back
        normal = Vector3.forward;
        up = Vector3.up;
        right = Vector3.left;
        break;
      case 2: // Left
        normal = Vector3.left;
        up = Vector3.up;
        right = Vector3.back;
        break;
      case 3: // Right
        normal = Vector3.right;
        up = Vector3.up;
        right = Vector3.forward;
        break;
      case 4: // Top
        normal = Vector3.down;
        up = Vector3.back;
        right = Vector3.right;
        break;
      case 5: // Bottom
        normal = Vector3.up;
        up = Vector3.forward;
        right = Vector3.right;
        break;
      default:
        return;
    }

    int vertexOffset = vertices.Count;

    // Create the grid on this face
    for (int y = 0; y <= gridSize; y++)
    {
      for (int x = 0; x <= gridSize; x++)
      {
        Vector3 point = normal * 0.5f + up * ((float)y / gridSize - 0.5f) + right * ((float)x / gridSize - 0.5f);
        vertices.Add(point);
        normals.Add(normal); // Add normals
      }
    }

    // Create triangles
    for (int y = 0; y < gridSize; y++)
    {
      for (int x = 0; x < gridSize; x++)
      {
        int i0 = vertexOffset + y * (gridSize + 1) + x;
        int i1 = vertexOffset + y * (gridSize + 1) + x + 1;
        int i2 = vertexOffset + (y + 1) * (gridSize + 1) + x;
        int i3 = vertexOffset + (y + 1) * (gridSize + 1) + x + 1;

        // Two right triangles for each grid cell
        triangles.AddRange(new int[] { i0, i2, i1 });
        triangles.AddRange(new int[] { i1, i2, i3 });
      }
    }
  }
}
