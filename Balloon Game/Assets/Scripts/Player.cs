using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float speedScale = 5f;
    public Player player;
    public Rigidbody2D rb;
    private Vector2 prevVelocity;
    private Vector2 downPos;
    private int health = 3;

    void Awake()
    {
        if(player == null)
        {
            player = this;
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.position.y < -5f || transform.position.y > 5f || transform.position.x < -9f || transform.position.x > 9f)
        {
            Destroy(gameObject);
        }

        if (Input.GetMouseButtonDown(0))
        {
            downPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            // rb.AddForce((touchPos - transform.position).normalized * moveSpeed);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Vector2 upPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 newVelocity = upPos - downPos;
            //Debug.Log($"downPos={downPos}, upPos={upPos}, newVelocity={newVelocity}");
            // rb.AddForce(newVelocity * speedScale);
            rb.linearVelocity += newVelocity;
            //Debug.Log($"rb.velocity={rb.linearVelocity}");
        }
        else
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, 0.005f);
            rb.angularVelocity = Mathf.Lerp(rb.angularVelocity, 0f, 0.005f);
        }
        prevVelocity = rb.linearVelocity;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log($"Collision with {collision.gameObject.name}, normal={collision.GetContact(0).normal}, velocity={prevVelocity}");
        var firstContact = collision.GetContact(0);
        if (prevVelocity.sqrMagnitude < 0.001f)
            return;
        var reflectDir = Vector2.Reflect(prevVelocity.normalized, firstContact.normal); //gives a direction vector
        rb.linearVelocity = new Vector2(reflectDir.x, reflectDir.y) * prevVelocity.magnitude; //gives a velocity vector
        if (collision.gameObject.CompareTag("Enemy"))
        {
            health--;
            Debug.Log($"Player hit an enemy! Health: {health}");
        }
    }

    public int GetHealth()
    {
        return health;
    }
}


