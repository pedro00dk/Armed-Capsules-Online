using UnityEngine;
using System.Collections;

public class Gun : MonoBehaviour {

    [Header("Gun properties")]
    public FireMode fireMode;
    public float projectileVelocity;
    public float projectileDamage;
    public float projectileLifeTime;
    public float fireRate;
    public int magazineCapacity;
    public float reloadTime;
    [Space]
    public int burstCapacity;
    [Space]
    public Projectile projectilePrefab;
    public Transform[] barrel;

    // Internal properties

    int currentMagazineCapacity;
    int currentBurstCapacity;

    void Start () {
        currentMagazineCapacity = magazineCapacity;
        currentBurstCapacity = burstCapacity;
	}

    public void HoldTrigger() {
        Shoot();
    }

    public void ReleaseTrigger() {

    }

    public void Reload() {

    }

    void Shoot() {
        Projectile projectile = Instantiate(projectilePrefab, barrel[0].position, barrel[0].rotation * projectilePrefab.transform.rotation) as Projectile;
        projectile.SetProperties(projectileVelocity, projectileDamage, projectileLifeTime);
        projectile.Launch();
    }

    public enum FireMode {
        AUTO,
        BURST,
        SINGLE
    }
}
