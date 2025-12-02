using UnityEngine;

public class Arrow_Controller : MonoBehaviour
{
    public int damage = 1;
    public float lifetime = 5.0f;
    public float speed = 10.0f;

    private Vector2 direction;

    private AudioSource audioSource;
    public AudioClip hitSound;
    public AudioClip wallHitSound;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    // Update is called once per frame
    void Update()
    {
        transform.position += (Vector3)(direction * speed * Time.deltaTime);
    }

    
    public void Initialize(Vector2 dir, float setSpeed)
    {
        direction = dir.normalized;
        speed = setSpeed;
    }

    public void OnTriggerEnter2D(Collider2D collision)
    { 
        if(collision.CompareTag("Player"))
        {
            AudioSource.PlayClipAtPoint(hitSound, transform.position);
            PlayerHealth playerHealth = collision.GetComponent<PlayerHealth>();
            if(playerHealth != null)
            {
                playerHealth.TakeDamage(damage);
            }
            Destroy(gameObject);
        }

        if(collision.gameObject.layer == LayerMask.NameToLayer("SolidObjects"))
        {
            AudioSource.PlayClipAtPoint(wallHitSound, transform.position);
            Destroy(gameObject);
        }
    }
}
