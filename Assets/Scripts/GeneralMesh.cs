using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;


/**
 * TODO: Add more methods.
 */
public class GeneralMesh
{
    public readonly Vector3[] vertices;
    public readonly Vector3[] normals;
    public readonly int[] primitives;
    public readonly int nGon;

    public GeneralMesh(Vector3[] vertices, Vector3[] normals, int[] primitives, int nGon)
    {
        this.vertices = vertices;
        this.normals = normals;
        this.primitives = primitives;
        this.nGon = nGon;
    }
}
