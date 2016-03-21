using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(TrailRenderer))]
public class Projectile : MonoBehaviour {

    // Internal properties

    float velocity;
    float damage;
    float lifeTime;

    Rigidbody body;
    TrailRenderer tRend;

    // Map tags
    readonly string playerTag = "Player";
    readonly string mapFloorTag = "Map Floor";
    readonly string mapObstacleTag = "Map Obstacle";

    void Awake() {
        body = GetComponent<Rigidbody>();
        body.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        // tRend = GetComponent<TrailRenderer>();
    }

    public void SetProperties(float velocity, float damage, float lifeTime) {
        this.velocity = velocity;
        this.damage = damage;
        this.lifeTime = lifeTime;
    }

    public void Launch() {
        body.AddForce(transform.forward * velocity, ForceMode.VelocityChange);
        StartCoroutine(DestroyProjectile());
        Destroy(gameObject, lifeTime);
    }

    IEnumerator DestroyProjectile() {
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

    public float GetDamage() {
        return damage;
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag(mapFloorTag)) {
            damage = 0;
        } else if (collision.gameObject.CompareTag(mapObstacleTag) && damage > 0) {
            damage /= 2;
        } else if (collision.gameObject.CompareTag(playerTag) && damage > 0) {
            Player player = collision.gameObject.GetComponent<Player>();
            player.HitByProjectile(this);
            Destroy(gameObject);
        }
    }
}
