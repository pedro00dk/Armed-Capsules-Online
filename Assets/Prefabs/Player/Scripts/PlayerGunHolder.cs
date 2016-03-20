using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerController))]
public class PlayerGunHolder : MonoBehaviour {

    public Gun startGunPrefab;
    public Transform gunHolder;

    // Internal properties

    Gun equippedGun;
    Camera playerControllerCamera;

    void Start() {
        playerControllerCamera = GetComponent<PlayerController>().cam;
        equippedGun = Instantiate(startGunPrefab, gunHolder.position, Quaternion.identity) as Gun;
        equippedGun.transform.parent = gunHolder;
    }

    void Update() {
        if (Input.GetMouseButtonDown(0)) {
            equippedGun.HoldTrigger();
        } else if (Input.GetMouseButtonUp(0)) {
            equippedGun.ReleaseTrigger();
        }

        if (Input.GetKeyDown(KeyCode.R)) {
            equippedGun.Reload();
        }
    }
}
