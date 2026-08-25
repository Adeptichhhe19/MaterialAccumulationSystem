public class FlatProfile : IMaterialDepositionProfile
{
    public float Evaluate(float normalizedDistanceSquared)
    {
        return 1f;
    }
}