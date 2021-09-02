using UnityEngine;

public class Bullet : MonoBehaviour
{

    private Rigidbody rb;

    [Header("Ballistics")]
    public float damage = 50f;
    public float duration = 1f;
    

    private void Start()
    {
        // Destroy bullet after duration
        Destroy(gameObject, duration);
    }

    void OnTriggerEnter(Collider col)
    {
        // Detect bullet collisions 
        if (col.gameObject.tag == "Asteroid")
        {
            Asteroid asteroid = col.gameObject.GetComponent<Asteroid>();
            if (asteroid != null)
            {
                asteroid.TakeDamage(damage);
                Destroy(gameObject);
            }
        }
        
    }

}
