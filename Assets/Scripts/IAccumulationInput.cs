using UnityEngine;

public interface IAccumulationInput
{
    Vector2 Movement { get; }

    bool AccumulationHeld { get; }

    bool ResetPressed { get; }
}