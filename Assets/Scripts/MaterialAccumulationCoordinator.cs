using UnityEngine;

[DefaultExecutionOrder(100)]
public class MaterialAccumulationCoordinator : MonoBehaviour
{
    [SerializeField] private MaterialAccumulationSurface surface;

    [SerializeField] private MonoBehaviour zoneSource;
    [SerializeField] private MonoBehaviour inputSource;

    private IAccumulationZoneState zone;
    private IAccumulationInput input;

    private void Awake()
    {
        zone = zoneSource as IAccumulationZoneState;
        input = inputSource as IAccumulationInput;

        if (surface == null || zone == null || input == null)
        {
            Debug.LogError(
                "Coordinator dependencies are not configured.",
                this);

            enabled = false;
        }
    }

    private void LateUpdate()
    {
        if (input.ResetPressed)
        {
            surface.ResetSurface();
        }

        if (input.AccumulationHeld)
        {
            surface.DepositSweep(
                zone,
                Time.deltaTime);
        }
    }
}