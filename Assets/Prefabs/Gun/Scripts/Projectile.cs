using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(TrailRenderer))]
public class Projectile : MonoBehaviour {

    public Color trailColor;
    public float lifeTime;

    // Internal properties

    float projectileVelocity;
    float projectileDamage;

    Rigidbody body;
    TrailRenderer tRend;

    void Awake() {
        body = GetComponent<Rigidbody>();
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        tRend = GetComponent<TrailRenderer>();
        tRend.material.SetColor("_TintColor", trailColor);
    }

    public void SetProperties(float velocity, float damage) {
        projectileVelocity = velocity;
        projectileDamage = damage;
    }

    public void Launch() {
        body.AddForce(transform.forward * projectileVelocity, ForceMode.VelocityChange);
        StartCoroutine(DestroyProjectile(lifeTime));
        Destroy(gameObject, lifeTime);
    }

    IEnumerator DestroyProjectile(float lifeTime) {
        yield return new WaitForSeconds(lifeTime * 2 / 3);

        float incRate = 1 / (lifeTime / 3);
        float percent = 0;
        Vector3 projectileScale = transform.localScale;

        while (percent <= 1) {
            percent += Time.deltaTime * incRate;
            transform.localScale = projectileScale * (1 - percent);
            yield return null;
        }
    }
}
