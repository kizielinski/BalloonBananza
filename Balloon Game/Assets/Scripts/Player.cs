using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float baseSpeedScale = 1f;
    [SerializeField] private int maxAir = 100;
    private float speedScale = 1f;
    private float speedBoostEndTime = 0f;
    private float shieldEndTime = 0f;
    private Rigidbody2D rb;
    private Vector2 prevVelocity;
    private Vector2 downPos;
    private int air;
    private bool isShielded = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        speedScale = baseSpeedScale; // Store original speed
        air = maxAir;
    }

    // Update is called once per frame
    void Update()
    {
        // Check if speed boost has expired
        if (speedBoostEndTime > 0f && Time.time >= speedBoostEndTime)
        {
            EndSpeedBoost();
        }

        // Check if shield has expired
        if (IsShielded() && Time.time >= shieldEndTime)
        {
            RemoveShield();
        }

        // if (!IsInBounds())
        // {
        //     Destroy(gameObject);
        // }

        if (Input.GetMouseButtonDown(0))
        {
            downPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Vector2 upPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 newVelocity = upPos - downPos;
            // Debug.Log($"downPos={downPos}, upPos={upPos}, newVelocity={newVelocity}");
            rb.linearVelocity += newVelocity * speedScale;
            // Debug.Log($"rb.velocity={rb.linearVelocity}");
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
    }

    public int GetAir()
    {
        return air;
    }

    public int GetMaxAir()
    {
        return maxAir;
    }

    public void LoseAir(int amount)
    {
        air -= amount;
        Debug.Log($"Player lost {amount} air! Air: {air}");
    }

    public void RestoreAir(int amount)
    {
        air += amount;
        Debug.Log($"Player received {amount} air! Air: {air}");
    }

    public void ApplySpeedBoost(float speedMultiplier, float duration)
    {
        speedScale = baseSpeedScale * speedMultiplier;
        speedBoostEndTime = Time.time + duration;
        Debug.Log($"Player speed boosted by {speedMultiplier}x for {duration}s! New speedScale: {speedScale}");
    }

    public void EndSpeedBoost()
    {
        speedScale = baseSpeedScale;
        speedBoostEndTime = 0f;
        Debug.Log("Speed boost expired! Speed back to normal.");
    }

    public void AddShield(float duration)
    {
        shieldEndTime = Time.time + duration;
        isShielded = true;
        Debug.Log($"Shield added for {duration} seconds!");
    }

    public bool IsShielded()
    {
        return isShielded;
    }

    public void RemoveShield()
    {
        isShielded = false;
        Debug.Log("Shield removed!");
    }
    public bool IsInBounds()
    {
        return transform.position.y >= GameManager.Instance.minY || transform.position.y <= GameManager.Instance.maxY || transform.position.x >= GameManager.Instance.minX || transform.position.x <= GameManager.Instance.maxX;
    }

}

