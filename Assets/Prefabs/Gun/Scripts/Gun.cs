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

    //

    float nextShootTime;
    bool isTriggerHold;
    bool isReloading;

    void Start() {
        currentMagazineCapacity = magazineCapacity;
        currentBurstCapacity = burstCapacity;
    }

    public void HoldTrigger() {
        Shoot();
        isTriggerHold = true;
    }

    public void ReleaseTrigger() {
        currentBurstCapacity = burstCapacity;
        isTriggerHold = false;
    }

    public void Reload() {
        if (!isReloading && currentMagazineCapacity != magazineCapacity) {
            StartCoroutine(ReloadAnimation());
            currentMagazineCapacity = magazineCapacity;
            currentBurstCapacity = burstCapacity;
        }
    }

    void Shoot() {
        if (Time.time >= nextShootTime && currentMagazineCapacity > 0 && !isReloading) {
            switch (fireMode) {
                case FireMode.AUTO:
                    // Do nothing (the time control the shoots)
                    break;
                case FireMode.BURST:
                    if (currentBurstCapacity == 0) {
                        return;
                    }
                    currentBurstCapacity--;
                    break;
                case FireMode.SINGLE:
                    if (isTriggerHold) {
                        return;
                    }
                    break;
            }
            currentMagazineCapacity--;
            nextShootTime = Time.time + 1 / fireRate;
            foreach (Transform br in barrel) {
                Projectile projectile = Instantiate(projectilePrefab, br.position, br.rotation) as Projectile;
                projectile.SetProperties(projectileVelocity, projectileDamage, projectileLifeTime);
                projectile.Launch();
            }
        }
    }

    IEnumerator ReloadAnimation() {
        isReloading = true;

        Vector3 initialEulerAngles = transform.localEulerAngles;
        float maxReloadAngle = 30;

        float incRate = 1 / reloadTime;
        float percent = 0;

        while (percent < 1) {
            percent += incRate * Time.deltaTime;
            float interpolation = 4 * (-Mathf.Pow(percent, 2) + percent);
            float currentReloadAngle = Mathf.Lerp(0, maxReloadAngle, interpolation);
            transform.localEulerAngles = initialEulerAngles + Vector3.left * currentReloadAngle;

            yield return null;
        }

        transform.localEulerAngles = initialEulerAngles;

        isReloading = false;
    }

    public enum FireMode {
        AUTO,
        BURST,
        SINGLE
    }
}
