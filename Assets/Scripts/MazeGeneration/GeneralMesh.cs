using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEngine;

/// <summary>
/// Manage vertices, vertex normals, and primitives of a mesh
/// </summary>
public class GeneralMesh
{
    public readonly Vector3[] vertices;
    public readonly Vector3[] normals;
    public readonly int[] primitives;
    public readonly int nGon;
    public readonly bool isDeduplicated;

    public GeneralMesh(Vector3[] vertices, Vector3[] normals, int[] primitives, int nGon)
    {
        this.nGon = nGon;

        if (vertices.Length == new HashSet<Vector3>(vertices).Count)
        {
            isDeduplicated = false;
            this.vertices = vertices;
            this.normals = normals;
            this.primitives = primitives;
            return;
        }

        // Deduplicate vertices, normals, and primitives.

        // Map Vector3 to its first occurring old vertex id.
        var vecToOldVid = new Dictionary<Vector3, int>();
        var oldVidToNewVid = new Dictionary<int, int>();
        var newVidToOldVid = new Dictionary<int, int>();

        var n = 0;
        for (var oldVid = 0; oldVid < vertices.Length; oldVid++)
        {
            var vec = vertices[oldVid];
            if (vecToOldVid.ContainsKey(vec))
            {
                oldVidToNewVid[oldVid] = oldVidToNewVid[vecToOldVid[vec]];
                continue;
            }
            vecToOldVid[vec] = oldVid;
            oldVidToNewVid[oldVid] = n;
            newVidToOldVid[n] = oldVid;
            n++;
        }

        primitives = primitives.ToArray();
        for (var i = 0; i < primitives.Length; i++)
        {
            var oldVid = primitives[i];
            var newVid = oldVidToNewVid[oldVid];
            primitives[i] = newVid;
        }

        var newVertices = new Vector3[n];
        var newNormals = new Vector3[n];
        for (var newVid = 0; newVid < n; newVid++)
        {
            var oldVid = newVidToOldVid[newVid];
            newVertices[newVid] = vertices[oldVid];
            newNormals[newVid] = normals[oldVid];
        }

        isDeduplicated = true;
        this.vertices = newVertices;
        this.normals = newNormals;
        this.primitives = primitives;
        this.nGon = nGon;
    }

    public GeneralMesh(Mesh mesh)
        : this(mesh.vertices, mesh.normals, mesh.triangles, 3) {}
}
