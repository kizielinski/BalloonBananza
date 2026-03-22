using UnityEngine;

public class FollowBorder : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 2f;

    [Header("Raycast Settings")]
    public float groundRayLength = 0.25f;
    public float wallRayLength = 0.25f;
    public bool autoConfigureRayLengths = true;
    public float rayLengthPadding = 0.05f;

    [Tooltip("The Unity Layer your blocks are on. CRITICAL for this to work.")]
    public LayerMask groundLayer;

    [Tooltip("How far the object stays offset from the block surface after snapping to a corner.")]
    public float cornerAdjustment = 0.2f;

    [Tooltip("Minimum time between turns to prevent rapid corner flip-flopping.")]
    public float turnCooldown = 0.06f;

    private float turnCooldownTimer;
    private Collider2D cachedCollider;

    void Awake()
    {
        cachedCollider = GetComponent<Collider2D>();
        AutoConfigureRayLengths();
    }

    void OnValidate()
    {
        speed = Mathf.Max(0f, speed);
        cornerAdjustment = Mathf.Max(0f, cornerAdjustment);
        turnCooldown = Mathf.Max(0f, turnCooldown);
        rayLengthPadding = Mathf.Max(0.01f, rayLengthPadding);

        AutoConfigureRayLengths();
    }

    void Update()
    {
        if (turnCooldownTimer > 0f)
        {
            turnCooldownTimer -= Time.deltaTime;
        }

        // 1. Move forward continuously (transform.right is 'forward' in local space)
        transform.Translate(Vector2.right * speed * Time.deltaTime);

        // 2. Cast ray DOWN to detect the floor
        RaycastHit2D groundHit = Physics2D.Raycast(transform.position, -transform.up, groundRayLength, groundLayer);
        Debug.DrawRay(transform.position, -transform.up * groundRayLength, Color.green);

        // 3. Cast ray FORWARD to detect a wall
        RaycastHit2D wallHit = Physics2D.Raycast(transform.position, transform.right, wallRayLength, groundLayer);
        Debug.DrawRay(transform.position, transform.right * wallRayLength, Color.red);

        // --- Turning Logic ---

        bool canTurn = turnCooldownTimer <= 0f;

        // External Corner: The ground disappeared beneath us (ledge)
        // This check comes first so outside corners don't conflict with a simultaneous forward wall hit.
        if (canTurn && groundHit.collider == null)
        {
            Vector2 previousForward = transform.right;
            if (TryGetExternalCornerHit(previousForward, out RaycastHit2D newSurfaceHit))
            {
                // Rotate only once we know where the next surface is.
                transform.Rotate(0, 0, -90);
                SnapToSurface(newSurfaceHit);
                turnCooldownTimer = turnCooldown;
            }
        }
        // Internal Corner: We hit a wall directly in front of us.
        else if (canTurn && wallHit.collider != null)
        {
            // Rotate 90 degrees counter-clockwise to climb up the wall
            transform.Rotate(0, 0, 90);

            SnapToSurface(wallHit);
            turnCooldownTimer = turnCooldown;
        }
    }

    void SnapToSurface(RaycastHit2D hit)
    {
        Vector2 snappedPosition = hit.point + hit.normal * cornerAdjustment;
        transform.position = new Vector3(snappedPosition.x, snappedPosition.y, transform.position.z);
    }

    bool TryGetExternalCornerHit(Vector2 previousForward, out RaycastHit2D hit)
    {
        Vector2 oldDown = -transform.up;
        Vector2 nextSurfaceDirection = -previousForward;
        float snapRayLength = groundRayLength + wallRayLength + rayLengthPadding;
        Vector2 baseOrigin = (Vector2)transform.position + previousForward * groundRayLength;

        // Probe a few vertical offsets to survive tile seams and tiny misalignments.
        Vector2[] probeOrigins =
        {
            baseOrigin,
            baseOrigin + oldDown * cornerAdjustment,
            baseOrigin + oldDown * (cornerAdjustment + rayLengthPadding)
        };

        for (int i = 0; i < probeOrigins.Length; i++)
        {
            Vector2 probeOrigin = probeOrigins[i];
            RaycastHit2D probeHit = Physics2D.Raycast(probeOrigin, nextSurfaceDirection, snapRayLength, groundLayer);
            Debug.DrawRay(probeOrigin, nextSurfaceDirection * snapRayLength, Color.cyan);

            if (probeHit.collider != null)
            {
                hit = probeHit;
                return true;
            }
        }

        hit = default;
        return false;
    }

    void AutoConfigureRayLengths()
    {
        if (!autoConfigureRayLengths)
        {
            return;
        }

        if (cachedCollider == null)
        {
            cachedCollider = GetComponent<Collider2D>();
        }

        if (cachedCollider == null)
        {
            return;
        }

        float downwardExtent = GetColliderExtentInDirection(-transform.up);
        float forwardExtent = GetColliderExtentInDirection(transform.right);

        const float minRayLength = 0.02f;
        groundRayLength = Mathf.Max(minRayLength, downwardExtent + rayLengthPadding);
        wallRayLength = Mathf.Max(minRayLength, forwardExtent + rayLengthPadding);
    }

    float GetColliderExtentInDirection(Vector2 direction)
    {
        Vector2 dir = direction.normalized;
        if (dir.sqrMagnitude <= 0.0001f)
        {
            return 0f;
        }

        Vector2 origin = transform.position;
        float probeDistance = Mathf.Max(1f, cachedCollider.bounds.extents.magnitude * 3f);
        Vector2 probePoint = origin + dir * probeDistance;
        Vector2 closestPoint = cachedCollider.ClosestPoint(probePoint);
        float extent = Vector2.Dot(closestPoint - origin, dir);

        return Mathf.Max(0f, extent);
    }
}