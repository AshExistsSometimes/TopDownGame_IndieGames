using UnityEngine;

public class MouseWorldIndicator : MonoBehaviour
{
    public TopDownPlayerController player;

    private void Awake()
    {
        Cursor.visible = false;

        if (player != null)
        {
            if (!player.UseMouseInput)
            {
                Cursor.lockState = CursorLockMode.Locked;
                gameObject.SetActive(false);
            }
        }
    }

    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, 0);

        float dist;

        if (plane.Raycast(ray, out dist))
        {
            transform.position = ray.GetPoint(dist);
        }
    }
}