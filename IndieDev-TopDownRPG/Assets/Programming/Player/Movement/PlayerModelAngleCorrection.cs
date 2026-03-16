using UnityEngine;

public class PlayerModelAngleCorrection : MonoBehaviour
{
    public Transform player;
    public TopDownPlayerController controller;

    public float tiltAngle = 25f;
    public float modelYRotation = -180f;

    void LateUpdate()
    {
        if (!player || !controller) return;

        if (!controller.InItemGetState)
        {
            float y = player.eulerAngles.y;

            float rad = y * Mathf.Deg2Rad;

            float x = -Mathf.Cos(rad) * tiltAngle;
            float z = -Mathf.Sin(rad) * tiltAngle;

            transform.localRotation = Quaternion.Euler(x, modelYRotation, z);
        }
        else
        {
            transform.localRotation = Quaternion.Euler(0, modelYRotation, 0);
        }
        
    }
}