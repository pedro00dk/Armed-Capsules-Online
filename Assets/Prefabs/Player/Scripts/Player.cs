using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Player : MonoBehaviour {

    public float health;

    // Internal properties

    float currentHealth;

    void Start() {
        currentHealth = health;
    }

    public void HitByProjectile(Projectile projectile) {
        health -= projectile.GetDamage();
    }
}
