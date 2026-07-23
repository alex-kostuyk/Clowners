
using UnityEngine;

public class ActionExplosionPush : MonoBehaviour
{
    public float explosionForce = 10f;
    void OnTriggerStay(Collider other)
    {
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb != null)
        {
            if(rb.isKinematic)
                rb.isKinematic = false;

            Vector3 explosionDir = other.transform.position - transform.position;

            rb.AddForce(explosionDir.normalized * explosionForce, ForceMode.Impulse);
        }
    }
}
