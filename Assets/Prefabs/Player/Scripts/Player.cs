using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour {

    [Header("Player properties")]
    public float health;

    [Header("Effects")]
    public ParticleSystem deathEffectPrefab;

    // Internal properties

    float currentHealth;
    bool isDead;

    void Start() {
        currentHealth = health;
    }

    public void Hit(float damage, Vector3 position) {
        health -= damage;
        if (health <= 0) {
            Die(position);
        }
    }

    void Die(Vector3 lastHitPosition) {
        if(!isDead) {
            Vector3 direction = transform.position - lastHitPosition;
            direction.Set(direction.x, 0, direction.z);
            ParticleSystem deathEffect = Instantiate(deathEffectPrefab, transform.position, Quaternion.FromToRotation(Vector3.forward, direction)) as ParticleSystem;
            gameObject.SetActive(false);
            isDead = true;
        }
    }
}
