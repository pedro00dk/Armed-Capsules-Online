using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
public class Shell : MonoBehaviour {

    // Internal properties

    float velocity;
    float lifeTime;

    Rigidbody body;

    void Awake() {
        body = GetComponent<Rigidbody>();
    }

    public void SetProperties(float velocity, float lifeTime) {
        this.velocity = velocity;
        this.lifeTime = lifeTime;
    }

    public void Eject() {
        body.AddForce(transform.right * velocity, ForceMode.VelocityChange);
        StartCoroutine(DestroyShell());
        Destroy(gameObject, lifeTime);
    }

    IEnumerator DestroyShell() {
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
