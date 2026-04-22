using UnityEngine;
using UnityEngine.InputSystem;

// Typical player controller - WASD movement, mouse look.
// Gravity and surface alignment are computed relative to the torus surface normal.
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

    public float MoveSpeed = 10f;
    public float FastMoveMultiplier = 3f;
    public float LookSensitivity = 2f;
    public float MinPitch = -89f;
    public float MaxPitch = 89f;

    public float TorusMajorRadius = 1024f;
    public float GravityStrength = 20f;
    public float JumpForce = 8f;
    public float GroundCheckDistance = 1.5f;

    private const float MouseLookScale = 0.06f;

    private float pitch;
    private float inputH, inputV, inputSpeed;
    private bool jumpRequested;
    private bool isGrounded;

    private Rigidbody rb;
    private Camera playerCamera;
    private WorldManager worldManager;

    // Contact normals gathered this physics step, used to depenetrate moveDir from walls.
    private readonly System.Collections.Generic.List<Vector3> contactNormals
        = new System.Collections.Generic.List<Vector3>();

    void Start() {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.freezeRotation = true;

        playerCamera = GetComponentInChildren<Camera>();
        if (playerCamera == null) {
            GameObject camObj = new GameObject("PlayerCamera");
            camObj.transform.SetParent(transform);
            camObj.transform.localPosition = new Vector3(0f, 1.7f, 0f);
            camObj.transform.localRotation = Quaternion.identity;
            playerCamera = camObj.AddComponent<Camera>();
            playerCamera.tag = "MainCamera";
        }

        worldManager = FindAnyObjectByType<WorldManager>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        transform.rotation = Quaternion.FromToRotation(Vector3.up, GetTorusNormal(transform.position));
    }

    void Update() {
        HandleMouseLook();
        AlignToTorusSurface();

        var kb = Keyboard.current;
        float h = 0f, v = 0f;
        if (kb.dKey.isPressed || kb.rightArrowKey.isPressed) h += 1f;
        if (kb.aKey.isPressed || kb.leftArrowKey.isPressed)  h -= 1f;
        if (kb.wKey.isPressed || kb.upArrowKey.isPressed)    v += 1f;
        if (kb.sKey.isPressed || kb.downArrowKey.isPressed)  v -= 1f;
        inputH = h;
        inputV = v;
        inputSpeed = MoveSpeed * (kb.leftShiftKey.isPressed ? FastMoveMultiplier : 1f);

        if (kb.spaceKey.wasPressedThisFrame && isGrounded)
            jumpRequested = true;

        if (kb.escapeKey.wasPressedThisFrame)
            Cursor.lockState = CursorLockMode.None;
        if (Mouse.current.leftButton.wasPressedThisFrame && Cursor.lockState == CursorLockMode.None) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        if (worldManager != null)
            worldManager.UpdatePlayerPosition(transform.position);
    }

    void FixedUpdate() {
        ApplyTorusGravity();
        HandleMovementPhysics();
    }

    // Returns the outward surface normal of the torus at the given world position.
    // The torus spine lies in the XY plane, so we project position onto XY to find the
    // nearest spine point, then compute the direction from spine to position.
    Vector3 GetTorusNormal(Vector3 position) {
        Vector3 spinePoint = new Vector3(position.x, position.y, 0f).normalized * TorusMajorRadius;
        return (position - spinePoint).normalized;
    }

    // Rotates the player so transform.up matches the torus surface normal at the current position.
    // Preserves the facing direction as much as possible.
    void AlignToTorusSurface() {
        Vector3 targetUp = GetTorusNormal(transform.position);
        Quaternion correction = Quaternion.FromToRotation(transform.up, targetUp);
        transform.rotation = correction * transform.rotation;
    }

    void ApplyTorusGravity() {
        Vector3 surfaceNormal = GetTorusNormal(rb.position);
        rb.AddForce(-surfaceNormal * GravityStrength, ForceMode.Acceleration);
    }

    void HandleMouseLook() {
        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        float mouseX = mouseDelta.x * LookSensitivity * MouseLookScale;
        float mouseY = mouseDelta.y * LookSensitivity * MouseLookScale;

        pitch -= mouseY;
        pitch = Mathf.Clamp(pitch, MinPitch, MaxPitch);

        // Yaw: rotate the player body around the outward torus normal so the
        // facing direction always stays tangent to the surface.
        Vector3 surfaceNormal = GetTorusNormal(transform.position);
        transform.Rotate(surfaceNormal, mouseX, Space.World);

        playerCamera.transform.localRotation = Quaternion.Euler(pitch, 0f, 0f);
    }

    void OnCollisionStay(Collision collision) {
        foreach (ContactPoint contact in collision.contacts)
            contactNormals.Add(contact.normal);
    }

    void HandleMovementPhysics() {
        Vector3 surfaceNormal = GetTorusNormal(rb.position);
        isGrounded = Physics.Raycast(transform.position, -surfaceNormal, GroundCheckDistance);

        // Project camera directions onto the surface tangent plane so horizontal
        // movement never pushes the player into or away from the torus.
        Vector3 camForward = Vector3.ProjectOnPlane(playerCamera.transform.forward, surfaceNormal).normalized;
        Vector3 camRight   = Vector3.ProjectOnPlane(playerCamera.transform.right,   surfaceNormal).normalized;

        Vector3 moveDir = camForward * inputV + camRight * inputH;
        if (moveDir.sqrMagnitude > 1f) moveDir.Normalize();
        moveDir *= inputSpeed;

        // For each surface the player is touching, cancel the component of moveDir
        // that points into it. This stops the player from being pressed into walls.
        foreach (Vector3 cn in contactNormals) {
            float penetration = Vector3.Dot(moveDir, -cn);
            if (penetration > 0f)
                moveDir += cn * penetration;
        }
        contactNormals.Clear();

        // Keep the velocity component along the surface normal intact so gravity
        // and jumping work naturally, but replace the tangential component each
        // frame for responsive movement with no momentum sliding.
        Vector3 normalVelocity = Vector3.Project(rb.linearVelocity, surfaceNormal);

        if (jumpRequested) {
            jumpRequested = false;
            normalVelocity = surfaceNormal * JumpForce;
        }

        rb.linearVelocity = moveDir + normalVelocity;
    }
}
