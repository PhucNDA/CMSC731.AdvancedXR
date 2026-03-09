using UnityEngine;

public class CameraMove : MonoBehaviour
{
    public float moveSpeed = 50f;
    public float rotateSpeed = 120f;
    public float mouseSensitivity = 2f;

    private float pitch = 0f;

    void Update()
    {
        // Movement: W/S = forward/backward, A/D = left/right
        float moveForward = 0f;
        float moveRight = 0f;

        if (Input.GetKey(KeyCode.W)) moveForward += 1f;
        if (Input.GetKey(KeyCode.S)) moveForward -= 1f;
        if (Input.GetKey(KeyCode.D)) moveRight += 1f;
        if (Input.GetKey(KeyCode.A)) moveRight -= 1f;

        Vector3 move = transform.forward * moveForward + transform.right * moveRight;
        transform.position += move.normalized * moveSpeed * Time.deltaTime;

        // Rotation with arrow keys
        float yaw = 0f;
        if (Input.GetKey(KeyCode.LeftArrow)) yaw -= 1f;
        if (Input.GetKey(KeyCode.RightArrow)) yaw += 1f;

        transform.Rotate(0f, yaw * rotateSpeed * Time.deltaTime, 0f);

        // Optional up/down look with arrow keys
        if (Input.GetKey(KeyCode.UpArrow)) pitch -= rotateSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.DownArrow)) pitch += rotateSpeed * Time.deltaTime;

        pitch = Mathf.Clamp(pitch, -80f, 80f);
        Vector3 angles = transform.eulerAngles;
        transform.eulerAngles = new Vector3(pitch, angles.y, 0f);
    }
}
