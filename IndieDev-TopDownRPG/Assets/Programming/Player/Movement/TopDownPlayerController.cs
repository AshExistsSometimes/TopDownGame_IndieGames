using System.Collections;
using UnityEngine;

public class TopDownPlayerController : MonoBehaviour
{
    public Transform cameraTransform;
    public Camera cam;

    [Header("Movement")]
    public float moveSpeed = 5f;
    public float rotationSpeed = 12f;

    public bool MovementON = true;
    public bool UseMouseInput = true;

    [Space]
    [Header ("Get Item")]
    public float itemRotateSpeed = 2f;
    public float itemPauseTime = 0.35f;

    public float returnRotateSpeed = 3f;
    public float fovLerpSpeed = 4f;

    public float ItemShowTimer = 0.2f;   // delay before showing item
    public float ItemGrowTime = 0.5f;    // how long it takes to grow to full scale
    public float DialogueDelay = 0.3f;   // delay before showing dialogue

    public Vector3 ItemGetLookAngle = new Vector3(-45f, 180f, 0f);

    public GameObject TempDialogue;
    public GameObject TempFakeItem;

    public float ItemGotFOV = 50f;

    private float defaultFOV = 60f;

    CharacterController controller;

    Vector3 savedPosition;
    Quaternion savedRotation;

    Coroutine itemRoutine;

    Vector3 lastMousePos;

    // True while the player is in the item pickup animation state
    public bool InItemGetState = false;

    void Start()
    {
        controller = GetComponent<CharacterController>();
        lastMousePos = Input.mousePosition;

        defaultFOV = cam.fieldOfView;
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
        InItemGetState = true;

        // Save current player position and rotation
        savedPosition = transform.position;
        savedRotation = transform.rotation;

        // Sink player down
        transform.position += Vector3.down * 0.72f;

        // Target rotation for the item pose
        Quaternion targetRot = Quaternion.Euler(ItemGetLookAngle);

        // Step 1: Rotate player into item pose & lerp camera FOV
        float t = 0f;
        while (t < 1f)
        {
            t += Time.deltaTime * itemRotateSpeed;
            transform.rotation = Quaternion.Slerp(savedRotation, targetRot, t);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, ItemGotFOV, Time.deltaTime * fovLerpSpeed);
            yield return null;
        }

        // Step 2: Wait before showing the item
        TempFakeItem.SetActive(false);
        TempDialogue.SetActive(false);
        yield return new WaitForSeconds(ItemShowTimer);

        // Step 3: Show the item and scale it up smoothly
        TempFakeItem.SetActive(true);
        Vector3 targetScale = TempFakeItem.transform.localScale;
        TempFakeItem.transform.localScale = Vector3.one * 0.001f; // start tiny

        float growT = 0f;
        while (growT < 1f)
        {
            growT += Time.deltaTime / ItemGrowTime;
            TempFakeItem.transform.localScale = Vector3.Lerp(Vector3.one * 0.001f, targetScale, growT);
            yield return null;
        }

        // Step 4: Wait before showing dialogue
        yield return new WaitForSeconds(DialogueDelay);
        TempDialogue.SetActive(true);

        // Step 5: Wait for player input to close item/dialogue
        while (true)
        {
            if (Input.GetMouseButtonDown(0) || Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("Submit"))
            {
                TempFakeItem.SetActive(false);
                TempDialogue.SetActive(false);
                InItemGetState = false;
                break;
            }

            yield return null;
        }

        // Step 6: Return player to original position & rotation while lerping FOV back
        transform.position = savedPosition;

        float returnT = 0f;
        while (returnT < 1f)
        {
            returnT += Time.deltaTime * returnRotateSpeed;
            transform.rotation = Quaternion.Slerp(transform.rotation, savedRotation, returnT);
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, defaultFOV, Time.deltaTime * fovLerpSpeed);
            yield return null;
        }

        MovementON = true;
        itemRoutine = null;
    }
}