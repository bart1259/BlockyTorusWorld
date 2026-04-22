using UnityEngine;
using UnityEngine.InputSystem;

public class FreeCam : MonoBehaviour {

    public float MoveSpeed = 10f;
    public float FastMoveMultiplier = 3f;
    public float LookSensitivity = 2f;
    public float MinPitch = -89f;
    public float MaxPitch = 89f;

    private const float MouseLookScale = 0.06f;

    private float yaw;
    private float pitch;

    private void Start() {
        Vector3 euler = transform.eulerAngles;
        yaw = euler.y;
        pitch = euler.x;
        if (pitch > 180f) pitch -= 360f;
    }

    private void Update() {
        var mouse = Mouse.current;
        var keyboard = Keyboard.current;
        if (mouse == null || keyboard == null)
            return;

        bool look = mouse.rightButton.isPressed;
        if (look) {
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            Vector2 delta = mouse.delta.ReadValue();
            yaw += delta.x * LookSensitivity * MouseLookScale;
            pitch -= delta.y * LookSensitivity * MouseLookScale;
            pitch = Mathf.Clamp(pitch, MinPitch, MaxPitch);
            transform.rotation = Quaternion.Euler(pitch, yaw, 0f);
        }
        else {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        Vector3 move = Vector3.zero;
        if (keyboard.wKey.isPressed) move += transform.forward;
        if (keyboard.sKey.isPressed) move -= transform.forward;
        if (keyboard.dKey.isPressed) move += transform.right;
        if (keyboard.aKey.isPressed) move -= transform.right;
        if (keyboard.eKey.isPressed) move += transform.up;
        if (keyboard.qKey.isPressed) move -= transform.up;

        float speed = MoveSpeed * (keyboard.leftShiftKey.isPressed ? FastMoveMultiplier : 1f);
        if (move.sqrMagnitude > 0f)
            transform.position += move.normalized * speed * Time.deltaTime;
    }

}
