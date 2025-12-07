using UnityEngine;

public class CameraFollow2D : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Offset")]
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Smoothing")]
    [SerializeField] private float smoothTime = 0.15f;

    private Vector3 _velocity;

    private void Start()
    {
        if (target == null) return;

        // Immediate snap on start; prevents the smoothing time update
        transform.position = target.position + offset;
        _velocity = Vector3.zero;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        Vector3 finalPos = target.position + offset;
        transform.position = Vector3.SmoothDamp(
            transform.position,
            finalPos,
            ref _velocity,
            smoothTime);
    }
}
