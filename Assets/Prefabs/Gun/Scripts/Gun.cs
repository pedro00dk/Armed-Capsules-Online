using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

    [Header("Gun properties")]
    public FireMode fireMode;
    public float shootDamage;
    public float shootSpeed;
    public float fireRate;
    public int magazineCapacity;
    public float reloadTime;
    [Space]
    public int burstCapacity;
    [Space]
    public Transform[] barrel;

    // Internal properties

    int currentMagazineCapacity;
    int currentBurstCapacity;

    void Start () {
        currentMagazineCapacity = magazineCapacity;
        currentBurstCapacity = burstCapacity;
	}

    public void HoldTrigger() {

    }

    public void ReleaseTrigger() {

    }

    public void Reload() {

    }

    void Shoot() {

    }

    public enum FireMode {
        AUTO,
        BURST,
        SINGLE
    }
}
