using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class CapsulePlayerController : MonoBehaviour
{
    [SerializeField] float moveSpeed = 6f;
    [SerializeField] float jumpHeight = 1.5f;
    [SerializeField] float gravity = -20f;
    [SerializeField] float groundedProbe = 0.2f;

    CharacterController controller;
    Vector3 velocity;
    Transform cam;

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        cam = Camera.main != null ? Camera.main.transform : null;
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;
        if (keyboard == null)
            return;

        bool grounded = IsGrounded();
        if (grounded && velocity.y < 0f)
            velocity.y = -2f;

        float x = 0f;
        float z = 0f;
        if (keyboard.aKey.isPressed) x -= 1f;
        if (keyboard.dKey.isPressed) x += 1f;
        if (keyboard.sKey.isPressed) z -= 1f;
        if (keyboard.wKey.isPressed) z += 1f;

        Vector3 move;
        if (cam != null)
        {
            Vector3 forward = cam.forward;
            Vector3 right = cam.right;
            forward.y = 0f;
            right.y = 0f;
            forward.Normalize();
            right.Normalize();
            move = forward * z + right * x;
        }
        else
        {
            move = new Vector3(x, 0f, z);
        }

        if (move.sqrMagnitude > 1f)
            move.Normalize();

        if (grounded && keyboard.spaceKey.wasPressedThisFrame)
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        velocity.y += gravity * Time.deltaTime;

        Vector3 motion = move * moveSpeed;
        motion.y = velocity.y;
        controller.Move(motion * Time.deltaTime);
    }

    bool IsGrounded()
    {
        if (controller.isGrounded)
            return true;

        Vector3 origin = transform.position + controller.center;
        float radius = Mathf.Max(0.05f, controller.radius * 0.9f);
        float distance = (controller.height * 0.5f) - radius + groundedProbe;
        return Physics.SphereCast(
            origin,
            radius,
            Vector3.down,
            out _,
            distance,
            Physics.DefaultRaycastLayers,
            QueryTriggerInteraction.Ignore);
    }

}
