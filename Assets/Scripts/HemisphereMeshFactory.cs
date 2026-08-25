using UnityEngine;

public static class HemisphereMeshFactory
{
    public static Mesh Create(int segments, int rings)
    {
        segments = Mathf.Max(8, segments);
        rings = Mathf.Max(2, rings);

        int rowLength = segments + 1;
        int vertexCount = 1 + rings * rowLength;
        int triangleCount =
            segments + (rings - 1) * segments * 2;

        Vector3[] vertices = new Vector3[vertexCount];
        Vector3[] normals = new Vector3[vertexCount];
        Vector2[] uv = new Vector2[vertexCount];
        int[] triangles = new int[triangleCount * 3];

        // Верхняя точка полусферы.
        vertices[0] = Vector3.up;
        normals[0] = Vector3.up;
        uv[0] = new Vector2(0.5f, 1f);

        int vertexIndex = 1;

        for (int ring = 1; ring <= rings; ring++)
        {
            float ringProgress = ring / (float)rings;
            float angleY = ringProgress * Mathf.PI * 0.5f;

            float ringRadius = Mathf.Sin(angleY);
            float height = Mathf.Cos(angleY);

            for (int segment = 0; segment <= segments; segment++)
            {
                float segmentProgress =
                    segment / (float)segments;

                float angle =
                    segmentProgress * Mathf.PI * 2f;

                Vector3 position = new Vector3(
                    Mathf.Cos(angle) * ringRadius,
                    height,
                    Mathf.Sin(angle) * ringRadius);

                vertices[vertexIndex] = position;
                normals[vertexIndex] = position.normalized;
                uv[vertexIndex] = new Vector2(
                    segmentProgress,
                    ringProgress);

                vertexIndex++;
            }
        }

        int triangleIndex = 0;

        for (int segment = 0; segment < segments; segment++)
        {
            triangles[triangleIndex++] = 0;
            triangles[triangleIndex++] = 1 + segment + 1;
            triangles[triangleIndex++] = 1 + segment;
        }

        for (int ring = 0; ring < rings - 1; ring++)
        {
            int upperRow = 1 + ring * rowLength;
            int lowerRow = upperRow + rowLength;

            for (int segment = 0; segment < segments; segment++)
            {
                int upperLeft = upperRow + segment;
                int upperRight = upperLeft + 1;
                int lowerLeft = lowerRow + segment;
                int lowerRight = lowerLeft + 1;

                triangles[triangleIndex++] = upperLeft;
                triangles[triangleIndex++] = upperRight;
                triangles[triangleIndex++] = lowerLeft;

                triangles[triangleIndex++] = upperRight;
                triangles[triangleIndex++] = lowerRight;
                triangles[triangleIndex++] = lowerLeft;
            }
        }

        Mesh mesh = new Mesh();
        mesh.name = "Accumulation Zone Hemisphere";

        mesh.vertices = vertices;
        mesh.normals = normals;
        mesh.uv = uv;
        mesh.triangles = triangles;

        mesh.RecalculateBounds();

        return mesh;
    }
}