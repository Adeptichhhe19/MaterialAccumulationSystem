using UnityEngine;
using UnityEngine.Rendering;

[DefaultExecutionOrder(50)]
[DisallowMultipleComponent]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HemisphereZoneView : MonoBehaviour
{
    [Header("Dependency")]
    [SerializeField] private MonoBehaviour zoneSource;

    [Header("View")]
    [SerializeField] private Material zoneMaterial;
    [SerializeField, Range(12, 96)] private int segments = 48;
    [SerializeField, Range(3, 32)] private int rings = 12;
    [SerializeField, Min(0f)] private float surfaceOffset = 0.025f;

    private IAccumulationZoneState zone;
    private Mesh mesh;
    private Material ownedMaterial;

    private void Awake()
    {
        zone = zoneSource as IAccumulationZoneState;
        if (zone == null)
        {
            Debug.LogError("Hemisphere view requires an IAccumulationZoneState source.", this);
            enabled = false;
            return;
        }

        mesh = HemisphereMeshFactory.Create(segments, rings);
        GetComponent<MeshFilter>().sharedMesh = mesh;
        var meshRenderer = GetComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = GetMaterial();
        meshRenderer.shadowCastingMode = ShadowCastingMode.Off;
        meshRenderer.receiveShadows = false;
        ApplyState();
    }

    private void LateUpdate()
    {
        ApplyState();
    }

    private void OnDestroy()
    {
        DestroyOwned(mesh);
        DestroyOwned(ownedMaterial);
    }

    private void ApplyState()
    {
        if (zone == null)
        {
            return;
        }

        Vector2 position = zone.Position;
        transform.localPosition = new Vector3(position.x, surfaceOffset, position.y);
        transform.localScale = Vector3.one * zone.Radius;
    }

    private Material GetMaterial()
    {
        if (zoneMaterial != null)
        {
            return zoneMaterial;
        }

        Shader shader = Shader.Find("Material Accumulation/Zone Overlay");
        ownedMaterial = new Material(shader != null
            ? shader
            : Shader.Find("Universal Render Pipeline/Unlit"));
        ownedMaterial.name = "Runtime Zone Material";
        return ownedMaterial;
    }

    private static void DestroyOwned(Object target)
    {
        if (target == null)
        {
            return;
        }

        if (Application.isPlaying)
        {
            Destroy(target);
        }
        else
        {
            DestroyImmediate(target);
        }
    }
}
