using UnityEngine;
using UnityEngine.InputSystem;

/// Interaction with world (break / place blocks)
class PlayerInteract : MonoBehaviour {
    public float interactDistance = 6f;

    public LayerMask interactLayer;

    private WorldManager worldManager;
    private Camera playerCamera;

    void Start() {
        worldManager = FindAnyObjectByType<WorldManager>();
        playerCamera = GetComponentInChildren<Camera>();
    }

    void Update() {
        if (Mouse.current == null) return;

        if (Mouse.current.leftButton.wasPressedThisFrame) {
            Ray ray = playerCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer)) {
                Vector3 blockPosition = hit.point - hit.normal * 0.1f;
                worldManager.SetBlock(blockPosition, 0); // Air
            }
        }
        if (Mouse.current.rightButton.wasPressedThisFrame) {
            Ray ray = playerCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (Physics.Raycast(ray, out RaycastHit hit, interactDistance, interactLayer)) {
                Vector3 blockPosition = hit.point + hit.normal * 0.1f;
                worldManager.SetBlock(blockPosition, 5); // Wood
            }
        }
    }
}