using UnityEngine;

public class DepositionSolver
{
    private readonly HeightFieldData data;
    private readonly IMaterialDepositionProfile profile;
    private readonly float spatialStep;

    public DepositionSolver(
        HeightFieldData data,
        IMaterialDepositionProfile profile)
    {
        this.data = data;
        this.profile = profile;

        spatialStep = Mathf.Min(
            data.CellSizeX,
            data.CellSizeZ) * 0.5f;
    }

    public bool DepositSweep(
        Vector2 previousPosition,
        Vector2 currentPosition,
        float previousRadius,
        float currentRadius,
        float deltaTime,
        float accumulationSpeed)
    {
        if (deltaTime <= 0f || accumulationSpeed <= 0f)
            return false;

        float distance = Vector2.Distance(
            previousPosition,
            currentPosition);

        float radiusChange = Mathf.Abs(
            currentRadius - previousRadius);

        int positionSteps =
            Mathf.CeilToInt(distance / spatialStep);

        int radiusSteps =
            Mathf.CeilToInt(radiusChange / spatialStep);

        int stepCount = Mathf.Max(
            1,
            Mathf.Max(positionSteps, radiusSteps));

        float amountPerStep =
            accumulationSpeed * deltaTime / stepCount;

        bool changed = false;

        for (int step = 0; step < stepCount; step++)
        {
            float t = (step + 0.5f) / stepCount;

            Vector2 position = Vector2.Lerp(
                previousPosition,
                currentPosition,
                t);

            float radius = Mathf.Lerp(
                previousRadius,
                currentRadius,
                t);

            if (DepositAt(position, radius, amountPerStep))
                changed = true;
        }

        return changed;
    }

    private bool DepositAt(
        Vector2 center,
        float radius,
        float amount)
    {
        if (radius <= 0f)
            return false;

        float halfWidth = data.Width * 0.5f;
        float halfDepth = data.Depth * 0.5f;

        int minX = Mathf.Max(0, Mathf.CeilToInt(
            (center.x - radius + halfWidth) / data.CellSizeX));

        int maxX = Mathf.Min(data.Columns - 1, Mathf.FloorToInt(
            (center.x + radius + halfWidth) / data.CellSizeX));

        int minZ = Mathf.Max(0, Mathf.CeilToInt(
            (center.y - radius + halfDepth) / data.CellSizeZ));

        int maxZ = Mathf.Min(data.Rows - 1, Mathf.FloorToInt(
            (center.y + radius + halfDepth) / data.CellSizeZ));

        float radiusSquared = radius * radius;
        bool changed = false;

        for (int z = minZ; z <= maxZ; z++)
        {
            float pointZ =
                -halfDepth + z * data.CellSizeZ;

            for (int x = minX; x <= maxX; x++)
            {
                float pointX =
                    -halfWidth + x * data.CellSizeX;

                float offsetX = pointX - center.x;
                float offsetZ = pointZ - center.y;

                float distanceSquared =
                    offsetX * offsetX + offsetZ * offsetZ;

                if (distanceSquared >= radiusSquared)
                    continue;

                float normalizedDistance =
                    distanceSquared / radiusSquared;

                float weight =
                    profile.Evaluate(normalizedDistance);

                data.AddHeight(x, z, amount * weight);
                changed = true;
            }
        }

        return changed;
    }
}