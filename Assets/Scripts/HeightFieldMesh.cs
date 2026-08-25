using System;
using UnityEngine;
using UnityEngine.Rendering;

public class HeightFieldMesh : IDisposable
{
    private readonly HeightFieldData data;
    private readonly Mesh mesh;

    private readonly Vector3[] vertices;
    private readonly Vector2[] uv;
    private readonly int[] triangles;

    public Mesh Mesh => mesh;

    public HeightFieldMesh(HeightFieldData data)
    {
        this.data = data;

        int vertexCount = data.Columns * data.Rows;
        int quadCount = (data.Columns - 1) * (data.Rows - 1);

        vertices = new Vector3[vertexCount];
        uv = new Vector2[vertexCount];
        triangles = new int[quadCount * 6];

        mesh = new Mesh();
        mesh.name = "Accumulation Surface";
        mesh.MarkDynamic();

        if (vertexCount > ushort.MaxValue)
            mesh.indexFormat = IndexFormat.UInt32;

        CreateVertices();
        CreateTriangles();

        mesh.SetVertices(vertices);
        mesh.SetUVs(0, uv);
        mesh.SetTriangles(triangles, 0);

        UpdateGeometry();
    }

    private void CreateVertices()
    {
        float startX = -data.Width * 0.5f;
        float startZ = -data.Depth * 0.5f;

        for (int z = 0; z < data.Rows; z++)
        {
            for (int x = 0; x < data.Columns; x++)
            {
                int index = z * data.Columns + x;

                vertices[index] = new Vector3(
                    startX + x * data.CellSizeX,
                    0f,
                    startZ + z * data.CellSizeZ);

                uv[index] = new Vector2(
                    x / (float)(data.Columns - 1),
                    z / (float)(data.Rows - 1));
            }
        }
    }

    private void CreateTriangles()
    {
        int triangleIndex = 0;

        for (int z = 0; z < data.Rows - 1; z++)
        {
            for (int x = 0; x < data.Columns - 1; x++)
            {
                int vertexIndex = z * data.Columns + x;
                int nextRow = vertexIndex + data.Columns;

                triangles[triangleIndex++] = vertexIndex;
                triangles[triangleIndex++] = nextRow;
                triangles[triangleIndex++] = vertexIndex + 1;

                triangles[triangleIndex++] = vertexIndex + 1;
                triangles[triangleIndex++] = nextRow;
                triangles[triangleIndex++] = nextRow + 1;
            }
        }
    }

    public void UpdateGeometry()
    {
        float[] heights = data.Heights;

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i].y = heights[i];
        }

        mesh.SetVertices(vertices);
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();
    }

    public void Dispose()
    {
        if (Application.isPlaying)
            UnityEngine.Object.Destroy(mesh);
        else
            UnityEngine.Object.DestroyImmediate(mesh);
    }
}