using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour {

    [Header("Player movement properties")]
    public float moveSpeed;

    // Internal properties

    Rigidbody body;
    Vector3 movementInput;

    void Start () {
        body = GetComponent<Rigidbody>();
        movementInput = new Vector3();
	}
	
	void Update () {

        // Movement input
        movementInput.Set(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));
        movementInput *= moveSpeed;
	}

    void FixedUpdate() {

        // Movement input
        body.MovePosition(body.position + movementInput * Time.fixedDeltaTime);
    }
}
