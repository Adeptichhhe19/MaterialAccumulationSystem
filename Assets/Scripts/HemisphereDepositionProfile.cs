using UnityEngine;

public class HemisphereDepositionProfile : IMaterialDepositionProfile
{
    public static readonly HemisphereDepositionProfile Instance =
        new HemisphereDepositionProfile();

    public float Evaluate(float normalizedDistanceSquared)
    {
        float value = 1f - normalizedDistanceSquared;
        return Mathf.Sqrt(Mathf.Max(0f, value));
    }
}