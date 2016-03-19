using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(Camera))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

    [Header("Player movement properties")]
    public float movementSpeed;

    [Header("Player camera properties")]
    public Camera cam;
    public Vector2 cameraSensitivity;
    public float minimumVerticalAngle;
    public float maximumVerticalAngle;

    // Internal properties

    Rigidbody body;

    Vector3 movementInput;
    Vector3 movementSmoothInput;
    Vector3 movementVelocity;

    Vector2 cameraInput;

    void Start () {
        body = GetComponent<Rigidbody>();
	}
	
	void Update () {

        // Movement input
        movementInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")) * movementSpeed;
        movementSmoothInput = Vector3.SmoothDamp(movementSmoothInput, movementInput, ref movementVelocity, 0.2f);

        // Camera input
        cameraInput = new Vector2(
            Input.GetAxis("Mouse X") * cameraSensitivity.x,
            Mathf.Clamp(cameraInput.y + Input.GetAxis("Mouse Y") * cameraSensitivity.y, minimumVerticalAngle, maximumVerticalAngle)
        );
        transform.Rotate(new Vector3(0, cameraInput.x, 0));
        cam.transform.localEulerAngles = new Vector3(-cameraInput.y, 0, 0);
    }

    void FixedUpdate() {

        // Movement input
        body.MovePosition(body.position + transform.TransformVector(movementSmoothInput) * Time.fixedDeltaTime);
    }
}
