using UnityEngine;

public class TopDownCameraFollow : MonoBehaviour
{
    public Transform Player;

    public Transform Target;// Set to plaeyr in awake, can be changed for cutscenes
    public Vector3 offset = new Vector3(0f, 10f, -6f);
    public float followSpeed = 5f;

    Vector3 velocity;


    // Smoothly follows the target with a slight drag effect.

    private void Awake()
    {
        Target = Player;
    }

    void LateUpdate()
    {
        if (!Target) return;

        Vector3 desiredPosition = Target.position + offset;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref velocity,
            1f / followSpeed
        );
    }
}
