using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Generate a hollow sphere mesh around the base mesh
/// </summary>
public class HollowSphere
{
    private int longitudeSegments;
    private int latitudeSegments;

    public void GenerateHollowSphere(GameObject BaseObj, MeshFilter meshFilter, MeshRenderer meshRenderer)
    {
        float outerRadius = 2.825f;
        float thickness = 2.0f;
        longitudeSegments = 64;
        latitudeSegments = 64;

        Mesh outerMesh = GenerateSphereMesh(outerRadius);
        Mesh innerMesh = GenerateSphereMesh(outerRadius - thickness, true);

        Mesh combinedMesh = CombineMeshes(outerMesh, innerMesh);
        meshFilter.mesh = combinedMesh;

        AddMeshCollider(outerMesh, false, meshFilter.transform);
        AddMeshCollider(innerMesh, true, meshFilter.transform);

        meshFilter.transform.position = BaseObj.transform.position;
    }

    private Mesh GenerateSphereMesh(float radius, bool invert = false)
    {
        Mesh mesh = new();

        List<Vector3> vertices = new();
        List<int> triangles = new();

        // generate vertices
        for (int i = 0; i <= latitudeSegments; i++)
        {
            // latitude angles
            float lat = Mathf.PI * i / latitudeSegments;
            float sinLat = Mathf.Sin(lat);
            float cosLat = Mathf.Cos(lat);

            for (int j = 0; j <= longitudeSegments; j++)
            {
                float lon = 2 * Mathf.PI * j / longitudeSegments; // longitude angles
                vertices.Add(new Vector3(radius * sinLat * Mathf.Cos(lon), // X coordinate
                                         radius * cosLat, // Y coordinate
                                         radius * sinLat * Mathf.Sin(lon))); // Z coordinate
            }
        }

        // generate triangles
        for (int i = 0; i < latitudeSegments; i++)
        {
            for (int j = 0; j < longitudeSegments; j++)
            {
                int current = i * (longitudeSegments + 1) + j;
                int next = current + longitudeSegments + 1;

                if (invert)
                {
                    // inverted triangles for inner mesh
                    triangles.Add(current);
                    triangles.Add(next + 1);
                    triangles.Add(next);

                    triangles.Add(current);
                    triangles.Add(current + 1);
                    triangles.Add(next + 1);
                }
                else
                {
                    // normal triangles for outer mesh
                    triangles.Add(current);
                    triangles.Add(next);
                    triangles.Add(next + 1);

                    triangles.Add(current);
                    triangles.Add(next + 1);
                    triangles.Add(current + 1);
                }
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateNormals();

        return mesh;
    }

    private Mesh CombineMeshes(Mesh mesh1, Mesh mesh2)
    {
        CombineInstance[] combine = new CombineInstance[2];
        combine[0].mesh = mesh1;
        combine[0].transform = Matrix4x4.identity;
        combine[1].mesh = mesh2;
        combine[1].transform = Matrix4x4.identity;

        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine, true, false);
        combinedMesh.RecalculateNormals();
        return combinedMesh;
    }

    private void AddMeshCollider(Mesh mesh, bool isTrigger, Transform transform)
    {
        GameObject colliderObj = new GameObject(isTrigger ? "InnerCollider" : "OuterCollider");
        colliderObj.transform.SetParent(transform);
        colliderObj.transform.localPosition = Vector3.zero;
        colliderObj.transform.localRotation = Quaternion.identity;

        MeshCollider meshCollider = colliderObj.AddComponent<MeshCollider>();
        meshCollider.sharedMesh = mesh;
        meshCollider.convex = false;
    }
}
