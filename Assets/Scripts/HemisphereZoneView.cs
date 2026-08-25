using UnityEngine;

[DefaultExecutionOrder(50)]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class HemisphereZoneView : MonoBehaviour
{
    [Header("Dependency")]
    [SerializeField] private MonoBehaviour zoneSource;

    [Header("Appearance")]
    [SerializeField] private Material zoneMaterial;

    [SerializeField] private int segments = 32;
    [SerializeField] private int rings = 12;

    private IAccumulationZoneState zone;
    private Mesh zoneMesh;

    private void Awake()
    {
        zone = zoneSource as IAccumulationZoneState;

        if (zone == null)
        {
            Debug.LogError(
                "Zone source is not configured.",
                this);

            enabled = false;
            return;
        }

        zoneMesh = HemisphereMeshFactory.Create(
            segments,
            rings);

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        meshFilter.sharedMesh = zoneMesh;

        if (zoneMaterial != null)
            meshRenderer.sharedMaterial = zoneMaterial;

        UpdateView();
    }

    private void LateUpdate()
    {
        UpdateView();
    }

    private void UpdateView()
    {
        transform.localPosition = new Vector3(
            zone.Position.x,
            0.02f,
            zone.Position.y);

        transform.localScale =
            Vector3.one * zone.Radius;
    }

    private void OnDestroy()
    {
        if (zoneMesh == null)
            return;

        if (Application.isPlaying)
            Destroy(zoneMesh);
        else
            DestroyImmediate(zoneMesh);
    }
}