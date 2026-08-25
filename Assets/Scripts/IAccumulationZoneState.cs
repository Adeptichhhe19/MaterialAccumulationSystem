using UnityEngine;

public interface IAccumulationZoneState
{
    Vector2 PreviousPosition { get; }

    Vector2 Position { get; }

    float PreviousRadius { get; }

    float Radius { get; }
}