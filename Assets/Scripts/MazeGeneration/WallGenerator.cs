using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class WallGenerator
{
  // Game Controller

  // Object to generate walls on
  private GameObject BaseObj;
  private Vector3[] vertices, normals;
  private Tuple<Vector3, Vector3>[] wallVerticesPairs;
  private HashSet<Vector3> createdVertices;

  public void Generate(GameObject obj)
  {
    BaseObj = obj;
    MeshFilter meshFilter = BaseObj.GetComponent<MeshFilter>();
    Mesh mesh = meshFilter.mesh;
    vertices = mesh.vertices;
    for (int i = 0; i < vertices.Length; i++)
    {
      vertices[i] = BaseObj.transform.TransformPoint(vertices[i]);
    }
    normals = mesh.normals;

    // Shuffling the vertices and normals but keeping the pairs
    System.Random random = new System.Random();
    for (int i = 0; i < vertices.Length; i++)
    {
      int randomIndex = random.Next(i, vertices.Length);
      Vector3 tempVertex = vertices[i];
      vertices[i] = vertices[randomIndex];
      vertices[randomIndex] = tempVertex;

      Vector3 tempNormal = normals[i];
      normals[i] = normals[randomIndex];
      normals[randomIndex] = tempNormal;
    }

    SortVertices();

    createdVertices = new HashSet<Vector3>();
    GenerateWalls();
  }

  private void GenerateWalls()
  {
      for (int i = 1; i < vertices.Length; i++)
      {
          CreateWalls(vertices[i - 1], vertices[i], normals[i - 1], normals[i]);
      }
  }

  private void CreateWalls(Vector3 start, Vector3 end, Vector3 StartN, Vector3 EndN)
  {
      float distance = Vector3.Distance(start, end);
      if (distance < 0.1f || distance > 0.6f)  // 0.25f is just for my testing case
          return;

      if (ClosestDistance(start) < 0.3f || ClosestDistance(end) < 0.3f)
          return;

      Material material = new Material(Shader.Find("Standard"));
      material.color = Color.black;

      // Create red cylinder to show the ewach vertex with the normal of that vertex
      if (!createdVertices.Contains(start))
      {
          GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
          cylinder.transform.position = start;
          cylinder.transform.localScale = new Vector3(0.14f, 0.27f, 0.14f);
          cylinder.transform.rotation = Quaternion.LookRotation(StartN);
          cylinder.transform.Rotate(90, 0, 0);
          cylinder.transform.parent = BaseObj.transform;

          // Set the color of the cylinder to black
          cylinder.GetComponent<Renderer>().material = material;
          createdVertices.Add(start);
      }

      if (!createdVertices.Contains(end))
      {
          GameObject cylinder = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
          cylinder.transform.position = end;
          cylinder.transform.localScale = new Vector3(0.14f, 0.27f, 0.14f);
          cylinder.transform.rotation = Quaternion.LookRotation(EndN);
          cylinder.transform.Rotate(90, 0, 0);
          cylinder.transform.parent = BaseObj.transform;

          // Set the color of the cylinder to black
          cylinder.GetComponent<Renderer>().material = material;
          createdVertices.Add(end);
      }

      Vector3 center = (start + end) / 2;
      GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
      wall.transform.position = center;
      wall.transform.localScale = new Vector3(0.1f, 0.5f, distance * 1.1f);

      wall.transform.rotation = Quaternion.LookRotation(end - start, (StartN + EndN) / 2);
      wall.transform.parent = BaseObj.transform;

      // Set the color of the wall to black
      wall.GetComponent<Renderer>().material = material;
  }

  private void SortVertices()
  {
      if (vertices == null || vertices.Length == 0 || normals == null || normals.Length != vertices.Length)
          return;

      List<Vector3> sortedVertices = new List<Vector3>();
      List<Vector3> sortedNormals = new List<Vector3>();
      HashSet<int> visitedIndices = new HashSet<int>();

      // Start with the first vertex
      int currentIndex = 0;
      sortedVertices.Add(vertices[currentIndex]);
      sortedNormals.Add(normals[currentIndex]);
      visitedIndices.Add(currentIndex);

      while (sortedVertices.Count < vertices.Length)
      {
          float closestDistance = float.MaxValue;
          int closestIndex = -1;

          for (int i = 0; i < vertices.Length; i++)
          {
              if (!visitedIndices.Contains(i))
              {
                  float distance = Vector3.Distance(vertices[currentIndex], vertices[i]);
                  if (distance < closestDistance)
                  {
                      closestDistance = distance;
                      closestIndex = i;
                  }
              }
          }

          // Add the closest vertex and its normal to the sorted lists
          currentIndex = closestIndex;
          sortedVertices.Add(vertices[currentIndex]);
          sortedNormals.Add(normals[currentIndex]);
          visitedIndices.Add(currentIndex);
      }

      vertices = sortedVertices.ToArray();
      normals = sortedNormals.ToArray();
  }

  private float ClosestDistance(Vector3 curr)
  {
      float closestDistance = float.MaxValue;
      int closestIndex = -1;

      // Looking through createdVertices
      foreach (Vector3 vertex in createdVertices)
      {
          if (vertex == curr)
              continue;
          float distance = Vector3.Distance(curr, vertex);
          if (distance < closestDistance)
          {
              closestDistance = distance;
              closestIndex = createdVertices.GetHashCode();
          }
      }

      return closestDistance;
  }
}
