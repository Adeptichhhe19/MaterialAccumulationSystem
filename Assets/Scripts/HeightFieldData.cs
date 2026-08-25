using System;
using UnityEngine;

public class HeightFieldData
{
    private readonly float[] heights;

    public float Width { get; }
    public float Depth { get; }

    public int Columns { get; }
    public int Rows { get; }

    public float CellSizeX { get; }
    public float CellSizeZ { get; }

    public float MaxHeight { get; private set; }

    internal float[] Heights => heights;

    public HeightFieldData(float width, float depth, int columns, int rows)
    {
        Width = width;
        Depth = depth;
        Columns = columns;
        Rows = rows;

        CellSizeX = width / (columns - 1);
        CellSizeZ = depth / (rows - 1);

        heights = new float[columns * rows];
    }

    public float GetHeight(int x, int z)
    {
        int index = z * Columns + x;
        return heights[index];
    }

    public void AddHeight(int x, int z, float amount)
    {
        if (amount <= 0f)
            return;

        int index = z * Columns + x;

        heights[index] += amount;
        MaxHeight = Mathf.Max(MaxHeight, heights[index]);
    }

    public void Reset()
    {
        Array.Clear(heights, 0, heights.Length);
        MaxHeight = 0f;
    }
}