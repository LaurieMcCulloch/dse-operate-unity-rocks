using UnityEngine;
using UnityEngine.InputSystem;

public class Ship : MonoBehaviour
{
    private GameManager gameManager;

    // Rigidbody for player ship physics
    private Rigidbody rb;
    public Vector2 screenBounds;
    private Vector3 newPosition;
    private AudioSource oAudio;
    public bool invulnerable = false;
    private Transform forceField;

    // Player inputs
    private float rotationInput = 0;
    private float thrustInput = 0;
    private float firingInput = 0;


    // Ship Movement Properties
    [Header("Movement")]
    public float thrustPower = 10;
    public float rotationalSpeed = 100.0f;


    // Ship Bullet Properties
    [Header("Bullets")]
    public Rigidbody bulletPrefab;
    public float bulletSpeed = 12;
    public float fireRate = 8f;
    private float nextTimeToFire = 0f;


    void Awake()
    {
        // Get Player Ship Rigidbody
        rb = gameObject.GetComponent<Rigidbody>();
        oAudio = this.GetComponent<AudioSource>();
        gameManager = GameObject.FindObjectOfType<GameManager>();
        forceField = this.gameObject.transform.GetChild(0);
        ToggleForceField(true);
    }




    void FixedUpdate()
    {
        // Rotate player ship        
        rb.AddRelativeTorque(Vector3.back * Time.deltaTime * rotationInput * rotationalSpeed);

        // Apply thrust to player ship
        rb.AddRelativeForce(Vector3.up * Time.deltaTime * thrustPower * thrustInput, ForceMode.Impulse);

    }




    void Update()
    {
        // Fire bullet if gun reloaded
        if (firingInput > 0f && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + 1f / fireRate;
            Shoot();
        }

        // Screen Wrap around calaculation on player ship
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));

        if (Mathf.Abs(transform.position.x) > screenBounds.x * 1.05f)
        {
            newPosition = transform.position;
            newPosition.x *= -0.99f;
            transform.position = newPosition;
        }
        if (Mathf.Abs(transform.position.y) > screenBounds.y * 1.05f)
        {
            newPosition = transform.position;
            newPosition.y *= -0.98f;
            transform.position = newPosition;
        }

    }




    void OnRotate(InputValue value)
    {
        // get rotation from 2D Vector X-axis in range -1 <[0]> 1         
        rotationInput = value.Get<Vector2>().x;
        ToggleForceField(false);
    }




    void OnThrust(InputValue value)
    {
        // get thrust from 2D Vector Y-axis in range 0 - 1
        thrustInput = value.Get<Vector2>().y;
        ToggleForceField(false);
    }




    void OnFire(InputValue value)
    {
        // Player presses FIRE   
        firingInput = value.Get<Vector2>().y;
        ToggleForceField(false);
    }




    void Shoot()
    {
        // Check bullet is assigned 
        if (bulletPrefab == null)
        {
            Debug.Log("ERROR: Bullet Prefab not defined on Player Ship");
            return;
        }

        // Shoot bullet
        Rigidbody bullet = Instantiate(bulletPrefab, transform.position, transform.rotation);
        bullet.velocity = transform.up * bulletSpeed;
    }


    void OnTriggerEnter(Collider col)
    {
        // Detect bullet collisions 
        if (col.gameObject.tag == "Asteroid" && !invulnerable)
        {
            Debug.Log("Player Died");
            gameManager.game.PlayerDied();
            gameManager.game.HandleUI();
            //Play Audio cue to explode
            oAudio.Play();

            //Die after half a second to let the audio time to play before being deactivated
            GetComponent<Renderer>().enabled = false;
            GetComponent<Collider>().enabled = false;


            Invoke("DieLater", .5f);

        }
      
    }

    public void Reset()
    {
        rb.velocity = Vector3.zero;
        transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        gameObject.SetActive(true);
        forceField.gameObject.SetActive(true);


    }

    private void DieLater()
    {
        gameObject.SetActive(false);
        gameManager.UpdateState(GameState.DIED);

        GetComponent<Renderer>().enabled = true;
        GetComponent<Collider>().enabled = true;

        ToggleForceField(true);
    }

    private void ToggleForceField(bool toggle)
    {
        invulnerable = toggle;
        forceField.gameObject.SetActive(toggle);
    }

}
