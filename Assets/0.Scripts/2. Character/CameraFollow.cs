using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    private Transform target;

    private void Awake()
    {
        PlayerController player = FindAnyObjectByType<PlayerController>();

        if (player != null)
            target = player.transform;
    }

    private void LateUpdate()
    {
        if (target == null) return;

        transform.position = new Vector3(
            target.position.x,
            target.position.y,
            transform.position.z
        );
    }
}