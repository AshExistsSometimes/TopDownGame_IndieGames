using System.Collections;
using UnityEngine;

public class TopDownPlayerController : MonoBehaviour
{
    public Transform cameraTransform;

    public float moveSpeed = 5f;
    public float rotationSpeed = 12f;

    public bool MovementON = true;
    public bool UseMouseInput = true;

    [Space]
    [Header ("Get Item")]
    public float itemRotateSpeed = 2f;
    public float itemPauseTime = 0.35f;
    public Vector3 ItemGetLookAngle = new Vector3(-45f, 180f, 0f);
    public GameObject TempDialogue;
    public GameObject TempFakeItem;

    CharacterController controller;

    Vector3 savedPosition;
    Quaternion savedRotation;

    Coroutine itemRoutine;

    Vector3 lastMousePos;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        lastMousePos = Input.mousePosition;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            if (itemRoutine == null)
                itemRoutine = StartCoroutine(ItemGot());
        }

        if (!MovementON) return;

        Vector3 moveDir = GetMoveDirection();

        HandleMovement(moveDir);
        HandleRotation(moveDir);
    }

    // Calculates movement direction relative to camera
    Vector3 GetMoveDirection()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 dir = forward * v + right * h;

        if (dir.magnitude > 1f)
            dir.Normalize();

        return dir;
    }

    // Moves the player
    void HandleMovement(Vector3 moveDir)
    {
        controller.Move(moveDir * moveSpeed * Time.deltaTime);
    }

    // Handles rotation logic
    void HandleRotation(Vector3 moveDir)
    {
        if (moveDir.magnitude > 0.1f)
        {
            Quaternion target = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, rotationSpeed * Time.deltaTime);
            return;
        }

        if (!UseMouseInput)
        {
            float y = transform.eulerAngles.y;
            float snapped = Mathf.Round(y / 45f) * 45f;
            transform.rotation = Quaternion.Euler(0, snapped, 0);
            return;
        }

        if (Input.mousePosition == lastMousePos)
            return;

        lastMousePos = Input.mousePosition;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Plane plane = new Plane(Vector3.up, transform.position);

        float dist;

        if (plane.Raycast(ray, out dist))
        {
            Vector3 point = ray.GetPoint(dist);
            Vector3 dir = point - transform.position;
            dir.y = 0;

            if (dir.magnitude > 0.1f)
            {
                Quaternion target = Quaternion.LookRotation(dir);
                transform.rotation = Quaternion.Slerp(transform.rotation, target, rotationSpeed * Time.deltaTime);
            }
        }
    }

    // Item pickup sequence
    public IEnumerator ItemGot()
    {
        MovementON = false;

        savedPosition = transform.position;
        savedRotation = transform.rotation;

        Quaternion targetRot = Quaternion.Euler(ItemGetLookAngle);

        float t = 0;

        while (t < 1f)
        {
            t += Time.deltaTime * itemRotateSpeed;
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, t);
            yield return null;
        }

        TempDialogue.SetActive(true); 
        TempFakeItem.SetActive(true); 
        yield return new WaitForSeconds(itemPauseTime);

        while (true)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Submit"))
            {
                TempFakeItem.SetActive(false);
                TempDialogue.SetActive(false);

                break;
            }

            yield return null;
        }

        transform.position = savedPosition;
        transform.rotation = savedRotation;

        MovementON = true;
        itemRoutine = null;
    }
}