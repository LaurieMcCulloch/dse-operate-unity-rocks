using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [Header("Asteroid")]
    public float health = 50f;
    [Range(0,2)]
    public int size = 0;
    public float[] sizes = { 1f, 0.5f, 0.25f };
    public int[] points = { 20, 50, 100 };

    private GameManager gameManager;
    public GameObject asteroidPrefab;
    private Rigidbody rb;
    public Vector2 screenBounds;
    private Vector3 newPosition;
    [SerializeField] private  AudioClip[] audioClips;
    private AudioSource oAudio;


    public void Start()
    {
        gameManager = GameObject.FindObjectOfType<GameManager>();
        oAudio = this.GetComponent<AudioSource>();
        // Check asteroid is assigned 
        if (asteroidPrefab == null)
        {
            Debug.Log("ERROR: Asteroid Prefab not defined on Asteroid object");
            return;
        }

        rb = gameObject.GetComponent<Rigidbody>();

        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));

        // Set asteroid to random position and rotation if it is largest size i.e. not a child
        if (size == 0)
        {
            Vector3 startPosition = new Vector3(
                Random.Range(screenBounds.x * -1f, screenBounds.x)
                , Random.Range(screenBounds.y * -1, screenBounds.y)
                , 0f);

            if(startPosition != new Vector3(0,0,0))
            {
                transform.position = startPosition;
            }



            

            transform.rotation = Quaternion.Euler(Random.Range(-0.0f, 359.0f), Random.Range(-0.0f, 359.0f), Random.Range(-0.0f, 359.0f));
            rb.AddForce(transform.up * Random.Range(50f, 150f));
            rb.angularVelocity = transform.forward * Random.Range(-0.5f,-0.1f);
        }


        
    }

    public void Update()
    {
        // Screen Wrap around calaculation on Asteroid
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));

        if (Mathf.Abs(transform.position.x) > screenBounds.x * 1.15f)
        {
            newPosition = transform.position;
            newPosition.x *= -0.98f;
            transform.position = newPosition;
        }
        if (Mathf.Abs(transform.position.y) > screenBounds.y * 1.25f)
        {
            newPosition = transform.position;
            newPosition.y *= -0.98f;
            transform.position = newPosition;
        }
    }

    public void SetSize(int s)
    {
        // Set the asteroid size [0-2]
        size = s; 
        Vector3 asteroidScale = Vector3.one * sizes[s];
        transform.localScale = asteroidScale; 

    }

    public void TakeDamage(float amount)
    {
        // Decrease asteroid health when hit
        health -= amount; 
        if (health <= 0f)
        {
            // Check for asteroid death

            //oAudio.clip = audioClips[0];
            oAudio.Play();

            Die();
        }
    }




    void Die()
    {
        // Spawn two new smaller asteroids
        if (size >= 0 && size < sizes.Length - 1)
        {
            for (int loop = 0; loop < 2; loop++)
            {

                // Spawn new smaller asteroid
                GameObject newAsteroid = Instantiate(asteroidPrefab, transform.localPosition, transform.rotation);
                

         
   
                newAsteroid.GetComponent<Asteroid>().SetSize(size + 1);
                gameManager.asteroids.Add(newAsteroid);
                //newAsteroid.velocity = transform.up * bulletSpeed;
            }
        }
        // Destroy asteroid
        oAudio.clip = audioClips[Random.Range(0,2)];
        oAudio.Play();

        gameObject.GetComponent<Renderer>().enabled = false;
        gameObject.GetComponent<Collider>().enabled = false;

        // Increment Score
        gameManager.game.IncrementScore(points[size]);


        Invoke("DieLater", .25f); //enough time to play some audio
       


    }    

    void DieLater()
    {
        //gameObject.SetActive(false);
        gameManager.asteroids.Remove(gameObject);
        Destroy(gameObject);

        if (gameManager.asteroids.Count == 0)
        {
            gameManager.UpdateState(GameState.NEXT_LEVEL);
        }

    }
}
