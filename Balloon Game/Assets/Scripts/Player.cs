using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private float baseSpeedScale = 1f;
    [SerializeField] private int maxAir = 100;
    [SerializeField] private float directionThreshold = 0.05f;
    private float speedScale = 1f;
    private float speedBoostEndTime = 0f;
    private float shieldEndTime = 0f;
    private float invincibilityEndTime = 0f;
    private Rigidbody2D rb;
    private SpriteRenderer sr;
    private Animator animator;
    private int hashDirection;
    private int hashIsMoving;
    private Color defaultColor;
    private Vector2 prevVelocity;
    private Vector2 downPos;
    private int air;
    private bool isShielded = false;
    private bool isInvincible = false;

    [Header("Directional Sprites")]
    public Sprite idleSprite;
    public Sprite rightSprite;
    public Sprite leftSprite;
    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite rightUpSprite;
    public Sprite rightDownSprite;
    public Sprite leftUpSprite;
    public Sprite leftDownSprite;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        hashDirection = Animator.StringToHash("Direction");
        hashIsMoving = Animator.StringToHash("IsMoving");
        defaultColor = sr.color;
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

        if (IsInvincible() && Time.time >= invincibilityEndTime)
        {
            RemoveInvincibility();
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
            rb.angularVelocity = Mathf.MoveTowardsAngle(rb.angularVelocity, 0f, 0.1f);
            Debug.Log($"rb.angularVelocity={rb.angularVelocity}");
            // if (Mathf.Abs(rb.angularVelocity) == 0f)
            // {
            //     float currentAngle = rb.rotation;
            //     float newAngle = Mathf.MoveTowardsAngle(currentAngle, 0f, 2f); // 2f is degrees per frame, adjust for speed
            //     rb.MoveRotation(newAngle);
            // }
        }

        // Update animations according to movement direction
        UpdateSprite();
        prevVelocity = rb.linearVelocity;

        // Alpha blinking for invincibility
        if (isInvincible && sr != null)
        {
            float blink = Mathf.PingPong(Time.time * 10f, 1f);
            Color c = sr.color;
            c.a = Mathf.Lerp(0.3f, 1f, blink);
            sr.color = c;
        }
        else if (sr != null)
        {
            Color c = sr.color;
            c.a = 1f;
            sr.color = c;
        }
    }

    private void UpdateSprite()
    {
        if (sr == null || rb == null) return;
        Vector2 v = rb.linearVelocity;
        bool isMoving = v.sqrMagnitude > directionThreshold * directionThreshold;

        if (!isMoving)
        {
            if (idleSprite != null) sr.sprite = idleSprite;
            return;
        }

        float ax = Mathf.Abs(v.x);
        float ay = Mathf.Abs(v.y);

        if (ax > directionThreshold && ay > directionThreshold)
        {
            if (v.x > 0f && v.y > 0f && rightUpSprite != null) sr.sprite = rightUpSprite;
            else if (v.x > 0f && v.y < 0f && rightDownSprite != null) sr.sprite = rightDownSprite;
            else if (v.x < 0f && v.y > 0f && leftUpSprite != null) sr.sprite = leftUpSprite;
            else if (v.x < 0f && v.y < 0f && leftDownSprite != null) sr.sprite = leftDownSprite;
        }
        else if (ax >= ay)
        {
            sr.sprite = v.x >= 0f ? rightSprite : leftSprite;
        }
        else
        {
            sr.sprite = v.y >= 0f ? upSprite : downSprite;
        }
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

    public void ApplyInvincibility(float duration)
    {

        invincibilityEndTime = Time.time + duration;
        isInvincible = true;
        Debug.Log($"Player is invincible for {duration} seconds!");

        float blink = Mathf.PingPong(Time.time * 10f, 1f);
        Color c = sr.color;
        c.a = Mathf.Lerp(0.3f, 1f, blink);
        sr.color = c;

    }
    public bool IsInvincible()
    {
        return isInvincible;
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

    public void RemoveInvincibility()
    {
        isInvincible = false;
        Debug.Log("Invincibility removed!");
        sr.color = defaultColor;
    }

    public bool IsInBounds()
    {
        return transform.position.y >= GameManager.Instance.minY || transform.position.y <= GameManager.Instance.maxY || transform.position.x >= GameManager.Instance.minX || transform.position.x <= GameManager.Instance.maxX;
    }

}

