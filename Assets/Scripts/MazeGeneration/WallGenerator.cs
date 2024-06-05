using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generate walls based on wallVertexPairs (class Tuple<Vector3, Vector3>[])
/// </summary>
public class WallGenerator
{
    private GameObject BaseObj;
    private Vector3[] vertices, normals;
    private Tuple<Vector3, Vector3>[] wallVerticesPairs;
    private HashSet<Vector3> createdVertices;
    private bool redCylinderCreated = false;
    private Dictionary<Vector3, Vector3> vertexToNormal = new();

    public void Generate(GameObject obj)
    {
        BaseObj = obj;
        MeshFilter meshFilter = BaseObj.GetComponent<MeshFilter>();
        Mesh mesh = meshFilter.mesh;

        // Get the scale of the object
        Vector3 scale = BaseObj.transform.localScale;

        // Scale the normals
        normals = mesh.normals;
        for (int i = 0; i < normals.Length; i++)
        {
            normals[i] = new Vector3(normals[i].x / Mathf.Pow(scale.x, 2), normals[i].y / Mathf.Pow(scale.y, 2), normals[i].z / Mathf.Pow(scale.z, 2));
        }

        // Scale the vertices
        vertices = mesh.vertices;
        // for (int i = 0; i < vertices.Length; i++)
        // {
        //     vertices[i] = new Vector3(vertices[i].x * 2.0f, vertices[i].y * 2.0f, vertices[i].z * 2.0f);
        // }

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = BaseObj.transform.TransformPoint(vertices[i]);
        }

        // Put the vertices and normals in vertexToNormal dictionary
        for (int i = 0; i < vertices.Length; i++)
        {
            vertexToNormal[vertices[i]] = normals[i];
        }

        GeneralMesh generalMesh = new(vertices, normals, mesh.triangles, 3);
        wallVerticesPairs = MazeGenerator.Generate(generalMesh);
        createdVertices = new HashSet<Vector3>();
        GenerateWallsFromPairs();
    }

    private void GenerateWallsFromPairs()
    {
        foreach (Tuple<Vector3, Vector3> pair in wallVerticesPairs)
            CreateWalls(pair.Item1, pair.Item2, vertexToNormal[pair.Item1], vertexToNormal[pair.Item2]);
    }

    private void CreateWalls(Vector3 start, Vector3 end, Vector3 StartN, Vector3 EndN)
    {
        float distance = Vector3.Distance(start, end);
        // if (distance < 0.1f || distance > 0.6f)  // 0.25f is just for my testing case
        //     return;

        if (ClosestDistance(start) < 0.3f || ClosestDistance(end) < 0.3f)
            return;

        Material material = new(Shader.Find("Standard"))
        {
            color = Color.black
        };

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

            // Randomly set one and only one cylinder to red (rest = black)
            if (!redCylinderCreated && UnityEngine.Random.Range(0, 10) == 0)
            {
                cylinder.GetComponent<Renderer>().material.color = Color.red;
                cylinder.tag = "RedCylinder";
                redCylinderCreated = true;
            }
            else
            {
                cylinder.GetComponent<Renderer>().material = material;
            }

            createdVertices.Add(end);
        }

        Vector3 center = (start + end) / 2;
        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
        wall.transform.position = center;
        wall.transform.localScale = new Vector3(0.1f, 0.5f, distance);
        Vector3 direction = end - start;
        Vector3 right = Vector3.Cross(direction, StartN + EndN);
        Vector3 upward = Vector3.Cross(right, direction);

        wall.transform.rotation = Quaternion.LookRotation(direction, upward);
        wall.transform.parent = BaseObj.transform;

        // Set the color of the wall to black
        wall.GetComponent<Renderer>().material = material;
    }

    private float ClosestDistance(Vector3 curr)
    {
        float closestDistance = float.MaxValue;

        foreach (Vector3 vertex in createdVertices)
        {
            if (vertex == curr)
                continue;

            float distance = Vector3.Distance(curr, vertex);
            if (distance < closestDistance)
                closestDistance = distance;
        }

        return closestDistance;
    }
}
