using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class Gun : MonoBehaviour {

    [Header("Gun properties")]
    public FireMode fireMode;
    public float fireRate;
    public int magazineCapacity;
    public float reloadTime;
    [Space]
    public int burstCapacity;
    [Space]
    public Transform[] barrel;

    [Header("Projectile properties")]
    public Projectile projectilePrefab;
    public float projectileVelocity;
    public float projectileDamage;
    public float projectileLifeTime;

    [Header("Recoil properties")]
    public Vector2 kickRecoilVariation;
    public float kickRecoilResolveTime;
    public Vector2 verticalAngleRecoilVariation;
    public float verticalAngleRecoilResolveTime;
    public float maxVerticalAngle;
    public Vector2 horizontalAngleRecoilVariation;
    public float horizontalAngleRecoilResolveTime;
    public float minMaxHorizontalAngle;

    [Header("Effects")]
    public Sprite[] shootFlashes;
    public SpriteRenderer[] shootFlashRenderers;
    [Space]
    public Light shootLight;
    [Space]
    public float shootEffectsTime;
    [Space]
    public AudioClip shootSound;
    public AudioClip reloadSound;

    // Internal properties

    AudioSource gunSoundPlayer;

    int currentMagazineCapacity;
    int currentBurstCapacity;
    float nextShootTime;
    bool isTriggerHold;
    bool isReloading;
    float actualVerticalAngle;
    float actualHorizontalAngle;
    Vector3 currentKickRecoilVelocity;
    float currentVerticalAngleRecoilVelocity;
    float currentHorizontalAngleRecoilVelocity;


    void Start() {
        currentMagazineCapacity = magazineCapacity;
        currentBurstCapacity = burstCapacity;
        gunSoundPlayer = GetComponent<AudioSource>();
        StartCoroutine(RecoilResolver());
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
            // Recoil
            transform.localPosition += Vector3.back * Random.Range(kickRecoilVariation.x, kickRecoilVariation.y);
            actualVerticalAngle = Mathf.Clamp(actualVerticalAngle + Random.Range(verticalAngleRecoilVariation.x, verticalAngleRecoilVariation.y), 0, maxVerticalAngle);
            actualHorizontalAngle = Mathf.Clamp(
                actualHorizontalAngle
                + ((Random.Range(-10, 10) > 0)
                ? Random.Range(horizontalAngleRecoilVariation.x, horizontalAngleRecoilVariation.y)
                : -Random.Range(horizontalAngleRecoilVariation.x, horizontalAngleRecoilVariation.y)),
                -minMaxHorizontalAngle, minMaxHorizontalAngle
            );

            // Effects
            StartCoroutine(ShootEffects());
        }
    }

    IEnumerator ReloadAnimation() {
        isReloading = true;

        // Reload sound effect
        gunSoundPlayer.clip = reloadSound;
        gunSoundPlayer.Play();

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

    IEnumerator RecoilResolver() {
        while (true) {
            yield return new WaitForEndOfFrame();
            transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref currentKickRecoilVelocity, kickRecoilResolveTime);
            transform.localEulerAngles = new Vector3(
                -(actualVerticalAngle = Mathf.SmoothDamp(actualVerticalAngle, 0, ref currentVerticalAngleRecoilVelocity, verticalAngleRecoilResolveTime)),
                actualHorizontalAngle = Mathf.SmoothDamp(actualHorizontalAngle, 0, ref currentHorizontalAngleRecoilVelocity, horizontalAngleRecoilResolveTime),
                0
            );
        }
    }

    IEnumerator ShootEffects() {

        // Sound
        gunSoundPlayer.clip = shootSound;
        gunSoundPlayer.Play();

        //Light
        shootLight.gameObject.SetActive(true);

        // Flashes
        int flashIndex = Random.Range(0, shootFlashes.Length);
        foreach(SpriteRenderer renderer in shootFlashRenderers) {
            renderer.sprite = shootFlashes[flashIndex];
            renderer.gameObject.SetActive(true);
        }

        // Disable
        yield return new WaitForSeconds(shootEffectsTime);

        shootLight.gameObject.SetActive(false);
        foreach (SpriteRenderer renderer in shootFlashRenderers) {
            renderer.gameObject.SetActive(false);
        }
    }

    public enum FireMode {
        AUTO,
        BURST,
        SINGLE
    }
}
