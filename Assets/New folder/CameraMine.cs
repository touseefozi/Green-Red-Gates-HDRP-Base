using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class CameraMine : MonoBehaviour
{
    public float zDelta, yDelta, fovDelta, xDampingDelta; // Multipliers for each property
    public float minFOV, maxFOV; // Min and max values for FOV
    public float minYOffset, maxYOffset; // Min and max values for Y offset
    public float minZOffset, maxZOffset; // Min and max values for Z offset
    public float minXDamping, maxXDamping; // Min and max values for X Damping

    public float transitionTime = 0.5f; // Time for the transition (lerp duration)
    public CinemachineVirtualCamera cam; // Reference to the Cinemachine camera

    private void OnEnable()
    {
        GateCollider.SizeChanged += OnSizeChanged;
    }

    private void OnDisable()
    {
        GateCollider.SizeChanged -= OnSizeChanged;
    }

    void OnSizeChanged(float delta)
    {
        // Calculate and clamp the target FOV
        float targetFOV = Mathf.Clamp(cam.m_Lens.FieldOfView * (1 + (fovDelta * delta)), minFOV, maxFOV);

        // Use DOTween to animate the FOV
        DOTween.To(() => cam.m_Lens.FieldOfView, x => cam.m_Lens.FieldOfView = x, targetFOV, transitionTime);

        // Adjust the follow offset and X Damping if a Follow target is set
        if (cam.Follow != null)
        {
            CinemachineTransposer transposer = cam.GetCinemachineComponent<CinemachineTransposer>();
            Vector3 currentOffset = transposer.m_FollowOffset;

            // Calculate new target offsets and clamp them
            float targetYOffset = Mathf.Clamp(currentOffset.y * (1 + (yDelta * delta)), minYOffset, maxYOffset);
            float targetZOffset = Mathf.Clamp(currentOffset.z * (1 + (zDelta * delta)), minZOffset, maxZOffset);
            Vector3 targetOffset = new Vector3(currentOffset.x, targetYOffset, targetZOffset);

            // Animate the offset change with DOTween
            DOTween.To(() => transposer.m_FollowOffset, x => transposer.m_FollowOffset = x, targetOffset, transitionTime);

            // Calculate and clamp the target X Damping
            float targetXDamping = Mathf.Clamp(transposer.m_XDamping * (1 + (xDampingDelta * delta)), minXDamping, maxXDamping);

            // Animate the X Damping change with DOTween
            DOTween.To(() => transposer.m_XDamping, x => transposer.m_XDamping = x, targetXDamping, transitionTime);
        }
    }
}
