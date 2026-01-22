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

        if (Input.GetMouseButtonDown(0))
        {
            downPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            Vector2 upPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 newVelocity = upPos - downPos;
            rb.linearVelocity += newVelocity * speedScale;
        }
        else
        {
            rb.linearVelocity = Vector2.Lerp(rb.linearVelocity, Vector2.zero, 0.005f);
            rb.angularVelocity = Mathf.MoveTowardsAngle(rb.angularVelocity, 0f, 0.1f);
        }

        // Update animations according to movement direction
        UpdateAnimationState();
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

    private void UpdateAnimationState()
    {
        if (animator == null || rb == null) return;

        Vector2 v = rb.linearVelocity;
        bool isMoving = v.sqrMagnitude > directionThreshold * directionThreshold;
        animator.SetBool(hashIsMoving, isMoving);
        if (!isMoving) return;

        // 8-direction mapping based purely on velocity
        // 0=Right, 1=Left, 2=Up, 3=Down, 4=RightUp, 5=RightDown, 6=LeftUp, 7=LeftDown
        float ax = Mathf.Abs(v.x);
        float ay = Mathf.Abs(v.y);
        if (ax > directionThreshold && ay > directionThreshold)
        {
            if (v.x > 0f && v.y > 0f) { animator.SetInteger(hashDirection, 4); return; }
            if (v.x > 0f && v.y < 0f) { animator.SetInteger(hashDirection, 5); return; }
            if (v.x < 0f && v.y > 0f) { animator.SetInteger(hashDirection, 6); return; }
            if (v.x < 0f && v.y < 0f) { animator.SetInteger(hashDirection, 7); return; }
        }
        // Fallback to cardinal where one axis dominates or only one exceeds threshold
        if (ax >= ay)
        {
            animator.SetInteger(hashDirection, v.x >= 0f ? 0 : 1);
        }
        else
        {
            animator.SetInteger(hashDirection, v.y >= 0f ? 2 : 3);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var firstContact = collision.GetContact(0);
        if (prevVelocity.sqrMagnitude < 0.001f)
            return;

        var reflectDir = Vector2.Reflect(prevVelocity.normalized, firstContact.normal);
        rb.linearVelocity = new Vector2(reflectDir.x, reflectDir.y) * prevVelocity.magnitude;
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
    }

    public void RestoreAir(int amount)
    {
        air += amount;
    }

    public void ApplySpeedBoost(float speedMultiplier, float duration)
    {
        speedScale = baseSpeedScale * speedMultiplier;
        speedBoostEndTime = Time.time + duration;
    }

    public void EndSpeedBoost()
    {
        speedScale = baseSpeedScale;
        speedBoostEndTime = 0f;
    }

    public void AddShield(float duration)
    {
        shieldEndTime = Time.time + duration;
        isShielded = true;
    }

    public void ApplyInvincibility(float duration)
    {
        invincibilityEndTime = Time.time + duration;
        isInvincible = true;

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
    }

    public void RemoveInvincibility()
    {
        isInvincible = false;
        sr.color = defaultColor;
    }

    public bool IsInBounds()
    {
        return transform.position.y >= GameManager.Instance.minY && transform.position.y <= GameManager.Instance.maxY && transform.position.x >= GameManager.Instance.minX && transform.position.x <= GameManager.Instance.maxX;
    }

}

