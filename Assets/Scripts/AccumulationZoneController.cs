using UnityEngine;

[DefaultExecutionOrder(-100)]
public class AccumulationZoneController :
    MonoBehaviour,
    IAccumulationZoneState
{
    [Header("Dependencies")]
    [SerializeField] private MaterialAccumulationSurface surface;
    [SerializeField] private MonoBehaviour inputSource;

    [Header("Movement")]
    [SerializeField] private float movementSpeed = 6f;

    [Header("Radius")]
    [SerializeField] private float baseRadius = 1.6f;
    [SerializeField] private float radiusAmplitude = 0.5f;
    [SerializeField] private float radiusFrequency = 0.25f;

    [SerializeField]
    private AnimationCurve radiusCurve =
        new AnimationCurve(
            new Keyframe(0f, 0f),
            new Keyframe(0.25f, 1f),
            new Keyframe(0.75f, -1f),
            new Keyframe(1f, 0f));

    private IAccumulationInput input;
    private float radiusTime;

    public Vector2 PreviousPosition { get; private set; }
    public Vector2 Position { get; private set; }

    public float PreviousRadius { get; private set; }
    public float Radius { get; private set; }

    private void Awake()
    {
        input = inputSource as IAccumulationInput;

        if (input == null || surface == null)
        {
            Debug.LogError("Zone dependencies are not configured.", this);
            enabled = false;
            return;
        }

        Radius = CalculateRadius();
        PreviousRadius = Radius;

        Position = Vector2.zero;
        Position = surface.ClampPosition(Position, Radius);

        PreviousPosition = Position;
    }

    private void Update()
    {
        PreviousPosition = Position;
        PreviousRadius = Radius;

        radiusTime += Time.deltaTime;
        Radius = CalculateRadius();

        Position +=
            input.Movement *
            movementSpeed *
            Time.deltaTime;

        Position = surface.ClampPosition(Position, Radius);
    }

    private float CalculateRadius()
    {
        float phase = Mathf.Repeat(
            radiusTime * radiusFrequency,
            1f);

        float curveValue = radiusCurve.Evaluate(phase);

        return Mathf.Max(
            0.05f,
            baseRadius + curveValue * radiusAmplitude);
    }
}