using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MaterialAccumulationSurface : MonoBehaviour
{
    [Header("Grid")]
    [SerializeField]
    private Vector2 surfaceSize =
        new Vector2(16f, 10f);

    [SerializeField]
    private Vector2Int vertexResolution =
        new Vector2Int(129, 81);

    [SerializeField] private Material surfaceMaterial;

    [Header("Accumulation")]
    [SerializeField] private float accumulationSpeed = 0.8f;

    private HeightFieldData data;
    private HeightFieldMesh meshView;
    private DepositionSolver solver;

    public Vector2 Size => surfaceSize;
    public HeightFieldData Data => data;

    private void Awake()
    {
        data = new HeightFieldData(
            surfaceSize.x,
            surfaceSize.y,
            vertexResolution.x,
            vertexResolution.y);

        meshView = new HeightFieldMesh(data);

        solver = new DepositionSolver(
            data,
            HemisphereDepositionProfile.Instance);

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        meshFilter.sharedMesh = meshView.Mesh;

        if (surfaceMaterial != null)
            meshRenderer.sharedMaterial = surfaceMaterial;
    }

    public void DepositSweep(
        IAccumulationZoneState zone,
        float deltaTime)
    {
        if (zone == null)
            return;

        bool changed = solver.DepositSweep(
            zone.PreviousPosition,
            zone.Position,
            zone.PreviousRadius,
            zone.Radius,
            deltaTime,
            accumulationSpeed);

        if (changed)
            meshView.UpdateGeometry();
    }

    public Vector2 ClampPosition(
        Vector2 position,
        float radius)
    {
        float halfWidth = surfaceSize.x * 0.5f;
        float halfDepth = surfaceSize.y * 0.5f;

        float borderX = Mathf.Min(radius, halfWidth);
        float borderZ = Mathf.Min(radius, halfDepth);

        position.x = Mathf.Clamp(
            position.x,
            -halfWidth + borderX,
            halfWidth - borderX);

        position.y = Mathf.Clamp(
            position.y,
            -halfDepth + borderZ,
            halfDepth - borderZ);

        return position;
    }

    [ContextMenu("Reset Surface")]
    public void ResetSurface()
    {
        if (data == null)
            return;

        data.Reset();
        meshView.UpdateGeometry();
    }

    private void OnDestroy()
    {
        if (meshView != null)
            meshView.Dispose();
    }
}